namespace TurnEdit;

public class TurnEditSettingsManager {
	public TurnEditSettingsManager() {
		var mainformforsettings = new Form1();
		mainformforsettings.SettingsSavedDirectory = Directory.GetCurrentDirectory();
		Console.WriteLine($@"Current Directory: {mainformforsettings.SettingsSavedDirectory}");
	}
}