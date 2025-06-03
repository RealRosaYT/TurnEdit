namespace TurnEdit;

public class SettingsJSONSyntax {
	public string FontFamily { get; set; } = "Segoe UI";
	public int FontSize { get; set; } = 11;
	public bool SaveBackupWhenFileSave { get; set; } =  false;
}

public partial class Form1 : Form
{
    public TextBox? maintextbox;
    public MenuStrip? MainMenuStrip;
    public int currentformheight;
    private string? currentfilename;
      public string SettingsSavedDirectory;
    private SettingsJSONSyntax TurnEditSettings;
    private string SettingsFilePath;
    public Form1()
    {
        InitializeComponent();
        TurnEditPrepare();
        CreateDefaultSettings();
        LoadTurnEditSettings();
    }
    public void TurnEditPrepare()
    {
        this.Text = "New File - TurnEdit";
        this.Size = new Size(1000, 700);
        this.currentformheight = this.Size.Height;
        TurnEditGUI();
        }
    public void TurnEditGUI()
    {
        this.MainMenuStrip = new MenuStrip();
        this.MainMenuStrip.Size = new Size(800, 25);
        this.MainMenuStrip.Dock = DockStyle.Top;
        this.maintextbox = new TextBox();
        this.maintextbox.Dock = DockStyle.None;
        this.maintextbox.Location = new Point(0, this.MainMenuStrip.Size.Height);
        this.maintextbox.Multiline = true;
        this.maintextbox.Size = new Size(this.Size.Width, this.currentformheight - this.MainMenuStrip.Size.Height);
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
        ToolStripMenuItem FileSaveAsMenuItem = new ToolStripMenuItem();
        FileSaveAsMenuItem.Name = "FileSaveAsMenuItem";
        FileSaveAsMenuItem.Text = "Save As";
        FileSaveAsMenuItem.Click += new EventHandler(this.FileSaveAsMenuItem_Click);
        FileMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            FileNewMenuItem,
            FileOpenMenuItem,
            FileSaveMenuItem,
            FileSaveAsMenuItem
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
        EditReplaceMenuItem.Click += new EventHandler(this.EditReplaceMenuItem_Click);
        EditMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            EditSearchMenuItem,
            EditReplaceMenuItem
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
        /* "Help" Menu */
        ToolStripMenuItem HelpMenuItem = new ToolStripMenuItem();
        HelpMenuItem.Name = "HelpMenuItem";
        HelpMenuItem.Text = "Help";
        ToolStripMenuItem HelpAboutMenuItem = new ToolStripMenuItem();
        HelpAboutMenuItem.Name = "HelpAboutMenuItem";
        HelpAboutMenuItem.Text = "About";
        HelpAboutMenuItem.Click += new EventHandler(this.HelpAboutMenuItem_Clicked);
        HelpMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            HelpAboutMenuItem
        });
        this.MainMenuStrip.Items.AddRange(new ToolStripItem[] {
            FileMenuItem,
            EditMenuItem,
            SettingsMenuItem,
            HelpMenuItem
        });
        this.Controls.Add(MainMenuStrip);
        this.Controls.Add(this.maintextbox);
        this.SizeChanged += new EventHandler(this.Form1_SizeChangedAction);
    }
    public void Form1_SizeChangedAction(object? sender, EventArgs e)
    {
        this.currentformheight = this.Size.Height;
        this.maintextbox.Size = new Size(this.Size.Width, this.currentformheight - this.MainMenuStrip.Size.Height);
    }
    public void FileNewMenuItem_Click(object? sender, EventArgs e)
    {
        this.maintextbox.Clear();
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
            this.maintextbox.Text = OpenFileContent;
            this.Text = @$"{OpenedFileName} - TurnEdit";
            this.currentfilename = OpenedFileName;
        }
    }
    public void FileSaveAsMenuItem_Click(object? sender, EventArgs e) {
        SaveFileDialog savedialog = new SaveFileDialog();
        savedialog.ShowHelp = true;
        savedialog.Filter = "Text file (*.txt)|*.txt|All file (*.*)|*.*";
        if (savedialog.ShowDialog() == DialogResult.OK) {
            string savefilename = savedialog.FileName;
            File.WriteAllText(savefilename, this.maintextbox.Text);
            this.Text = @$"{savefilename} - TurnEdit";
            this.currentfilename = savefilename;
        }
    }
    public void EditSearchMenuItem_Click(object? sender, EventArgs e) {
        var SearchForm = new TurnEditSearchForm(this);
        SearchForm.Show();
    }
    public void EditReplaceMenuItem_Click(object? sender, EventArgs e) {
        var ReplaceForm = new TurnEditReplaceForm(this);
        ReplaceForm.Show();
    }
    public void SettingsFontMenuItem_Click(object? sender, EventArgs e) {
        FontDialog fontdlg = new FontDialog();
        fontdlg.Font = this.maintextbox.Font;
        if (fontdlg.ShowDialog() == DialogResult.OK) {
            this.maintextbox.Font = fontdlg.Font;
            this.TurnEditSettings.FontFamily = fontdlg.Font.FontFamily.Name;
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
    	SettingsJSONSyntax DeserializedJSON = System.Text.Json.JsonSerializer.Deserialize<SettingsJSONSyntax>(SettingsJSONContent);
    	this.maintextbox.Font = new Font(DeserializedJSON.FontFamily, DeserializedJSON.FontSize);
    	return true;
    	} catch (Exception ex) {
    	MessageBox.Show($@"Failed to load settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    	CreateDefaultSettings();
    	this.maintextbox.Font = new Font(this.TurnEditSettings.FontFamily, this.TurnEditSettings.FontSize);
    	return false;
    	}
    }
}
