namespace TurnEdit;

public class TurnEditTextCounter : Form {
    private Form1 _mainForm;
    private Label TurnEditTextCounterLabel;

    public TurnEditTextCounter(Form1 mainForm) {
        _mainForm = mainForm;

        this.Text = "Count text";
        this.Size = new Size(200, 100);
        this.MaximumSize = this.Size;
        this.MinimumSize = this.Size;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        this.TurnEditTextCounterLabel = new Label();
        this.TurnEditTextCounterLabel.AutoSize = true;
        this.TurnEditTextCounterLabel.Dock = DockStyle.Top;
        this.Controls.Add(this.TurnEditTextCounterLabel);
        UpdateCounter(); 
    }

    public void UpdateCounter() {
        if (_mainForm?.maintextbox != null && TurnEditTextCounterLabel != null) {
            var TextBoxTextCountReal = _mainForm.maintextbox.Text.Length;
            this.TurnEditTextCounterLabel.Text = "Count (Includes return): " + TextBoxTextCountReal;
        } else {
            if (TurnEditTextCounterLabel != null)
            {
                this.TurnEditTextCounterLabel.Text = "Count: 0 (Error)";
            }
        }
    }
}