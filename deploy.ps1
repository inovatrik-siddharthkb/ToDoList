$projectPath = "E:\Inovatrik\ToDoList"
$publishPath = "E:\Inovatrik\ToDoList\PublishedOutput"
$remoteIP = "13.83.14.176"
$remoteUsername = "inoadmin"
$password = "Inovatrik@4321"
$remotePassword = ConvertTo-SecureString "Inovatrik@4321" -AsPlainText -Force
$remoteCred = New-Object System.Management.Automation.PSCredential ($remoteUsername, $remotePassword)
$remoteDeployPath = "D:\Apps" 

dotnet publish $projectPath -c Release -o $publishPath

$session = New-PSSession -ComputerName $remoteIP -Credential $remoteCred

Copy-Item -Path "$publishPath\*" -Destination $remoteDeployPath -Recurse -ToSession $session -Force

Invoke-Command -Session $session -ScriptBlock {
    iisreset
}

Remove-PSSession $session