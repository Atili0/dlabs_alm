function Get-ConfigOrg {
    <#
    
#>

    [CmdletBinding()]
    
    Param(	
        [Parameter(Mandatory = $True, position = 0)]
        [string]
        $filePath
    )
    
    try {
        [xml]$OrgContent = Get-Content $filePath'\Organization.xml'

        Write-Host 'Select a organization to connect'

        $i = 0
        Select-Xml -Xml $OrgContent -XPath "//Org" | ForEach-Object {
            Write-Host ($i++) $_.node.Code '|' $_.node.serverType  '->'$_.Node.Url -ForegroundColor DarkCyan
        }

        $OrgSelected = Read-Host 'Seleccione la organizacion?'

        $XmlResult = $OrgContent.selectNodes("Organizations/Org[@Code='$OrgSelected']")

        #Write-Log -Level 'DEBUG' -Message 'Xml selected was'

        Write-InformationLog -Message 'Xml selected was ' $XmlResult.OuterXml

        #Write-Host $XmlResult.serverType
    }
    catch [Exception] {
        Write-ErrorLog -Message $_.Exception
    }
}
