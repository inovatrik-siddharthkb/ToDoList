$remoteIp = "13.83.14.176"
$username = "inoadmin"
$password = "Inovatrik@4321"
$remotePath = "D:\Apps\ToDoListApp"
$localPath = "$PSScriptRoot\PublishedOutput"  # 🧠 Dynamic path to local output folder

# Convert password to secure string
$securePassword = ConvertTo-SecureString $password -AsPlainText -Force
$cred = New-Object System.Management.Automation.PSCredential ($username, $securePassword)

# 🛠 Publish the project
dotnet publish ToDoList/ToDoList.csproj -c Release -o $localPath

Write-Host "🔗 Connecting to $remoteIp..."

Invoke-Command -ComputerName $remoteIp -Credential $cred -ScriptBlock {
    param ($remotePath)

    Write-Host "🛑 Stopping IIS..."
    Stop-Service 'W3SVC'

    Write-Host "🧹 Cleaning existing files..."
    if (Test-Path $remotePath) {
        Remove-Item -Path "$remotePath\*" -Recurse -Force -ErrorAction SilentlyContinue
    } else {
        New-Item -Path $remotePath -ItemType Directory | Out-Null
    }
} -ArgumentList $remotePath

Write-Host "📁 Copying files to $remotePath..."
Copy-Item -Path "$localPath\*" -Destination "\\$remoteIp\D$\Apps\ToDoListApp" -Recurse -Force -Credential $cred

Invoke-Command -ComputerName $remoteIp -Credential $cred -ScriptBlock {
    Write-Host "✅ Restarting IIS..."
    Start-Service "W3SVC"
    Write-Host "🚀 Deployment to D:\Apps\ToDoListApp complete!"
}
