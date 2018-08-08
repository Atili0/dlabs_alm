
Set-StrictMode -Version Latest
$extractfolder = "C:\DELOITTE\CODE\Deloitte.Labs.ALM\Deloitte.Labs.ALM\Pkg-PreDeploy\Solutions\customizations\Sandbox"
$path = "C:\DELOITTE\CODE\Deloitte.Labs.ALM\Deloitte.Labs.ALM\Pkg-PreDeploy\Solutions\customizations\uno_managed.zip"
$cmdPath = "C:\SDK 365\SDK\Bin\SolutionPackager.exe"
$cmdArgList = @(
	"/action","Extract",
    "/zipfile","$($path)",
    "/folder","$($extractfolder)"
)

& $cmdPath $cmdArgList