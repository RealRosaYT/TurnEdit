namespace TurnEdit;

public partial class Form1 : Form
{
    public TextBox? maintextbox;
    public MenuStrip? MainMenuStrip;
    public int currentformheight;
    public Form1()
    {
        InitializeComponent();
        TurnEditPrepare();
    }
    public void TurnEditPrepare()
    {
        this.Text = "TurnEdit";
        this.Size = new Size(700, 900);
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
        ToolStripMenuItem FileSaveMenuItem = new ToolStripMenuItem();
        FileSaveMenuItem.Name = "FileSaveMenuItem";
        FileSaveMenuItem.Text = "Save";
        ToolStripMenuItem FileSaveAsMenuItem = new ToolStripMenuItem();
        FileSaveAsMenuItem.Name = "FileSaveAsMenuItem";
        FileSaveAsMenuItem.Text = "Save As";
        FileMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            FileNewMenuItem,
            FileOpenMenuItem,
            FileSaveMenuItem,
            FileSaveAsMenuItem
        });
        this.MainMenuStrip.Items.AddRange(new ToolStripItem[] {
            FileMenuItem
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
}
