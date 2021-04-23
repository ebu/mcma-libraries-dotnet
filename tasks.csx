using System.Text.RegularExpressions;

private static readonly string Version = File.ReadAllText("version");

private static readonly Dictionary<string, Action<string[]>> Tasks = new Dictionary<string, Action<string[]>>(StringComparer.OrdinalIgnoreCase)
{
    [nameof(SetVersion)] = SetVersion,
    [nameof(Build)] = Build,
    [nameof(Pack)] = Pack,
    [nameof(Clean)] = Clean,
    [nameof(NugetPush)] = NugetPush,
    [nameof(NugetPushLocal)] = NugetPushLocal
};

var task = Args.FirstOrDefault();
if (string.IsNullOrWhiteSpace(task))
    return Fail("Task not specified.");

if (!Tasks.ContainsKey(task))
    return Fail($"Invalid task '{task}' specified");

var now = DateTime.Now;
Console.WriteLine($"[{DateTime.Now:O}] Executing task '{task}'...");
Tasks[task](Args.Skip(1).ToArray());
return Succeed($"[{DateTime.Now:O}] Task '{task}' executed successfully.");

private static void SetVersion(params string[] args)
{
    var version = args.FirstOrDefault() ?? Version;
    RunForEachProject((projectFolder, projectName, csprojFile) => 
        File.WriteAllText(
            csprojFile,
            Regex.Replace(
                File.ReadAllText(csprojFile),
                @"\<PackageVersion\>\d+\.\d+\.\d+(?:-(?:alpha|beta|rc)\d+)?\<\/PackageVersion\>",
                $"<PackageVersion>{version}</PackageVersion>")));
                
    if (version != Version)
        File.WriteAllText("version", version);
}

private static void Build(params string[] args)
    =>
    RunDotNetCmd("build");
    
private static void Pack(params string[] args)
    =>
    RunDotNetCmd("pack");
    
private static void Clean(params string[] args)
    =>
    RunDotNetCmd("clean");

private static void NugetPush(params string[] args)
{
    if (RunDotNetCmd("pack") == 0)
        RunForEachProject((projectFolder, projectName, csprojFile) =>
            RunDotNetCmd(
                "nuget push",
                $"{projectFolder}/bin/packages/{projectName}.{Version}.nupkg",
                "-s=https://api.nuget.org/v3/index.json",
                $"-k={Environment.GetEnvironmentVariable("NUGET_API_KEY")}"));
}

private static void NugetPushLocal(params string[] args)
{
    if (RunDotNetCmd("pack") == 0)
        RunForEachProject((projectFolder, projectName, csprojFile) =>
            RunDotNetCmd(
                "nuget push",
                $"{projectFolder}/bin/packages/{projectName}.{Version}.nupkg",
                "-s=Local"));
}

private static int RunDotNetCmd(string cmd, params string[] args)
{
    var process = Process.Start("dotnet", cmd + " " + string.Join(" ", args));
    process.WaitForExit();
    return process.ExitCode;
}

private static int RunForEachProject(Action<string, string, string> run)
{
    foreach (var csprojFile in Directory.GetFiles(".", "*.csproj", SearchOption.AllDirectories))
        run(Path.GetDirectoryName(csprojFile), Path.GetFileNameWithoutExtension(csprojFile), csprojFile);
      
    return 0;
}

private static int Fail(string message)
{
    var originalColor = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Error.WriteLine(message);
    Console.ForegroundColor = originalColor;
    return -1;
}

private static int Succeed(string message)
{
    var originalColor = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(message);
    Console.ForegroundColor = originalColor;
    return 0;
}