namespace TurnEdit;

public partial class Form1 : Form
{
    public TextBox? maintextbox;
    public MenuStrip? MainMenuStrip;
    public int currentformheight;
    private string? currentfilename;
    public Form1()
    {
        InitializeComponent();
        TurnEditPrepare();
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
        /* "Settings" Menu */
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
}
