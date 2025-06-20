namespace TurnEdit;

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Diagnostics;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
		Application.EnableVisualStyles();
		if (args.Length > 0) {
			string filePath = args[0];
			if (File.Exists(filePath)) {
				Application.Run(new Form1(filePath));
			} else {
				MessageBox.Show($@"File {filePath} not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Application.Run(new Form1());
			}
		} else {
			Application.Run(new Form1());
		}
    }    
}