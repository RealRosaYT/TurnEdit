using System.Windows.Forms;

namespace TurnEdit;

public class SettingsJSONSyntax {
	public string FontFamily { get; set; } = "Segoe UI";
	public int FontSize { get; set; } = 11;
	public bool SaveBackupWhenFileSave { get; set; } =  false;
}

public partial class Form1 : Form
{
    public System.Windows.Forms.TextBox? maintextbox;
    public new MenuStrip? MainMenuStrip;
    public int currentformheight;
    private string? currentfilename;
      public string? SettingsSavedDirectory;
    private SettingsJSONSyntax? TurnEditSettings;
    private string? SettingsFilePath;
	private TurnEditTextCounter? textCounterForm;
	private StatusStrip? mainstatus;
	private ToolStripStatusLabel? StatusColumnAndLine;
	private bool? ChangesUnsaved;
    public Form1()
    {
        InitializeComponent();
        TurnEditPrepare();
        CreateDefaultSettings();
        LoadTurnEditSettings();
    }
    public void TurnEditPrepare()
    {
		this.textCounterForm = new TurnEditTextCounter(this);
        this.Text = "New File - TurnEdit";
        this.Size = new Size(1000, 700);
		var IconPath = @Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "turnedit-icon.ico");
		if (File.Exists(IconPath)) {
			this.Icon = new Icon(IconPath);
		}
        this.currentformheight = this.Size.Height;
        TurnEditGUI();
		this.ChangesUnsaved = false;
		// this.textCounterForm = new TurnEditTextCounter(this);
        }
    public void TurnEditGUI()
    {
        this.MainMenuStrip = new MenuStrip();
        this.MainMenuStrip.Size = new Size(800, 25);
        this.MainMenuStrip.Dock = DockStyle.Top;
        this.maintextbox = new System.Windows.Forms.TextBox();
        this.maintextbox.Dock = DockStyle.Fill;
        this.maintextbox.Multiline = true;
		this.mainstatus = new StatusStrip();
		this.mainstatus.Dock = DockStyle.Bottom;
		this.StatusColumnAndLine = new ToolStripStatusLabel();
		this.StatusColumnAndLine.Text = "Line: Column: ";
		this.mainstatus.Items.Add(this.StatusColumnAndLine);
        /* "File" Menu */
        ToolStripMenuItem FileMenuItem = new ToolStripMenuItem();
        FileMenuItem.Name = "FileMenuItem";
        FileMenuItem.Text = "File";
        ToolStripMenuItem FileNewMenuItem = new ToolStripMenuItem();
        FileNewMenuItem.Name = "FileNewMenuItem";
        FileNewMenuItem.Text = "New";
        FileNewMenuItem.Click += new EventHandler(this.FileNewMenuItem_Click);
        ToolStripMenuItem FileOpenMenuItem = new ToolStripMenuItem();
        FileOpenMenuItem.Name = "FileOpenMenuItem";
        FileOpenMenuItem.Text = "Open";
        FileOpenMenuItem.Click += new EventHandler(this.FileOpenMenuItem_Clicked);
        ToolStripMenuItem FileSaveMenuItem = new ToolStripMenuItem();
        FileSaveMenuItem.Name = "FileSaveMenuItem";
        FileSaveMenuItem.Text = "Save";
		FileSaveMenuItem.Click += new EventHandler(this.FileSaveMenuItem_Click);
        ToolStripMenuItem FileSaveAsMenuItem = new ToolStripMenuItem();
        FileSaveAsMenuItem.Name = "FileSaveAsMenuItem";
        FileSaveAsMenuItem.Text = "Save As";
        FileSaveAsMenuItem.Click += new EventHandler(this.FileSaveAsMenuItem_Click);
		ToolStripMenuItem FileExitMenuItem = new ToolStripMenuItem();
		FileExitMenuItem.Name = "FileExitMenuItem";
		FileExitMenuItem.Text = "Exit";
		FileExitMenuItem.Click += new EventHandler(this.FileExitMenuItem_Click);
        FileMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            FileNewMenuItem,
            FileOpenMenuItem,
            FileSaveMenuItem,
            FileSaveAsMenuItem,
			FileExitMenuItem
        });
        /* "Edit" Menu */
        ToolStripMenuItem EditMenuItem = new ToolStripMenuItem();
        EditMenuItem.Name = "EditMenuItem";
        EditMenuItem.Text = "Edit";
        ToolStripMenuItem  EditSearchMenuItem = new ToolStripMenuItem();
        EditSearchMenuItem.Name = "EditSearchMenuItem";
        EditSearchMenuItem.Text = "Search";
        EditSearchMenuItem.Click += new EventHandler(this.EditSearchMenuItem_Click);
        ToolStripMenuItem EditReplaceMenuItem = new ToolStripMenuItem();
        EditReplaceMenuItem.Name = "EditReplaceMenuItem";
        EditReplaceMenuItem.Text = "Replace";
		ToolStripMenuItem EditNowMenuItem = new ToolStripMenuItem();
		EditNowMenuItem.Name = "EditNowMenuItem";
		EditNowMenuItem.Text = "Insert date and time";
		EditNowMenuItem.Click += new EventHandler(this.EditNowMenuItem_Click);
        EditReplaceMenuItem.Click += new EventHandler(this.EditReplaceMenuItem_Click);
        EditMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            EditSearchMenuItem,
            EditReplaceMenuItem,
			EditNowMenuItem
        });
        /* "Settings" Menu */
        ToolStripMenuItem SettingsMenuItem = new ToolStripMenuItem();
        SettingsMenuItem.Name = "SettingsMenuItem";
        SettingsMenuItem.Text = "Settings";
        ToolStripMenuItem SettingsFontMenuItem = new ToolStripMenuItem();
        SettingsFontMenuItem.Name = "SettingsFontMenuItem";
        SettingsFontMenuItem.Text = "Font";
        SettingsFontMenuItem.Click += new EventHandler(this.SettingsFontMenuItem_Click);
        SettingsMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            SettingsFontMenuItem
        });
		/* "Tools" Menu */
		ToolStripMenuItem ToolsMenuItem = new ToolStripMenuItem();
		ToolsMenuItem.Name = "ToolsMenuItem";
		ToolsMenuItem.Text = "Tools";
		ToolStripMenuItem ToolsCounterMenuItem = new ToolStripMenuItem();
		ToolsCounterMenuItem.Name = "ToolsCounterMenuItem";
		ToolsCounterMenuItem.Text = "Text counter";
		ToolsCounterMenuItem.Click += new EventHandler(this.ToolsCounterMenuItem_Click);
		ToolsMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
			ToolsCounterMenuItem
		});
        /* "Help" Menu */
        ToolStripMenuItem HelpMenuItem = new ToolStripMenuItem();
        HelpMenuItem.Name = "HelpMenuItem";
        HelpMenuItem.Text = "Help";
		ToolStripMenuItem HelpOnlineMenuItem = new ToolStripMenuItem();
		HelpOnlineMenuItem.Name = "HelpOnlineMenuItem";
		HelpOnlineMenuItem.Text = "Online help";
		HelpOnlineMenuItem.Click += new EventHandler(this.HelpOnlineMenuItem_Click);
        ToolStripMenuItem HelpAboutMenuItem = new ToolStripMenuItem();
        HelpAboutMenuItem.Name = "HelpAboutMenuItem";
        HelpAboutMenuItem.Text = "About";
        HelpAboutMenuItem.Click += new EventHandler(this.HelpAboutMenuItem_Clicked);
        HelpMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            HelpOnlineMenuItem,
			HelpAboutMenuItem
        });
        this.MainMenuStrip.Items.AddRange(new ToolStripItem[] {
            FileMenuItem,
            EditMenuItem,
			ToolsMenuItem,
            SettingsMenuItem,
            HelpMenuItem
        });
		this.Controls.Add(this.maintextbox);
        this.Controls.Add(MainMenuStrip);
		this.Controls.Add(this.mainstatus);
		this.maintextbox.TextChanged += new EventHandler(this.maintextboxTextChangedEvent);
		this.maintextbox.KeyUp += new KeyEventHandler(this.maintextbox_KeyUp);
		this.FormClosing += new FormClosingEventHandler(this.mainform_closing);
		UpdateColumnAndLineStatus();
    }
	public Form1(string filePath) : this()
	{
			try {
				if (File.Exists(filePath)) {
					string OpenFileContent = File.ReadAllText(filePath);
					this.maintextbox!.Text = OpenFileContent;
					this.Text = @$"{filePath} - TurnEdit";
					this.currentfilename = filePath;
					this.ChangesUnsaved = false;
				} else {
					MessageBox.Show($@"File {filePath} not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			} catch (Exception ex) {
				MessageBox.Show($@"An error occurred during opening file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
	}
	public void mainform_closing(object? sender, FormClosingEventArgs e) {
		if (ChangesUnsaved == true) {
			DialogResult result = MessageBox.Show(
			"Changes are unsaved. do you want save changes?",
			"Warning",
			MessageBoxButtons.YesNoCancel,
			MessageBoxIcon.Warning
			);
		if (result == DialogResult.Yes) {
			bool savesuccessful;
			if (currentfilename is not null) {
				savesuccessful = SaveCurrentFile();
			} else {
				savesuccessful = SaveAs();
			}
			if (!savesuccessful) {
				e.Cancel = true;
			}
		} else if (result == DialogResult.No) {
		
		} else {
			e.Cancel = true;
		}
		}
	}
	public void maintextbox_KeyUp(object? sender, KeyEventArgs e) {
		UpdateColumnAndLineStatus();
	}
	public void UpdateColumnAndLineStatus() {
		if (this.maintextbox!.Text.Length == 0) {
			this.StatusColumnAndLine!.Text = "Line: 0 Column: 0";
			return;
		}
		int caretPosition = this.maintextbox.SelectionStart;
		int line = this.maintextbox.GetLineFromCharIndex(caretPosition);
		int displayLine = line + 1;
		
		int firstCharOfLine = this.maintextbox.GetFirstCharIndexFromLine(line);
		int column = caretPosition - firstCharOfLine + 1;
		this.StatusColumnAndLine!.Text = $"Line: {displayLine} Column: {column}";
	}
	public bool SaveAs() {
		try {
			SaveFileDialog savedialog = new SaveFileDialog();
			savedialog.ShowHelp = true;
			savedialog.Filter = "Text file (*.txt)|*.txt|All file (*.*)|*.*";
			if (savedialog.ShowDialog() == DialogResult.OK) {
				string savefilename = savedialog.FileName;
				File.WriteAllText(savefilename, this.maintextbox!.Text);
				this.Text = @$"{savefilename} - TurnEdit";
				this.currentfilename = savefilename;
				this.ChangesUnsaved = false;
				return true;
			}
			return false;
		} catch (Exception ex) {
			MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return false;
		}
	}
	public bool SaveCurrentFile() {
		if (this.currentfilename != null) {
			try {
				File.WriteAllText(this.currentfilename!, this.maintextbox!.Text);
				this.ChangesUnsaved = false;
				return true;
			} catch (Exception ex) {
				MessageBox.Show($@"Failed to save file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		} else {
			MessageBox.Show("Please open file.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return false;
		}
	}
    public void FileNewMenuItem_Click(object? sender, EventArgs e)
    {
        this.maintextbox!.Clear();
		this.currentfilename = null;
		this.ChangesUnsaved = true;
    }
    public void HelpAboutMenuItem_Clicked(object? sender, EventArgs e) {
        var AboutForm = new TurnEditAboutForm();
        AboutForm.Show();
    }
    public void FileOpenMenuItem_Clicked(object? sender, EventArgs e) {
        OpenFileDialog opendialog = new OpenFileDialog();
        opendialog.ShowHelp = true;
        opendialog.CheckFileExists = true;
        opendialog.Filter = "Text file(*.txt)|*.txt|All file (*.*)|*.*";
        if (opendialog.ShowDialog() == DialogResult.OK) {
            string OpenedFileName = opendialog.FileName;
            string OpenFileContent = File.ReadAllText(OpenedFileName);
            this.maintextbox!.Text = OpenFileContent;
            this.Text = @$"{OpenedFileName} - TurnEdit";
            this.currentfilename = OpenedFileName;
			this.ChangesUnsaved = false;
        }
    }
    public void FileSaveAsMenuItem_Click(object? sender, EventArgs e) {
        SaveAs();
    }
	public void FileExitMenuItem_Click(object? sender, EventArgs e) {
		Application.Exit();
	}
    public void EditSearchMenuItem_Click(object? sender, EventArgs e) {
        var SearchForm = new TurnEditSearchForm(this);
        SearchForm.Show();
    }
    public void EditReplaceMenuItem_Click(object? sender, EventArgs e) {
        var ReplaceForm = new TurnEditReplaceForm(this);
        ReplaceForm.Show();
    }
	public void EditNowMenuItem_Click(object? sender, EventArgs e) {
		DateTime now = DateTime.Now;
		string nowStr = now.ToString();
		this.maintextbox!.SelectedText = nowStr;
		this.maintextbox.SelectionStart = this.maintextbox.SelectionStart + nowStr.Length;
		this.maintextbox.ScrollToCaret();
	}
    public void SettingsFontMenuItem_Click(object? sender, EventArgs e) {
        FontDialog fontdlg = new FontDialog();
        fontdlg.Font = this.maintextbox!.Font;
        if (fontdlg.ShowDialog() == DialogResult.OK) {
            this.maintextbox.Font = fontdlg.Font;
            this.TurnEditSettings!.FontFamily = fontdlg.Font.FontFamily.Name;
            this.TurnEditSettings.FontSize = (int)fontdlg.Font.Size;
            SaveTurnEditSettings(this.TurnEditSettings);
        }
    }
    public void CreateDefaultSettings() {
    	this.TurnEditSettings = new SettingsJSONSyntax
    	{
    		FontFamily = "Segoe UI",
    		FontSize = 11,
    		SaveBackupWhenFileSave = false
    	};
    }
    public bool SaveTurnEditSettings(SettingsJSONSyntax settings) {
    	try {
    		this.SettingsFilePath = Path.Combine(Application.LocalUserAppDataPath, "turnedit-settings.json");
    		System.Text.Json.JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
    		string JSONString = System.Text.Json.JsonSerializer.Serialize(settings, options);
    		File.WriteAllText(this.SettingsFilePath, JSONString);
    		return true;
    	} catch (Exception ex) {
    		MessageBox.Show($@"Error saving settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    		return false;
    	}
    }
    public bool LoadTurnEditSettings() {
    	try {
    	var SettingsJSONPathForLoad = Path.Combine(Application.LocalUserAppDataPath, "turnedit-settings.json");
    	var SettingsJSONContent = File.ReadAllText(SettingsJSONPathForLoad);
    	SettingsJSONSyntax? DeserializedJSON = System.Text.Json.JsonSerializer.Deserialize<SettingsJSONSyntax>(SettingsJSONContent);
    	if (DeserializedJSON is not null) {
			this.maintextbox!.Font = new Font(DeserializedJSON.FontFamily, DeserializedJSON.FontSize);
			this.TurnEditSettings = DeserializedJSON;
		}
		return true;
    	} catch (Exception ex) {
    	MessageBox.Show($@"Failed to load settings: {ex.Message} . Use default settings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    	CreateDefaultSettings();
    	this.maintextbox!.Font = new Font(this.TurnEditSettings!.FontFamily, this.TurnEditSettings!.FontSize);
    	return false;
    	}
    }
	public void ToolsCounterMenuItem_Click(object? sender, EventArgs e) {
		var CounterInstanceForShowDlg = new TurnEditTextCounter(this);
		CounterInstanceForShowDlg.Show();
	}
	public void maintextboxTextChangedEvent(object? sender, EventArgs e) {
		
		this.textCounterForm!.UpdateCounter();
		this.ChangesUnsaved = true;
	}
	public void FileSaveMenuItem_Click(object? sender, EventArgs e) {
		SaveCurrentFile();
	}
	public void HelpOnlineMenuItem_Click(object? sender, EventArgs e) {
		try {
			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo{
			FileName = "https://github.com/suzuki3932/TurnEdit/wiki",
			UseShellExecute = true
			});
		} catch (Exception ex) {
			MessageBox.Show($@"Failed to open online help: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
