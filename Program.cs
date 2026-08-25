using UninstallTool;
using System.IO;

var log = new OperationLog();
var inventory = new AppInventory(log);

var apps = inventory.GetInstalledApps();

var targets = new[] { "Unreal", "JetBrains", "Arduino", "Git", "OneDrive", "Android", "Fusion", "Minecraft" };

Console.WriteLine("=== 検出済みアプリ一覧に含まれるか確認 ===\n");
foreach (var target in targets)
{
    var matches = apps.Where(a => a.DisplayName.Contains(target, StringComparison.OrdinalIgnoreCase)).ToList();
    Console.WriteLine($"[{target}] {matches.Count}件");
    foreach (var m in matches)
    {
        Console.WriteLine($"  - {m.DisplayName} | InstallLocation: {m.InstallLocation}");
    }
}
