// 既存ファイルを以下の内容で置き換えてください（署名/チェックサムが無くてもプラグインを読み込むが、検証があれば使用する）。
using System;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Configuration;
using System.Windows.Media.TextFormatting;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;

namespace TurnEdit;

public static class PluginLoader
{
    public static List<PluginInformationsTemplate> LoadPlugins()
    {
        var plugins = new List<PluginInformationsTemplate>();
        string pluginsDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
        if (!Directory.Exists(pluginsDir))
        {
            return plugins;
        }
        try
        {
            var pluginFiles = Directory.GetFiles(pluginsDir, "*.dll");
            foreach (var file in pluginFiles)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(file);
                    foreach (var type in assembly.GetTypes())
                    {
                        if (typeof(PluginInformationsTemplate).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                        {
                            try
                            {
                                var plugin = Activator.CreateInstance(type) as PluginInformationsTemplate;
                                if (plugin != null)
                                {
                                    plugins.Add(plugin);
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"TurnEdit: error creating plugin instance from {file}: {ex.Message}");
                                // 続行
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"TurnEdit: error loading assembly {file}: {ex.Message}");
                    // 続行して他のプラグインを試す
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("TurnEdit: error: error listing plugins: " + ex.Message);
            return plugins;
        }
        return plugins;
    }

    public static string CalculateSha256(string fileName)
    {
        using var hash = System.Security.Cryptography.SHA256.Create();
        using var stream = System.IO.File.OpenRead(fileName);
        byte[] calculatedHash = hash.ComputeHash(stream);
        var sb = new System.Text.StringBuilder();
        foreach (byte b in calculatedHash)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }

    public static void InitPluginAll()
    {
        var plugins = new List<PluginInformationsTemplate>();
        string pluginsDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
        if (!Directory.Exists(pluginsDir))
        {
            return;
        }
        try
        {
            var pluginFiles = Directory.GetFiles(pluginsDir, "*.dll");
            foreach (var file in pluginFiles)
            {
                try
                {
                    // checksums ディレクトリにファイルがあれば検証を行う（無ければ警告してロードする）
                    string expectedFileHashDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "checksums");
                    string expectedFileHashTextFile = System.IO.Path.Combine(expectedFileHashDirectory, System.IO.Path.GetFileNameWithoutExtension(file) + "-sha256.txt");
                    bool verified = false;
                    if (File.Exists(expectedFileHashTextFile))
                    {
                        try
                        {
                            string expectedFileHash = System.IO.File.ReadAllText(expectedFileHashTextFile).Trim();
                            string actual = CalculateSha256(file);
                            if (string.Equals(actual, expectedFileHash, StringComparison.OrdinalIgnoreCase))
                            {
                                verified = true;
                                Debug.WriteLine($"TurnEdit: plugin {System.IO.Path.GetFileName(file)} checksum verified.");
                            }
                            else
                            {
                                Debug.WriteLine($"TurnEdit: plugin {System.IO.Path.GetFileName(file)} checksum mismatch. expected={expectedFileHash} actual={actual}. Plugin will be skipped.");
                                MessageBox.Show("Plugin checksum mismatch.");
                                // チェックサムが存在していて不一致ならスキップ（明示的に検証がある場合のみスキップ）
                                continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"TurnEdit: failed to verify checksum for {file}: {ex.Message}. This plugin will be skipped.");
                            continue;
                        }
                    }
                    else
                    {
                        // チェックサムファイルが無ければ警告するが、読み込みは行う（ユーザー要求）
                        Debug.WriteLine($"TurnEdit: plugin {System.IO.Path.GetFileName(file)} has no checksum file. Loading as unverified plugin.");
                    }

                    // Assembly を読み込み、PluginInfos を収集
                    Assembly assembly = Assembly.LoadFrom(file);
                    foreach (var type in assembly.GetTypes())
                    {
                        if (typeof(PluginInformationsTemplate).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                        {
                            try
                            {
                                var plugin = Activator.CreateInstance(type) as PluginInformationsTemplate;
                                if (plugin != null)
                                {
                                    plugins.Add(plugin);
                                    // LoadedPluginsFile 登録（存在すれば）
                                    try
                                    {
                                        PluginManager.LoadedPluginsFile.Add(file);
                                    }
                                    catch
                                    {
                                        // PluginManager が存在しない／例外でも継続
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"TurnEdit: failed to instantiate plugin type {type.FullName} in {file}: {ex.Message}");
                                // 続行
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"TurnEdit: error processing plugin file {file}: {ex.Message}");
                    // 続行
                }
            }

            // 各プラグインの初期化は個別に例外を捕捉してアプリ全体の停止を防ぐ
            foreach (var pluginTwo in plugins)
            {
                try
                {
                    pluginTwo.InitiaizePlugin();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"TurnEdit: plugin {pluginTwo?.GetType().FullName} initialize failed: {ex.Message}");
                    // 続行
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("TurnEdit: error: failure loading plugin when starting application: " + ex.Message);
            return;
        }
    }
}