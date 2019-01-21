#C:\Windows\System32\WindowsPowerShell\v1.0\Modules

#config powershell

<#
    Set-LoggingDefaultLevel -Level 'WARNING'
    Add-LoggingTarget -Name Console -Configuration @{}
    Add-LoggingTarget -Name File -Configuration @{Path = 'C:\Users\Atilio\Desktop\Config_S2G\Config_S2G\log_%{+%Y%m%d}.log'}
    
    Wait-Logging

$Level = 'DEBUG', 'INFO', 'WARNING', 'ERROR'
foreach ($i in 1..100) {
    Write-Log -Level ($Level | Get-Random) ('Message n.{0}' -f $i)
    Start-Sleep -Milliseconds (Get-Random -Min 100 -Max 1000)
}
hasdhasdhasdhashdashasdasd
#>

function Import-Dll {

    #SOLO LA PRIMERA VEZ
    Get-ExecutionPolicy
	#Set-ExecutionPolicy RemoteSigned
    Set-ExecutionPolicy –ExecutionPolicy RemoteSigned –Scope CurrentUser

    Install-Module Microsoft.Xrm.Data.PowerShell -Scope CurrentUser

    #Import Micrsoft.Xrm.Data.Powershell module 
    #Import-Module Microsoft.Xrm.Data.Powershell

    <#
    Install-Module ScriptLogger

    Start-ScriptLogger -Path 'C:\Users\Atilio\Desktop\Config_S2G\Config_S2G\LOG.log' -Format '{0:yyyy-MM-dd}   {0:HH:mm:ss}   {1}   {2}   {3,-11}   {4}' -Level Warning -Encoding 'UTF8' -NoEventLog -NoConsoleOutput

    Write-InformationLog -Message '----- Information Message ----'
    #>

    .$PSScriptRoot'\Get-S2G-LogConfig.ps1'
    Set-LogConfig

    #eee
}

#Import-Dll
function Get-ScriptDirectory
{
	if ($script:MyInvocation.MyCommand.Path) { Split-Path $script:MyInvocation.MyCommand.Path } else { $pwd }
}

function Show-Menu {
    param (
        [string]$Title = 'Menu Deloitte Labs'
    )
    cls
    Write-Host "================ $Title ================"
     
    Write-Host "1: Press '1' for prepare solution importation."
    Write-Host "2: Press '2' for delete entity."
    Write-Host "3: Press '3' for deploy solution process."
    Write-Host "Q: Press 'Q' to quit."
}

do {
    Show-Menu
    $input = Read-Host "Please make a selection"
    switch ($input) {
        '1' {
            .$PSScriptRoot'\Deloitte-PreDeploy.ps1'
            Set-PreDeploy -filePath 'F:\PROJECT\ROYAL\DEPLOY\Config_S2G'
        } 
        '2' {
            .$PSScriptRoot'\Get-S2G-Solution.ps1'
            Import-Export-Solution
        } 
		'3'{
			.$PSScriptRoot'\Get-S2G-Import-Solution.ps1'
			Import-Solution -filePath 'C:\DELOITTE\DEPLOY\RC\Config_S2G' -SolutionPath 'F:\PROJECT\ROYAL\DEPLOY\Customizations\Solutions'
		}
        'q' {
            return
        }
    }
    pause
}
until ($input -eq 'q')




#Write-Host $selectedOrg


#MUESTRA URL DEL MSCRM
#$deploymentServiceUrl = "http://esmadcrm02:5555/CBRE-Spain"


#QUIEN ERES
#Get-CrmWhoAmI -Connection $orgConnection

#BUSQUEDA DE UN CONTACTO
#$myFetchText = '<fetch mapping="logical"><entity name="account"><all-attributes /></entity></fetch>'
#Get-CrmEntity -Connection $orgConnection -FetchXml $myFetchText

#Get-CrmSolutionVersion -Connection $orgConnection -SolutionName "Default"

#Write-Host ("Incompatible CLR Version: {0}" -f $PSVersionTable.CLRVersion)

#Exporta contenido del CRM
#Export-CrmContent -Connection $orgConnection -OutputPath "C:\Demo ALM\mydatafolder" -Uncompressed -ExcludeMetadata true

Write-Host "Comienza la operacion"

#$entities = Get-CrmEntity -Connection $orgConnection -EntityLogicalName "account"
#$entities | Format-CrmEntity


Write-Host "Termino la importacion"