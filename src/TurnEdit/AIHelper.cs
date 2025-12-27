using Microsoft.Win32;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;
using Google.GenAI;
using Google.GenAI.Types;
using OpenAI;
using System.Security.RightsManagement;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace TurnEdit.AI
{
    /// <summary>
    /// Specifies the available AI model versions supported by the application.
    /// </summary>
    /// <remarks>Use this enumeration to select a specific AI model when configuring or invoking AI-powered
    /// features. The values correspond to distinct model families and versions, which may differ in capabilities,
    /// performance, or cost. Refer to the application or service documentation for details about each model's
    /// characteristics and recommended usage scenarios.</remarks>
    public enum AIModels
    {
        Gemini25Flash = 1,
        Gemini25FlashLite = 2,
        Gemini25Pro = 3,
        Gemini3Pro = 4,
        GPT51 = 5,
        GPT5Mini = 6,
        GPT5Nano = 7,
        Grok41 = 8,
        Grok4 = 9,
        Grok3 = 10,
        Grok3Mini = 11,
        Grok2 = 12
    }
    /// <summary>
    /// Provides helper methods for performing AI-powered text operations such as summarization and proofreading using
    /// supported AI models.
    /// </summary>
    /// <remarks>The AIHelper class offers static methods that interact with various AI services to process
    /// and analyze text. Supported models include those with names starting with "gemini-" or "gpt-". Callers are
    /// responsible for supplying valid API keys when required by the underlying AI service. Methods may return null or
    /// throw exceptions if the specified model is not supported or if the AI service returns an invalid
    /// response.</remarks>
    public static partial class AIHelper
    {
        /// <summary>
        /// Deserializes the JSON string into an anonymous type specified by a sample object.
        /// </summary>
        /// <remarks>This method is typically used to deserialize JSON into anonymous types by providing a
        /// sample object that defines the desired structure. The sample object is used for type inference only and is
        /// not modified.</remarks>
        /// <typeparam name="T">The type of the anonymous object to deserialize to.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="anonymousTypeObject">An instance of the anonymous type that defines the structure to deserialize into. This parameter is used
        /// only for type inference and is not populated or modified.</param>
        /// <param name="options">Options to control the behavior during deserialization. If not specified, default options are used.</param>
        /// <returns>An instance of the anonymous type populated with values from the JSON string, or null if the JSON is null or
        /// empty.</returns>
        public static T? DeserializeAnonymousType<T>(string json, T anonymousTypeObject, JsonSerializerOptions options = default) => JsonSerializer.Deserialize<T>(json, options);

        // ヘルパ: X.AI (Grok) のレスポンスからテキストを抽出する
        private static string? ExtractXAIResponseText(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // 試行順序: choices[0].message.content  -> choices[0].message.content.text -> choices[0].text
                if (root.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                {
                    var first = choices[0];
                    if (first.TryGetProperty("message", out var message))
                    {
                        if (message.ValueKind == JsonValueKind.Object)
                        {
                            if (message.TryGetProperty("content", out var content))
                            {
                                if (content.ValueKind == JsonValueKind.String)
                                {
                                    return content.GetString();
                                }
                                if (content.ValueKind == JsonValueKind.Object)
                                {
                                    if (content.TryGetProperty("text", out var textProp) && textProp.ValueKind == JsonValueKind.String)
                                    {
                                        return textProp.GetString();
                                    }
                                }
                            }
                        }
                    }
                    if (first.TryGetProperty("text", out var text2) && text2.ValueKind == JsonValueKind.String)
                    {
                        return text2.GetString();
                    }
                }

                // 別パス: output[0].content[0].text
                if (root.TryGetProperty("output", out var output) && output.GetArrayLength() > 0)
                {
                    var out0 = output[0];
                    if (out0.TryGetProperty("content", out var outContent) && outContent.GetArrayLength() > 0)
                    {
                        var c0 = outContent[0];
                        if (c0.ValueKind == JsonValueKind.Object)
                        {
                            if (c0.TryGetProperty("text", out var t3) && t3.ValueKind == JsonValueKind.String)
                            {
                                return t3.GetString();
                            }
                            if (c0.TryGetProperty("message", out var m2) && m2.ValueKind == JsonValueKind.Object)
                            {
                                if (m2.TryGetProperty("content", out var m2c) && m2c.ValueKind == JsonValueKind.String)
                                {
                                    return m2c.GetString();
                                }
                            }
                        }
                    }
                }

                // フォールバック: root から最初の文字列ノードを探索する（安全策）
                foreach (var prop in root.EnumerateObject())
                {
                    if (prop.Value.ValueKind == JsonValueKind.String)
                    {
                        return prop.Value.GetString();
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        /// <summary>
        /// Asynchronously generates a summary of the specified text using the selected AI model.
        /// </summary>
        /// <remarks>The method supports models with names starting with "gemini-" or "gpt-". If an
        /// unsupported model is specified, the method returns null. The caller is responsible for providing a valid API
        /// key for models that require authentication.</remarks>
        /// <param name="text">The text to be summarized. Cannot be null or empty.</param>
        /// <param name="model">The name of the AI model to use for summarization. Must start with a supported model prefix such as
        /// "gemini-" or "gpt-" or "grok-".</param>
        /// <param name="apiKey">The API key used to authenticate with the AI service. Required for models that need authentication.</param>
        /// <returns>A string containing the summarized text, or null if the model is not supported.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the AI service returns a null or invalid response.</exception>
        public static async Task<string?> SummarizeTextAsync(string text, string model, string apiKey)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(text);
            if (model.StartsWith("gemini-"))
            {
                var client = new Client(apiKey: apiKey);
                var response = await client.Models.GenerateContentAsync(
                    model: model, contents: $"Summarize the following text: {text}"
                );
                if (response != null && response.Candidates[0] != null && response.Candidates[0].Content != null && response.Candidates[0].Content.Parts[0] != null && response.Candidates[0].Content.Parts[0].Text != null)
                {
                    return response!.Candidates[0]!.Content!.Parts[0]!.Text!;
                }
                else
                {
                    throw new InvalidOperationException("AI-Generated content is null.");
                }
            }
            else if (model.StartsWith("gpt-"))
            {
                using var openAIApi = new OpenAIClient(apiKey);
                var response = await openAIApi.ResponsesEndpoint.CreateModelResponseAsync($"Summarize the following text: {text}");
                var responseItem = response.Output.LastOrDefault();
                string responseText = responseItem.ToString();
                if (response != null && responseItem != null && responseText != null)
                {
                    return responseText;
                }
                else
                {
                    throw new InvalidOperationException("AI-Generated content is null.");
                }
            }
            else if (model.StartsWith("grok-"))
            {
                // Grok (X.AI) path
                // apiKey may be required depending on provider; SendXAIRequestAsync will attach Authorization header.
                string prompt = $"Summarize the following text: {text}";
                string json = await SendXAIRequestAsync(prompt, model, apiKey);
                string? result = ExtractXAIResponseText(json);
                if (!string.IsNullOrWhiteSpace(result))
                    return result;
                throw new InvalidOperationException("AI-Generated content is null.");
            }
            return null;
        }
        /// <summary>
        /// Asynchronously proofreads the specified text using an AI language model and returns the corrected version.
        /// </summary>
        /// <remarks>The method selects the AI provider based on the prefix of the model parameter. For
        /// models starting with "gemini-", the Gemini API is used; for models starting with "gpt-", the OpenAI API is
        /// used; for models starting with "grok-", the Grok API is used. Ensure that the apiKey parameter is valid and appropriate for the selected provider.</remarks>
        /// <param name="text">The text to be proofread. Cannot be null or empty.</param>
        /// <param name="model">The identifier of the AI model to use for proofreading. Must start with either "gemini-" or "gpt-" to select
        /// the appropriate provider.</param>
        /// <param name="apiKey">The API key used to authenticate with the AI provider. Required when using a model that starts with "gpt-".</param>
        /// <returns>A string containing the proofread version of the input text, or null if the model type is not supported.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the AI-generated content is unexpectedly null.</exception>
        public static async Task<string?> ProofreadTextAsync(string text, string model, string apiKey)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(text);
            if (model.StartsWith("gemini-"))
            {
                var client = new Client(apiKey: apiKey);
                var response = await client.Models.GenerateContentAsync(
                    model: model, contents: $"Proofread the following text: {text}"
                );
                if (response != null && response.Candidates[0] != null && response.Candidates[0].Content != null && response.Candidates[0].Content.Parts[0] != null && response.Candidates[0].Content.Parts[0].Text != null)
                {
                    return response!.Candidates[0]!.Content!.Parts[0]!.Text!;
                }
                else
                {
                    throw new InvalidOperationException("AI-Generated content is null.");
                }
            }
            else if (model.StartsWith("gpt-"))
            {
                using var openAIApi = new OpenAIClient(apiKey);
                var response = await openAIApi.ResponsesEndpoint.CreateModelResponseAsync($"Proofread the following text: {text}");
                var responseItem = response.Output.LastOrDefault();
                string responseText = responseItem.ToString();
                if (response != null && responseItem != null && responseText != null)
                {
                    return responseText;
                }
                else
                {
                    throw new InvalidOperationException("AI-Generated content is null.");
                }
            }
            else if (model.StartsWith("grok-"))
            {
                string prompt = $"Proofread the following text: {text}";
                string json = await SendXAIRequestAsync(prompt, model, apiKey);
                string? result = ExtractXAIResponseText(json);
                if (!string.IsNullOrWhiteSpace(result))
                    return result;
                throw new InvalidOperationException("AI-Generated content is null.");
            }
            return null;
        }

        /// <summary>
        /// Generates a continuation of the specified text using the selected AI model.
        /// </summary>
        /// <remarks>The method supports models with prefixes "gemini-" and "gpt-". If an unsupported
        /// model prefix is provided, the method returns null. The caller is responsible for ensuring that the API key
        /// is valid for the selected model.</remarks>
        /// <param name="text">The input text to be extended by the AI model. Cannot be null or empty.</param>
        /// <param name="model">The identifier of the AI model to use for text generation. Must start with a supported model prefix such as
        /// "gemini-" or "gpt-".</param>
        /// <param name="apiKey">The API key used to authenticate requests to the AI service. Required for models that need authentication.</param>
        /// <returns>A string containing the AI-generated continuation of the input text, or null if the model is not supported.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the AI-generated content is null or cannot be retrieved from the service.</exception>
        public static async Task<string?> WriteMoreAsync(string text, string model, string apiKey)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(text);
            if (model.StartsWith("gemini-"))
            {
                var client = new Client(apiKey: apiKey);
                var response = await client.Models.GenerateContentAsync(
                    model: model, contents: $"Write more the following text: {text}"
                );
                if (response != null && response.Candidates[0] != null && response.Candidates[0].Content != null && response.Candidates[0].Content.Parts[0] != null && response.Candidates[0].Content.Parts[0].Text != null)
                {
                    return response!.Candidates[0]!.Content!.Parts[0]!.Text!;
                }
                else
                {
                    throw new InvalidOperationException("AI-Generated content is null.");
                }
            }
            else if (model.StartsWith("gpt-"))
            {
                using var openAIApi = new OpenAIClient(apiKey);
                var response = await openAIApi.ResponsesEndpoint.CreateModelResponseAsync($"Write more the following text: {text}");
                var responseItem = response.Output.LastOrDefault();
                string responseText = responseItem.ToString();
                if (response != null && responseItem != null && responseText != null)
                {
                    return responseText;
                }
                else
                {
                    throw new InvalidOperationException("AI-Generated content is null.");
                }
            }
            else if (model.StartsWith("grok-"))
            {
                string prompt = $"Write more the following text: {text}";
                string json = await SendXAIRequestAsync(prompt, model, apiKey);
                string? result = ExtractXAIResponseText(json);
                if (!string.IsNullOrWhiteSpace(result))
                    return result;
                throw new InvalidOperationException("AI-Generated content is null.");
            }
            return null;
        }

        /// <summary>
        /// Sends a chat completion request to the X.AI API using the specified message, model, and API key.
        /// </summary>
        /// <remarks>The returned JSON string contains the full response from the X.AI API, which may
        /// include the generated message and additional metadata. Callers are responsible for parsing the response as
        /// needed. This method throws an exception if the HTTP request is unsuccessful.</remarks>
        /// <param name="message">The user message to include in the chat request. This will be sent as the content of the user role.</param>
        /// <param name="model">The identifier of the X.AI model to use for generating the response. Must be a valid model supported by the
        /// X.AI API.</param>
        /// <param name="apiKey">The API key used to authenticate the request with the X.AI service. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the raw JSON response from the
        /// X.AI API as a string.</returns>
        public static async Task<string> SendXAIRequestAsync(string message, string model, string apiKey)
        {
            using HttpClient client = new();
            var requestData = new
            {
                model = model,
                messages = new[]
                {
                    new {role = "user", content = message}
                }
            };
            string jsonPayload = JsonSerializer.Serialize(requestData);
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
            }
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api.x.ai/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Returns the string identifier corresponding to the specified AI model.
        /// </summary>
        /// <param name="model">The AI model for which to retrieve the string identifier.</param>
        /// <returns>A string representing the identifier of the specified AI model, or null if the model is not recognized.</returns>
        public static string? GetAIModelString(AIModels model)
        {
            switch (model)
            {
                case AIModels.Gemini25Flash:
                    return "gemini-2.5-flash";
                case AIModels.Gemini25FlashLite:
                    return "gemini-2.5-flash-lite";
                case AIModels.Gemini25Pro:
                    return "gemini-2.5-pro";
                case AIModels.Gemini3Pro:
                    return "gemini-3-pro-preview";
                case AIModels.GPT51:
                    return "gpt-5.1-chat-latest";
                case AIModels.GPT5Mini:
                    return "gpt-5-mini-2025-08-07";
                case AIModels.GPT5Nano:
                    return "gpt-5-nano-2025-08-07";
                case AIModels.Grok2:
                    return "grok-2-vision-1212";
                case AIModels.Grok3:
                    return "grok-3";
                case AIModels.Grok3Mini:
                    return "grok-3-mini";
                case AIModels.Grok4:
                    return "grok-4-fast-reasoning";
                case AIModels.Grok41:
                    return "grok-4-1-fast-reasoning";
            }

            return null;
        }
    }
}