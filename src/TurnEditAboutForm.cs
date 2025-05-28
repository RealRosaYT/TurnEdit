namespace TurnEdit;

public partial class TurnEditAboutForm : Form
{
    private Label TurnEditAboutFormAppName;
    private Button TurnEditAboutFormOkBtn;
    private Label TurnEditAboutFormAppDescription;
    public TurnEditAboutForm() {
       this.Text = "About TurnEdit";
       this.Size = new Size(400, 350);
       this.FormBorderStyle = FormBorderStyle.FixedSingle;
       this.MaximumSize = this.Size;
       this.MinimumSize = this.Size;
       this.MaximizeBox = false;
       this.MinimizeBox = false;
       this.TurnEditAboutFormAppName = new Label();
       this.TurnEditAboutFormAppName.Font = new Font("Segoe UI", 25);
       this.TurnEditAboutFormAppName.Text = "TurnEdit";
       this.TurnEditAboutFormAppName.Dock = DockStyle.Top;
       this.TurnEditAboutFormAppName.AutoSize = true;
       this.Controls.Add(this.TurnEditAboutFormAppName);
       this.TurnEditAboutFormAppDescription = new Label();
       this.TurnEditAboutFormAppDescription.Text = "TurnEdit is an simplified text editor written in C#.\nLicensed by GNU general public license 3.0.";
       this.TurnEditAboutFormAppDescription.AutoSize = true;
       this.TurnEditAboutFormAppDescription.Dock = DockStyle.None;
       this.TurnEditAboutFormAppDescription.Location = new Point(68, 60);
       this.Controls.Add(this.TurnEditAboutFormAppDescription);
       this.TurnEditAboutFormOkBtn = new Button();
       this.TurnEditAboutFormOkBtn.Text = "OK";
       this.TurnEditAboutFormOkBtn.AutoSize = true;
       this.TurnEditAboutFormOkBtn.Visible = true;
       this.TurnEditAboutFormOkBtn.Enabled = true;
       this.TurnEditAboutFormOkBtn.Dock = DockStyle.None;
       this.TurnEditAboutFormOkBtn.Location = new Point(68, 100);
       this.TurnEditAboutFormOkBtn.Click += new EventHandler(this.TurnEditAboutFormOkBtn_Clicked);
       this.Controls.Add(this.TurnEditAboutFormOkBtn);
    }
    public void TurnEditAboutFormOkBtn_Clicked(object? sender, EventArgs e) {
        this.Close();
    }
}