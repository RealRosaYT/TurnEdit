namespace TurnEdit;

public class TurnEditSearchForm : Form {
    private TextBox searchtextbox;
    private Button searchbutton;
    private Form1 mainform;
    public TurnEditSearchForm(Form1 mainform) {
        this.mainform = mainform;
        this.Text = "Search";
        this.Size = new Size(450, 100);
        this.MaximumSize = this.Size;
        this.MinimumSize = this.Size;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.searchtextbox = new TextBox();
        this.searchtextbox.Dock = DockStyle.Top;
        this.searchtextbox.Size = new Size(450, 70);
        this.Controls.Add(this.searchtextbox);
        this.searchbutton = new Button();
        this.searchbutton.Text = "Search";
        this.searchbutton.Visible = true;
        this.searchbutton.Enabled = true;
        this.searchbutton.AutoSize = true;
        this.searchbutton.Dock = DockStyle.Bottom;
        this.searchbutton.Click += new EventHandler(this.searchbutton_Clicked);
        this.Controls.Add(this.searchbutton);
    }
    public void searchbutton_Clicked(object? sender, EventArgs e) {
        // mainformがnullの場合は何もしない
        if (this.mainform == null) return;

        // mainformのmaintextboxからテキストを取得
        string textboxcontent = this.mainform.maintextbox.Text;
        string searchtarget = this.searchtextbox.Text;

        int searchi = textboxcontent.IndexOf(searchtarget);
        if (searchi >= 0) {
            this.mainform.maintextbox.SelectionStart = searchi;
            this.mainform.maintextbox.SelectionLength = searchtarget.Length;
            this.mainform.maintextbox.Focus();
        } else {
            MessageBox.Show($@"{searchtarget} not found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}