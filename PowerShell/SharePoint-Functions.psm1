#Loading Microsoft.SharePoint.PowerShell as dependency
Add-PSSnapin "Microsoft.SharePoint.PowerShell" -PassThru -ErrorAction:SilentlyContinue

Function Restart-TimerService
{
    [CmdletBinding()]
	Param
	(
        [switch]$RestartAllInFarm
    )

    if($RestartAllInFarm)
    {
        Write-Host "Restarting all timer services in current farm..."
	    $farm = Get-SPFarm
	    $farm.TimerService.Instances | foreach {$_.Stop();$_.Start();}
    }
    else
    {
        Write-Host "Restarting timer service..."
	    Get-SPTimerJob job-timer-recycle | Start-SPTimerJob
    }
    
}

Function Get-WebPage([string]$url)
{
    $wc = new-object net.webclient;
    $wc.credentials = [System.Net.CredentialCache]::DefaultCredentials;
    $pageContents = $wc.DownloadString($url);
    $wc.Dispose();
    return $pageContents;
}

function Get-SharePointVersion
{	
	process
	{
		trap
		{
			continue
		}
	
		$ver = (Get-SPFarm).BuildVersion.Major

        if ($ver -eq 16) 
		{ 
			Write-Host "Detected SharePoint 2015 is installed" 
		}
		elseif ($ver -eq 15) 
		{ 
			Write-Host "Detected SharePoint 2013 is installed" 
		} 
		elseif ($ver -eq 14) 
		{ 
			Write-Host "Detected SharePoint 2010 is installed" 
		}
		else 
		{ 
			Write-Host "Could not determine version of SharePoint" 
		}

		$ver
	}
}

function WarmUpSharePoint
{
    # Enumerate the web app along with the site collections within it, and send a request to each one of them
    foreach ($webApp in Get-SPWebApplication -IncludeCentralAdministration)
    {
        Write-host $webApp.Url
        $html=Get-WebPage -url $webApp.Url
    }
}