# Try to pull and fast forward
git pull --ff-only
if ($? -eq $True) {
	Write-Host -ForegroundColor Green "PullAll successful."
	Exit 0
}

# Fast forward not possible, start rebase dialog of TortoiseGit
Write-Warning "There are local changes."
Write-Warning "Rebase local changes on server commits..."
$process = (Start-Process TortoiseGitProc -ArgumentList "/command:rebase" -PassThru -Wait)

# Report result back to user
switch ($process.ExitCode)
{
	0 { Write-Host -ForegroundColor Green "Rebase successful." }
	-1 { Write-Host -ForegroundColor Red "ERROR: Rebase local changes on server commits FAILED -- ABORTED" }
	default  { Write-Host -ForegroundColor Red "ERROR: Rebase local changes on server commits FAILED -- UNKNOWN ($($process.ExitCode))" }
}
