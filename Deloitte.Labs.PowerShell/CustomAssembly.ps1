(New-Object System.Net.WebClient).Proxy.Credentials =  
    [System.Net.CredentialCache]::DefaultNetworkCredentials

$conn = Connect-CrmOnlineDiscovery -InteractiveMode

Import-Module C:\DELOITTE\CODE\Deloitte.Labs.ALM\Deloitte.Labs.Custom\bin\Debug\Deloitte.Labs.Custom.dll -Force


$fetch = "<fetch top='50' >" +
  "<entity name='account'>" +
    "<attribute name='name'/>" +
    "<attribute name='accountnumber'/>"+
  "</entity>" +
"</fetch>";

Get-DataEntityByFetch -c $conn -e account -k "dlabs.mscrm.2018" -f $fetch

#Get-Command -module PowerShellModuleCSharp
#Get-Help Get-NetworkAdapter -Full