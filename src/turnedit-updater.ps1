chcp 65001 | Out-Null

Write-Output "****************************************"
Write-Output "TurnEdit updater"
Write-Output "****************************************"

$currentappversion = [version]"1.1"
$apiurl = "https://api.github.com/repos/suzuki3932/TurnEdit/releases"
$choice = Read-Host "Do you want update TurnEdit(y/n)"
if ($choice -ceq "y") {
    Write-Output "Downloading latest version information from GitHub..."
    try {
        # FIX: Use array indexing to ensure only the first (latest) release object is selected
        $latestrelease = (Invoke-RestMethod -Uri $apiurl)[0]
    } catch {
        Write-Error "Failed to retrieve latest release information from GitHub. Error: $($_.Exception.Message)"
        Write-Output "Press any key to continue..."
        Read-Host | Out-Null
        Exit 1
    }

    if ($latestrelease) {

        Write-Output "Latest release tag: $($latestrelease.tag_name)"
        $latest_version_string = $latestrelease.tag_name

        # Attempt to clean the version string if it contains 'v' or other prefixes
        # This will remove 'v' if present, e.g., "v1.2.3" becomes "1.2.3"
        $latest_version_string = $latest_version_string -replace '^v'

        try {
            $latest_version = [version]$latest_version_string
        } catch {
            Write-Error "Failed to parse latest version string '$latest_version_string'. Error: $($_.Exception.Message)"
            Write-Output "Press any key to continue..."
            Read-Host | Out-Null
            Exit 1
        }

        Write-Output "Latest version parsed: $($latest_version.ToString())"
        Write-Output "Current application version: $($currentappversion.ToString())"

        if ($latest_version -gt $currentappversion) {
            Write-Output "New version of TurnEdit is available!"
            $assetdownloadurl = "https://github.com/suzuki3932/TurnEdit/releases/download/$($latestrelease.tag_name)/turnedit-setup.exe"
            $assetdownloaddestination = "$($env:TEMP)\turnedit-setup.exe"

            Write-Output "Downloading from: $assetdownloadurl"
            Write-Output "Saving to: $assetdownloaddestination"

            try {
                Invoke-WebRequest -Uri $assetdownloadurl -OutFile $assetdownloaddestination -ErrorAction Stop
                Write-Information "Successfully downloaded latest version from Github."
            } catch {
                Write-Error "Failed to download latest version of TurnEdit. Error: $($_.Exception.Message)"
                Write-Output "Press any key to continue..."
                Read-Host | Out-Null
                Exit 1
            }

            Write-Output "Installing..."
            if ([System.IO.File]::Exists($assetdownloaddestination)) {
                Start-Process -FilePath $assetdownloaddestination -Wait
                Write-Output "Installation complete."
            } else {
                Write-Error "The latest installer file is not found at '$assetdownloaddestination'."
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
        Write-Error "Failed to get latest release information from GitHub. The returned object was null or empty."
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