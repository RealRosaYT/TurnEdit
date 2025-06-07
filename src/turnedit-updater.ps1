chcp 65001 | Out-Null

Write-Output "****************************************"
Write-Output "TurnEdit updater"
Write-Output "****************************************"

$currentappversion = [version]"1.0"
$apiurl = "https://api.github.com/repos/suzuki3932/TurnEdit/releases"
$choice = Read-Host "Do you want update TurnEdit(y/n)"
if ($choice -ceq "y") {
	Write-Output "Downloading latest version information from GitHub..."
	$latestrelease = Invoke-RestMethod -Uri $apiurl | Select-Object -First 1 -ErrorAction Stop
	if ($latestrelease) {
		Write-Output "Latest version: $(latestrelease.tag_name)"
		$latest_version = $latestrelease.tag_name
		if ([version]$latest_version -gt [version]$currentappversion) {
			Write-Output "New version of TurnEdit is available!"
			$assetdownloadurl = "https://github.com/suzuki3932/TurnEdit/releases/download/$(latestrelease.tag_name)/turnedit-setup.exe"
			$assetdownloaddestination = "$($env:TEMP)\turnedit-setup.exe"
			try {
				Invoke-WebRequest -Uri $assetdownloadurl -OutFile $assetdownloaddestination -ErrorAction Stop
				Write-Information "Successfully downloaded latest version from Github."
			} catch {
				Write-Error "Failed to download latest version of TurnEdit."
				Write-Output "Press any key to continue..."
				Read-Host | Out-Null
				Exit 1
			}
			Write-Output "Installing..."
			if ([System.IO.File]::Exists($assetdownloaddestination)) {
				& $assetdownloaddestination
			} else {
				Write-Error "The latest installer file is not found."
				Write-Output "Press any key to continue..."
				Read-Host | Out-Null
				Exit 1
			}
		} else {
			Write-Output "TurnEdit is up to date."
			Write-Output "Press any key to continue..."
			Read-Host | Out-Null
			Exit 0
		}
	} else {
		Write-Error "Failed to get latest version information."
		Write-Output "Press any key to continue..."
		Read-Host | Out-Null
		Exit 1
	}
} elseif ($choice -ceq "n") {
	Write-Output "Canceled."
	Write-Output "Press any key to continue..."
	Read-Host | Out-Null
} else {
	Write-Error "The selection is invalid."
	Write-Output "Press any key to continue..."
	Read-Host | Out-Null
	Exit 1
}