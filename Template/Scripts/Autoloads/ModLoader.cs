using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using System.Xml.Linq;

namespace Template;

public partial class ModLoader
{
    public Dictionary<string, ModInfo> Mods { get; } = new();

    public void LoadMods(Node node)
    {
        Global.Services.Add(this, persistent: true);
        Logger logger = Global.Services.Get<Logger>();

        string modsPath = ProjectSettings.GlobalizePath("res://Mods");

        // Ensure "Mods" directory always exists
        Directory.CreateDirectory(modsPath);

        using DirAccess dir = DirAccess.Open(modsPath);

        if (dir == null)
        {
            logger.LogWarning("Failed to open Mods directory has it does not exist");
            return;
        }

        dir.ListDirBegin();

        string filename = dir.GetNext();

        while (filename != "")
        {
            if (!dir.CurrentIsDir())
                goto Next;

            string modRoot = $@"{modsPath}/{filename}";
            string modJson = $@"{modRoot}/mod.json";

            if (!File.Exists(modJson))
            {
                logger.LogWarning($"The mod folder '{filename}' does not have a mod.json so it will not be loaded");
                goto Next;
            }

            string jsonFileContents = File.ReadAllText(modJson);

            jsonFileContents = jsonFileContents.Replace("*", "Any");

            ModInfo modInfo = JsonSerializer.Deserialize<ModInfo>(jsonFileContents, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (Mods.ContainsKey(modInfo.Id))
            {
                logger.LogWarning($"Duplicate mod id '{modInfo.Id}' was skipped");
                goto Next;
            }

            Mods.Add(modInfo.Id, modInfo);

            // Load dll
            string dllPath = $@"{modRoot}/Mod.dll";

            if (File.Exists(dllPath))
            {
                byte[] file = File.ReadAllBytes(dllPath);
                AssemblyLoadContext context = AssemblyLoadContext.GetLoadContext(typeof(Godot.Bridge.ScriptManagerBridge).Assembly);
                Assembly assembly = context.LoadFromStream(new MemoryStream(file));
                Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(assembly);
            }

            // Load pck
            string pckPath = $@"{modRoot}/mod.pck";

            if (File.Exists(pckPath))
            {
                bool success = ProjectSettings.LoadResourcePack(pckPath, replaceFiles: true);

                if (!success)
                {
                    logger.LogWarning($"Failed to load pck file for mod '{modInfo.Name}'");
                    goto Next;
                }

                string modScenePath = $"res://{modInfo.Author}/{modInfo.Id}/mod.tscn";

                PackedScene importedScene = (PackedScene)ResourceLoader.Load(modScenePath);

                if (importedScene == null)
                {
                    logger.LogWarning($"Failed to load mod.tscn for mod '{modInfo.Name}'");
                    goto Next;
                }

                Node mod = importedScene.Instantiate<Node>();
                node.GetTree().Root.CallDeferred("add_child", mod);
            }

        Next:
            filename = dir.GetNext();
        }
    }
}

public class ModInfo
{
    public string Name { get; set; }
    public string Id { get; set; }
    public string ModVersion { get; set; }
    public string GameVersion { get; set; }
    public string Description { get; set; }
    public string Author { get; set; }
    public Dictionary<string, string> Dependencies { get; set; }
    public Dictionary<string, string> Incompatibilities { get; set; }
}
