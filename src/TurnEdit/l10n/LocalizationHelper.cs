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
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Markup;


namespace TurnEdit.l10n
{
    /// <summary>
    /// Language management class
    /// </summary>
    public static class LocalizationHelper
    {
        /// <summary>
        /// Gets available languages.
        /// </summary>
        /// <returns>Available languages as List. If directory is empty or doesn't exist, it returns only en-US.</returns>
        public static List<LanguageInfo> GetAvailableLanguages()
        {
            var languages = new List<LanguageInfo>();
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            string languagePath = System.IO.Path.Combine(appDir, "languages");
            if (!Directory.Exists(languagePath))
            {
                languages.Add(new LanguageInfo
                {
                    DisplayName = "English",
                    Code = "en-US"
                });
                return languages;
            }
            var files = Directory.GetFiles(languagePath, "*.xaml");
            foreach (var file in files)
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                try
                {
                    CultureInfo culture = new CultureInfo(fileName);
                    languages.Add(new LanguageInfo
                    {
                        Code = culture.Name,
                        DisplayName = culture.DisplayName
                    });
                } catch (Exception)
                {
                    continue;
                }
            }
            if (!languages.Any())
            {
                languages.Add(new LanguageInfo
                {
                    DisplayName = "English",
                    Code = "en-US"
                });
                return languages;
            }
            return languages.OrderBy(l => l.DisplayName).ToList();
        }
        /// <summary>
        /// Loads language resource file.
        /// </summary>
        /// <param name="Code">Language code</param>
        public static void LoadLanguageResource(string Code)
        {
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = System.IO.Path.Combine(appDir, "languages", $"{Code}.xaml");
            if (File.Exists(filePath))
            {
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        ResourceDictionary dict = (ResourceDictionary)XamlReader.Load(fs);
                        Application.Current.Resources.MergedDictionaries.Add(dict);
                    }
                    CultureInfo culture = new CultureInfo(Code);
                    System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
                } catch (Exception)
                {
                    return;
                }
            } else
            {
                return;
            }
        }
    }
}
