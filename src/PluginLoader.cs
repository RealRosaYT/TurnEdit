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

namespace TurnEdit;

public static class PluginLoader
{
    public static List<PluginInformationsTemplate> LoadPlugins()
    {
        var plugins = new List<PluginInformationsTemplate>();
        if (!Directory.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins")))
        {
            return plugins;
        }
        try
        {
            var pluginFiles = Directory.GetFiles(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins"), "*.dll");
            foreach (var file in pluginFiles)
            {
                Assembly assembly = Assembly.LoadFrom(file);
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(PluginInformationsTemplate).IsAssignableFrom(type) && !type.IsInterface)
                    {
                        var plugin = Activator.CreateInstance(type) as PluginInformationsTemplate;
                        if (plugin != null)
                        {
                            plugins.Add(plugin);
                        }
                    }
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
    public static string CalculateSha256(string fileName) {
		System.Security.Cryptography.SHA256 hash = System.Security.Cryptography.SHA256.Create();
		System.IO.FileStream stream = System.IO.File.OpenRead(fileName);
		byte[] calculatedHash = hash.ComputeHash(stream);
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		foreach (byte b in calculatedHash) {
			sb.Append(b.ToString("x2"));
		}
		return sb.ToString();
	}
    public static void InitPluginAll()
    {
        var plugins = new List<PluginInformationsTemplate>();
        if (!Directory.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins")))
        {
            return;
        }
        try
        {
            var pluginFiles = Directory.GetFiles(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins"), "*.dll");
            foreach (var file in pluginFiles)
            {
				string expectedFileHashDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "checksums");
				string expectedFileHashTextFile = System.IO.Path.Combine(expectedFileHashDirectory, System.IO.Path.GetFileNameWithoutExtension(file) + "-sha256.txt");
				if (!File.Exists(expectedFileHashTextFile)) {
					// Skip plugin loading
					continue;
				}
				string expectedFileHash = System.IO.File.ReadAllText(expectedFileHashTextFile);
				if (CalculateSha256(file) == expectedFileHash) {
					// Do nothing
				} else {
					// Skip plugin loading
					continue;
				}
                Assembly assembly = Assembly.LoadFrom(file);
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(PluginInformationsTemplate).IsAssignableFrom(type) && !type.IsInterface)
                    {
                        var plugin = Activator.CreateInstance(type) as PluginInformationsTemplate;
						PluginManager.LoadedPluginsFile.Add(file);
                        plugins.Add(plugin);
                    }
                }
            }
            foreach (var pluginTwo in plugins)
            {
                pluginTwo.InitiaizePlugin();
            }
        }
        catch (Exception ex)
        {
			Debug.WriteLine("TurnEdit: error: failure loading plugin when starting application: " + ex.Message);
            return;
        }
        return;
    }
}