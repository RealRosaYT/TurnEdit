using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace TurnEditUpdater;

public class GitHubRelease {
	[JsonPropertyName("tag_name")]
	public string? TagName {get; set;}
}

public partial class Form1 : Form
{
	private int? phase;
	private Label? currentPhase;
	private Button? btn;
	private string? tag;
	private ProgressBar? progress;
    public Form1()
    {
        InitializeComponent();
		this.MaximumSize = this.Size;
		this.MinimumSize = this.Size;
		this.MinimizeBox = false;
		this.MaximizeBox = false;
		TurnEditUpdaterGUI();
    }
	
	private async Task TurnEditUpdaterGUI() {
		this.phase = 1;
		int top = 20;
		int margin = 10;
		this.currentPhase = new Label();
		this.currentPhase.Text = "Waiting for exit TurnEdit...";
		this.currentPhase.Location = new System.Drawing.Point(20, top);
		this.currentPhase.AutoSize = true;
		this.progress = new ProgressBar();
		this.progress.Style = ProgressBarStyle.Marquee;
		this.progress.MarqueeAnimationSpeed = 30;
		top += currentPhase.Height + margin;
		progress.Location = new System.Drawing.Point(20, top);
		progress.Width = 500;
		this.btn = new Button();
		this.btn.Text = "";
		this.btn.Enabled = false;
		top += currentPhase.Height + margin;
		this.btn.Location = new System.Drawing.Point(20, top);
		this.Controls.Add(this.currentPhase);
		this.Controls.Add(this.progress);
		this.Controls.Add(this.btn);
		await WaitForExitTurnEdit();
	}
	private async Task WaitForExitTurnEdit() {
		try {
			Process[] processes = Process.GetProcessesByName("TurnEdit");
			if (processes.Length > 0) {
			foreach (var process in processes) {
				if (!process.HasExited) {
					await Task.Run(() => process.WaitForExit());
				}
			}
			this.phase = 2;
			UpdatePhaseText();
			} else {
				this.phase = 2;
				UpdatePhaseText();
			}
		} catch (Exception ex) {
			MessageBox.Show("An error occurred while waiting TurnEdit exit: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			OnErrors();
		}
	}
	private void OnErrors() {
		this.progress!.Visible = false;
		this.currentPhase!.Text = "TurnEdit update has been failed.";
		this.btn!.Enabled = true;
		this.btn.Text = "Close";
		this.btn.Click += new EventHandler(this.OnErrorBtnClick);
	}
	private void OnErrorBtnClick(object? sender, EventArgs? e) {
		Application.Exit();
	}
	private async void UpdatePhaseText() {
		if (this.phase == 2) {
			this.currentPhase!.Text = "Getting latest version information from GitHub...";
			await GetReleaseVersionFromGitHub();
		} else if (this.phase == 3) {
			this.currentPhase!.Text = "Comparing TurnEdit version...";
			CompareTurnEditVersion();
		} else if (this.phase == 4) {
			this.currentPhase!.Text = "Downloading latest version...";
			await DownloadLatestVersionOfTurnEdit(this.tag!);
		} else if (this.phase == 5) {
			this.currentPhase!.Text = "Installing latest version...";
			await InstallTurnEditLatestVersion();
		} else if (this.phase == 6) {
			this.currentPhase!.Text = "Update complete.";
			this.progress!.Visible = false;
			this.btn!.Text = "Launch TurnEdit";
			this.btn.Enabled = true;
			this.btn.Click += new EventHandler(this.btnClick);
		}
	}
	public async Task DownloadLatestVersionOfTurnEdit(string version) {
		try {
		var client = new HttpClient();
		client.DefaultRequestHeaders.UserAgent.ParseAdd("TurnEdit-Updater");
		byte[] fileBytes = await client.GetByteArrayAsync($"https://github.com/RealRosaYT/TurnEdit/releases/download/{version}/turnedit-setup.exe");
		string tempPath = System.IO.Path.GetTempPath();
		string tempTurnEditPath = System.IO.Path.Combine(tempPath, "TurnEdit");
		string tempTurnEditExePath = System.IO.Path.Combine(tempTurnEditPath, "turnedit-setup.exe");
		string ExpectedSha256 = await client.GetStringAsync($"https://github.com/RealRosaYT/TurnEdit/releases/download/{version}/turnedit-setup.exe.sha256");
		var sha256 = SHA256.Create();
		byte[] hashBytes = sha256.ComputeHash(fileBytes);
		StringBuilder sb = new StringBuilder();
		foreach (byte b in hashBytes) {
			sb.Append(b.ToString("x2"));
		}
		string calculatedSha256 = sb.ToString();
		if (!string.Equals(ExpectedSha256, calculatedSha256, StringComparison.OrdinalIgnoreCase)) {
			MessageBox.Show("Checksum verify failed. the downloaded file may be corrupted or tampered. for a security, update has been aborted.");
			OnErrors();
			return;
		}
		if (!Directory.Exists(tempTurnEditPath)) {
			Directory.CreateDirectory(tempTurnEditPath);
		}
		await File.WriteAllBytesAsync(tempTurnEditExePath, fileBytes);
		this.phase = 5;
		UpdatePhaseText();
		} catch (Exception ex) {
			MessageBox.Show("An error occurred while downloading file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			OnErrors();
		}
	}
	public string CalculateSHA256(string filePath) {
		SHA256 sha256 = SHA256.Create();
		FileStream fileStream = File.OpenRead(filePath);
		byte[] hashBytes = sha256.ComputeHash(fileStream);
		StringBuilder sb = new StringBuilder();
		foreach (byte b in hashBytes) {
			sb.Append(b.ToString("x2"));
		}
		return sb.ToString();
	}
	public async Task InstallTurnEditLatestVersion() {
		try {
			string temp = System.IO.Path.GetTempPath();
			string tempTurnEdit = System.IO.Path.Combine(temp, "TurnEdit");
			string tempTurnEditExe = System.IO.Path.Combine(tempTurnEdit, "turnedit-setup.exe");
			Process? process = Process.Start(new ProcessStartInfo{
				FileName = tempTurnEditExe,
				Arguments = "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART",
				UseShellExecute = true,
				Verb = "runas"
			});
			if (process != null) {
				await Task.Run(() => process.WaitForExit());
				int exitCode = process.ExitCode;
				if (exitCode == 1 || exitCode == 3 || exitCode == 4 || exitCode == 7) {
					throw new InvalidOperationException("Installer returned error: code: " + exitCode);
				}
				if (exitCode == 8) {
					throw new InvalidOperationException("Updater needs restart system because setup error.");
				}
				this.phase = 6;
				UpdatePhaseText();
			}
		} catch (Exception ex) {
			MessageBox.Show("An error occurred while installing TurnEdit latest version: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			OnErrors();
		}
	}
	public void CompareTurnEditVersion() {
		string? currentVersionStr = GetFileVersion();
		if (currentVersionStr != null && this.tag != null) {
			try {
			Version currentVersion = Version.Parse(currentVersionStr);
			Version latestVersion = Version.Parse(this.tag);
			if (latestVersion > currentVersion) {
				this.phase = 4;
				UpdatePhaseText();
			} else {
				this.currentPhase!.Text = "TurnEdit is up to date.";
				this.progress!.Visible = false;
				this.btn!.Text = "Launch TurnEdit";
				this.btn.Enabled = true;
				this.btn.Click += new EventHandler(this.btnClick);
			}	
			} catch (Exception ex) {
				MessageBox.Show("Error parsing version string: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				OnErrors();
			}
		}
	}
	public void btnClick(object? sender, EventArgs? e) {
		if (File.Exists("TurnEdit.exe")) {
			Process.Start("TurnEdit.exe");
		}
	}
	public string? GetFileVersion() {
		try {
			FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo("TurnEdit.exe");
			return versionInfo.FileVersion;
		} catch (FileNotFoundException) {
			MessageBox.Show("TurnEdit executable file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			OnErrors();
			return null;
		}
	}
	private async Task GetReleaseVersionFromGitHub() {
		try {
		var client = new HttpClient();
		client.DefaultRequestHeaders.UserAgent.ParseAdd("TurnEdit-Updater");
		var response = await client.GetStringAsync("https://api.github.com/repos/RealRosaYT/TurnEdit/releases/latest");
		var release = System.Text.Json.JsonSerializer.Deserialize<GitHubRelease>(response);
		if (release != null) {
			this.tag = release.TagName;
			this.phase = 3;
			UpdatePhaseText();
		}
		} catch (HttpRequestException ex) {
			MessageBox.Show("An error occurred while requesting to GitHub. here's error message:\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			OnErrors();
		}
	}
}
