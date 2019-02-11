
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
function Set-PreDeploy() {
    <#
    .SYNOPSIS
    Short description
    
    .DESCRIPTION
    Long description
    
    .EXAMPLE
    An example
    
    .NOTES
    General notes
    #>

    [CmdletBinding()]
    
    Param(	
        [Parameter(Mandatory = $True, position = 0)]
        [string]
        $filePath
    )

    (New-Object System.Net.WebClient).Proxy.Credentials =  
    [System.Net.CredentialCache]::DefaultNetworkCredentials



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
                    Set-DoAction -conn $conn -filePath $filePath
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
}

function Set-DoAction() {
    [CmdletBinding()]
    
    Param(	
        [Parameter(Mandatory = $True, position = 0)]
        [Object]
        $conn,

        [Parameter(Mandatory = $True, position = 1)]
        [string]
        $filePath
    )

    try {
        [xml]$OrgContent = Get-Content $filePath'\Setting.xml'

        #Write-Host 'Se exportar los datos de las siguientes entidades :'

        #$OrgContent.deloitte.preimport.Data.Entities | Format-Table -AutoSize

        #Write-Host 'Se procedera a borrar los siguientes campos :'

        #$OrgContent.deloitte.preimport.Delete.Field | Format-Table -AutoSize

        #$OrgContent.deloitte.preimport.Data | %{$_.Entity} | select-object -unique

        #$counter = 0
        #$totalTimes = $OrgContent.deloitte.preimport.Data.Entities.Entity.Count

        #region Export Entity Data
        #$OrgContent.deloitte.preimport.Data.Entities.Entity | ForEach-Object {
        #   Write-Host ($counter++)$_ -ForegroundColor Yellow
        #}

        #$OrgContent.deloitte.preimport.Data.Entities.Entity | ForEach-Object {
        #    Write-Progress -Activity "Counting entities $($_)" -Status "Entity $i of $($totalTimes)" -PercentComplete (($counter / $totalTimes) * 100)  
        #    $counter++
        #}

        #endregion

        #Export Solution   
        
        Write-Host 'Se van a exportar las siguietnes soluciones :'

        $OrgContent.deloitte.preimport.Data.Solutions | Format-Table -AutoSize

        $countSolutions = $OrgContent.deloitte.preimport.Solutions.Solution.Count

        $initialCount = 0

        $OrgContent.deloitte.preimport.Solutions.Solution | ForEach-Object {
            $percent = [math]::Round(($initialCount * 100 / $countSolutions),2)

            Write-Progress -Activity 'Exporting' -Status "Processing $($_.InnerXml)" -PercentComplete $percent -CurrentOperation "$($percent)% complete"

            if (-not ([string]::IsNullOrEmpty($_.InnerXml)))
            {
                If ($_.managed -eq "true") {
                    $ZipName = "$($_.InnerXml)_managed.zip"
                    Export-CrmSolution -conn $conn -SolutionName $_.InnerXml -Managed -SolutionFilePath "F:\PROJECT\ROYAL\DEPLOY\Customizations\Solutions" -SolutionZipFileName $ZipName
                } 
                Else {
                    $ZipName = "$($_.InnerXml)_unmanaged.zip"
                    Export-CrmSolution -conn $conn -SolutionName $_.InnerXml -SolutionFilePath "F:\PROJECT\ROYAL\DEPLOY\Customizations\Solutions" -SolutionZipFileName $ZipName
                }
            }

            Write-Verbose -Message "Exported solution $($_.InnerXml)"

            $initialCount++
        }

    }
    catch [Exception] {
        Write-Error -Message $_.Exception
    }
}