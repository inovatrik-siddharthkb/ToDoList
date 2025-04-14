using Microsoft.Build.Utilities;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    Target Deploy => _ => _
        .Executes(() =>
        {
            var localFolder = (AbsolutePath)@"E:\Inovatrik\ToDoList\Build";
            var remoteIP = "13.83.14.176";
            var remoteUser = "inoadmin";
            var remotePass = "Inovatrik@4321";
            var remoteFolder = @"D:\Apps";

            var netUse = $"net use \\\\{remoteIP}\\D$ {remotePass} /user:{remoteUser}";
            ProcessTasks.StartProcess("cmd.exe", $"/c {netUse}").AssertZeroExitCode();

            var copyCommand = $"xcopy \"{localFolder}\\*\" \"\\\\{remoteIP}\\{remoteFolder}\" /E /I /Y";
            ProcessTasks.StartProcess("cmd.exe", $"/c {copyCommand}").AssertZeroExitCode();

            ProcessTasks.StartProcess("cmd.exe", $"/c net use \\\\{remoteIP}\\D$ /delete").AssertZeroExitCode();

        });
    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
        });

    Target Restore => _ => _
        .Executes(() =>
        {
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
        });

}
