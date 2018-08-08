function Import-Export-Solution
{
    ."$PSScriptRoot\Get-S2G-Config.ps1"
    $Org = Get-Config-Org

    #PREGUTNA POR LAS CREDINTIAL
    $credential = Get-Credential

    #CONNECT TO MSCRM
    $orgConnection = Get-CrmConnection -Url $Org -Credential $credential

    Write-Host "User connect : " -ForegroundColor Blue -backgroundcolor Yellow " " $credential.GetNetworkCredential().Username
   
    $content = Get-Content C:\Config_S2G\solution.txt
    $i = 0
    $content | ForEach-Object {
            Write-Host ($i++) $_ -ForegroundColor Yellow;
        }
        
    $selectedSolution = Get-Content  C:\Config_S2G\solution.txt | Select -Index $indexSolution
    $indexSolution = Read-Host 'Select a solution?' $selectedSolution
    
     
    #Get-CrmSolution -Connection $orgConnection | Format-CrmEntity
    #Export-CrmSolution -Connection $orgConnection -SolutionName "Fix20160323" -OutputPath "C:\Config_S2G\Solutions\Fix20160323.zip"

    #Write-Host "Comienza la importacion de la solucion"
    #Import-CrmSolution -Connection $orgConnection  -CustomizationPath "C:\Config_S2G\Solutions\Fix20160322_2.zip" 
}