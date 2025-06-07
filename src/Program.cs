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
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
		Application.EnableVisualStyles();
        Application.Run(new Form1());
    }    
}