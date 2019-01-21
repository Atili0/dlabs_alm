#Install-Module Microsoft.Xrm.Data.PowerShell -Scope CurrentUser -Force
#Install-Module -Name newtonsoft.json

<#
(New-Object System.Net.WebClient).Proxy.Credentials =  
    [System.Net.CredentialCache]::DefaultNetworkCredentials

$conn = Connect-CrmOnlineDiscovery -InteractiveMode

Import-Module C:\Users\Atilio\Desktop\PowerShellModuleCSharp\PowerShellModuleCSharp\bin\Debug\PowerShellModuleCSharp.dll -Force
#>

#Get-Command -module PowerShellModuleCSharp
#Get-Help Get-NetworkAdapter -Full

#$conn | ConvertTo-Json -Compress | Out-File 'C:\Users\Atilio\Desktop\Config_S2G\connection.json'

#Get-NetworkAdapter -Connection $conn


$extractfolder = "C:\CODE\SVN\dlabs_alm\Deloitte.Labs.PowerShell\PreDeploy\Solutions"
$Command = "C:\SDK 365\SDK\Bin\SolutionPackager.exe"
$Parms = "/action:Extract /zipfile:$($path) /folder:$($extractfolder)"

$path = "C:\CODE\SVN\dlabs_alm\Deloitte.Labs.PowerShell\PreDeploy\Solutions\customizations\uno_managed.zip"


$Prms = $Parms.Split()
& "$Command" $Prms


