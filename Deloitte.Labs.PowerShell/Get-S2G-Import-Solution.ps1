
function Show-Menu {
    param (
        [string]$Title = 'Menu Deloitte Labs'
    )
    cls
    Write-Host "================ $Title ================"
     
    Write-Host "1: Press '1' for CRM ONPRE."
    Write-Host "2: Press '2' for CRM ONLINE"
    Write-Host "Q: Press 'Q' to quit."
}

function Import-Solution
{
	[CmdletBinding()]
    
    Param(	
        [Parameter(Mandatory = $True, position = 0)]
        [string]
        $filePath,

        [Parameter(Mandatory = $True, position = 1)]
        [string]
        $SolutionPath
    )

	 try {

        do {
            Show-Menu
            $input = Read-Host "Please make a selection"
            switch ($input) {
                '1' {
                    $conn = Connect-CrmOnPremDiscovery -InteractiveMode
                } 
                '2' {
                    $conn = Connect-CrmOnlineDiscovery -InteractiveMode 
                    Set-CrmConnectionTimeout -conn $conn -TimeoutInSeconds 3600
                    Set-DoAction -conn $conn -filePath $filePath -SolutionPath $SolutionPath
                } 
                'q' {
                    return
                }
            }
            pause
        }
        until ($input -eq 'q')
    }
    catch [Exception] {
        #Write-ErrorLog -Message $_.Exception
        Write-Error -Message $_.Exception
    }

    #."$PSScriptRoot\Get-S2G-Config.ps1"
    #$Org = Get-Config-Org

    ##PREGUTNA POR LAS CREDINTIAL
    #$credential = Get-Credential

    ##CONNECT TO MSCRM
    #$orgConnection = Get-CrmConnection -Url $Org -Credential $credential

    #Write-Host "User connect : " -ForegroundColor Blue -backgroundcolor Yellow " " $credential.GetNetworkCredential().Username
   
    #$content = Get-Content C:\Config_S2G\solution.txt
    #$i = 0
    #$content | ForEach-Object {
    #        Write-Host ($i++) $_ -ForegroundColor Yellow;
    #    }
        
    #$selectedSolution = Get-Content  C:\Config_S2G\solution.txt | Select -Index $indexSolution
    #$indexSolution = Read-Host 'Select a solution?' $selectedSolution

    #Write-Host "Comienza la importacion de la solucion"
    #Import-CrmSolution -Connection $orgConnection  -CustomizationPath "C:\Config_S2G\Solutions\Fix20160322_2.zip" 
}

function Set-DoAction() {
    [CmdletBinding()]

	Param(	
        [Parameter(Mandatory = $True, position = 0)]
        [Object]
        $conn,

        [Parameter(Mandatory = $True, position = 1)]
        [string]
        $filePath,

        [Parameter(Mandatory = $True, position = 1)]
        [string]
        $SolutionPath


    )

	try{
		[xml]$OrgContent = Get-Content $filePath'\Setting.xml'

         Write-Host 'Se van a importar las siguientes soluciones :'
        
         #Write-Host $OrgContent.deloitte.SolutionPath
         
         $files = Get-ChildItem $SolutionPath

         $files | ForEach-Object {
            Write-Host ($i++) $_ -ForegroundColor Yellow;

            $confirmation = Read-Host "Confirme la impotación de estas soluciones.? [y/n]"
            while($confirmation -eq "y")
            {
                Write-Host "Comienza la importacion de la solucion"
                Import-CrmSolution -conn $conn -SolutionFilePath $_.FullName -PublishChanges $True
                $confirmation = ""
            }
        }
	}
	catch [Exception] {
        Write-Error -Message $_.Exception
    }

}

if ($loadingModule) {
	Export-ModuleMember -Function 'Import-Solution'
}
