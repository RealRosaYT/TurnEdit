namespace TurnEdit;

public class TurnEditReplaceForm : Form {
    public Form1 mainformrequirereplace;
    private TextBox ReplaceTextBx;
    private Label ReplaceTextLabel;
    private TextBox ReplaceDestinationTextBx;
    private Label ReplaceDestinationLabel;
    private Button ReplaceButton;
    public TurnEditReplaceForm(Form1 mainformrequirereplace) {
        this.mainformrequirereplace = mainformrequirereplace;
        this.Text = "Replace";
        this.Size = new Size(450, 150);
        this.MaximumSize = this.Size;
        this.MinimumSize = this.Size;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.ReplaceTextBx = new TextBox();
        this.ReplaceTextBx.Dock = DockStyle.None;
        this.ReplaceTextBx.Location = new Point(0, 15);
        this.ReplaceTextBx.Size = new Size(450, 70);
        this.Controls.Add(this.ReplaceTextBx);
        this.ReplaceTextLabel = new Label();
        this.ReplaceTextLabel.Dock = DockStyle.None;
        this.ReplaceTextLabel.AutoSize = true;
        this.ReplaceTextLabel.Text = "What to replace";
        this.ReplaceTextLabel.Location = new Point(0, 0);
        this.Controls.Add(this.ReplaceTextLabel);
        this.ReplaceDestinationTextBx = new TextBox();
        this.ReplaceDestinationTextBx.Dock = DockStyle.None;
        this.ReplaceDestinationTextBx.Location = new Point(0, 60);
        this.ReplaceDestinationTextBx.Size = new Size(450, 70);
        this.Controls.Add(this.ReplaceDestinationTextBx);
        this.ReplaceDestinationLabel = new Label();
        this.ReplaceDestinationLabel.Dock = DockStyle.None;
        this.ReplaceDestinationLabel.AutoSize = true;
        this.ReplaceDestinationLabel.Text = "Replace destination";
        this.ReplaceDestinationLabel.Location = new Point(0, 45);
        this.Controls.Add(this.ReplaceDestinationLabel);
        this.ReplaceButton = new Button();
        this.ReplaceButton.Text = "Replace";
        this.ReplaceButton.Dock = DockStyle.Bottom;
        this.ReplaceButton.AutoSize = true;
        this.ReplaceButton.Visible = true;
        this.ReplaceButton.Enabled = true;
        this.ReplaceButton.Click += new EventHandler(this.ReplaceButton_Click);
        this.Controls.Add(this.ReplaceButton);
    }
    public void ReplaceButton_Click(object? sender, EventArgs e) {
        if (this.mainformrequirereplace == null) {
            return;
        }
		if (this.mainformrequirereplace.maintextbox is null) {
			MessageBox.Show("Main text box is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}
        string textboxcontentforreplace = this.mainformrequirereplace.maintextbox.Text;
        string replacetarget = this.ReplaceTextBx.Text;
        string replacedestination = this.ReplaceDestinationTextBx.Text;
        int replacei = textboxcontentforreplace.IndexOf(replacetarget);
        if (replacei >= 0) {
            this.mainformrequirereplace.maintextbox.SelectionStart = replacei;
            this.mainformrequirereplace.maintextbox.SelectionLength = replacetarget.Length;
            this.mainformrequirereplace.maintextbox.SelectedText = replacedestination;
        } else {
            MessageBox.Show($@"{replacetarget} not found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}