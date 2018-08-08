function Set-LogConfig() {

    Get-ExecutionPolicy
    Set-ExecutionPolicy –ExecutionPolicy RemoteSigned –Scope CurrentUser
    Install-Module ScriptLogger

    Start-ScriptLogger -Path 'C:\Users\Atilio\Desktop\Config_S2G\Config_S2G\LOG.log' -Format '{0:yyyy-MM-dd}   {0:HH:mm:ss}   {1}   {2}   {3,-11}   {4}' -Level Warning -Encoding 'UTF8' -NoEventLog -NoConsoleOutput
    Set-ScriptLogger -Level Verbose
    Set-ScriptLogger -Level Information
    Set-ScriptLogger -Level Error

    Write-InformationLog -Message '----- Information Message ----'

    <#
    # Log an error message
    Write-ErrorLog -Message 'My Error Message'

    # Log a warning massage
    Write-WarningLog -Message 'My Warning Message'

    # Log an information message
    

    # Log a verbose message
    Write-VerboseLog -Message 'My Verbose Message'
    #>
}