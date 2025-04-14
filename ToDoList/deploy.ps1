
$sourcePath = "E:\Inovatrik\ToDoList\ToDoList\bin\Release\net9.0"
$remotePath = "C:\inetpub\MyApp"
$server = "13.83.14.176"
$username = "inoadmin"
$password = "Inovatrik@4321" 

# Using PsExec to copy files (you can use other methods as well)
Invoke-Command -ScriptBlock {
    Copy-Item -Path $using:sourcePath -Destination $using:remotePath -Recurse -Force
} -ComputerName $server -Credential (New-Object System.Management.Automation.PSCredential($username, ($password | ConvertTo-SecureString -AsPlainText -Force)))
