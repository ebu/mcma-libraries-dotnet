using System.Text.RegularExpressions;

private static readonly string Version = File.ReadAllText("version");

private static readonly Dictionary<string, Action> Tasks = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase)
{
    [nameof(Build)] = Build,
    [nameof(SetVersion)] = SetVersion,
    [nameof(NugetPush)] = NugetPush,
    [nameof(NugetPushLocal)] = NugetPushLocal,
};

var task = Args.FirstOrDefault();
if (string.IsNullOrWhiteSpace(task))
    return Fail("Task not specified.");

if (!Tasks.ContainsKey(task))
    return Fail($"Invalid task '{task}' specified");

var now = DateTime.Now;
Console.WriteLine($"[{DateTime.Now:O}] Executing task '{task}'...");
Tasks[task]();
return Succeed($"[{DateTime.Now:O}] Task '{task}' executed successfully.");

private static void Build()
    =>
    RunDotNetCmd("build");

private static void SetVersion()
    =>
    RunForEachProject((projectName, csprojFile) => 
        File.WriteAllText(
            csprojFile,
            Regex.Replace(
                File.ReadAllText(csprojFile),
                @"\<PackageVersion\>\d+\.\d+\.\d+(?:-(?:alpha|beta|rc)\d+)?\<\/PackageVersion\>",
                $"<PackageVersion>{Version}</PackageVersion>")));

private static void NugetPush()
    =>
    RunForEachProject((projectName, csprojFile) =>
        RunDotNetCmd(
            "nuget push",
            $"bin/packages/{projectName}.{Version}.nupkg",
            "-s=https://api.nuget.org/v3/index.json",
            $"-k={Environment.GetEnvironmentVariable("NUGET_API_KEY")}"));

private static void NugetPushLocal()
    =>
    RunForEachProject((projectName, csprojFile) =>
        RunDotNetCmd(
            "nuget push",
            $"bin/packages/{projectName}.{Version}.nupkg",
            "-s=Local"));

private static int RunDotNetCmd(string cmd, params string[] args)
{
    var process = Process.Start("dotnet", cmd + " " + string.Join(" ", args));
    process.WaitForExit();
    return process.ExitCode;
}

private static int RunForEachProject(Action<string, string> run)
{
    foreach (var csprojFile in Directory.GetFiles(".", "*.csproj", SearchOption.AllDirectories))
        run(Path.GetFileNameWithoutExtension(csprojFile), csprojFile);
      
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