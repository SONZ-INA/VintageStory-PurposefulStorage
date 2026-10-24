using System.Linq;

namespace PurposefulStorage;

public static class PSDataLoader {
    /// <summary>
    /// Loads restriction and transformation data.
    /// </summary>
    public static void LoadAssets(ICoreAPI api, Dictionary<string, RestrictionData> restrictions, Dictionary<string, Dictionary<string, ModelTransform>> transformations) {
        if (api.Side == EnumAppSide.Server) {
            var restrictionGroupsServer = DiscoverRestrictionGroups(api);
            LoadData(api, restrictionGroupsServer, restrictions, transformations);
        }

        if (api.Side == EnumAppSide.Client) {
            Dictionary<string, string[]> restrictionGroupsClient = new() {
                ["resources"] = ["resourcebin"]
            };

            LoadData(api, restrictionGroupsClient, restrictions, transformations);
        }

        LoadTransformations(api, transformations);
        BEResourceBin.ResourceBinData = restrictions["resourcebin"];
    }

    /// <summary>
    /// Discovers all restriction files present in the mod.
    /// </summary>
    private static Dictionary<string, string[]> DiscoverRestrictionGroups(ICoreAPI api) {
        var restrictionGroups = new Dictionary<string, string[]>();
        string basePath = "config/restrictions/";

        var restrictionAssets = api.Assets.GetMany("config/restrictions", "purposefulstorage", false);

        foreach (var asset in restrictionAssets) {
            string fullPath = asset.Location.Path;

            string relativePath = fullPath[basePath.Length..];
            string[] pathParts = relativePath.Split('/');

            if (pathParts.Length >= 2) {
                string folderName = pathParts[0];
                string fileName = pathParts[1];

                if (fileName.EndsWith(".json")) {
                    fileName = fileName[..^5];
                }

                if (!restrictionGroups.TryGetValue(folderName, out string[]? value)) {
                    value = [];
                    restrictionGroups[folderName] = value;
                }

                var currentFiles = value.ToList();
                if (!currentFiles.Contains(fileName)) {
                    currentFiles.Add(fileName);
                    restrictionGroups[folderName] = [.. currentFiles];
                }
            }
        }

        // Remove folders that have no files
        var foldersToRemove = restrictionGroups.Where(kvp => kvp.Value.Length == 0).Select(kvp => kvp.Key).ToList();
        foreach (var folder in foldersToRemove) {
            restrictionGroups.Remove(folder);
        }

        return restrictionGroups;
    }

    /// <summary>
    /// Loads the Restriction and Transformation data from json files.
    /// </summary>
    private static void LoadData(
        ICoreAPI api,
        Dictionary<string, string[]> restrictionGroups,
        Dictionary<string, RestrictionData> restrictions,
        Dictionary<string, Dictionary<string, ModelTransform>> transformations
    ) {
        foreach (var (category, names) in restrictionGroups) {
            foreach (var name in names) {
                string restrictionPath = $"purposefulstorage:config/restrictions/{category}/{name}.json".Replace("//", "/");
                restrictions[name] = api.LoadAsset<RestrictionData>(restrictionPath);
            }
        }
    }

    /// <summary>
    /// Loads every file under config/transformations (any subfolder), keyed by file name.<br />
    /// Independent of the restrictions, so a block can have one transformation file for all item types it displays.
    /// </summary>
    private static void LoadTransformations(ICoreAPI api, Dictionary<string, Dictionary<string, ModelTransform>> transformations) {
        foreach (var asset in api.Assets.GetMany("config/transformations", "purposefulstorage", false)) {
            string path = asset.Location.Path;
            if (!path.EndsWith(".json")) continue;

            string name = path[(path.LastIndexOf('/') + 1)..^".json".Length];

            var data = api.LoadAsset<Dictionary<string, ModelTransform>>($"purposefulstorage:{path}");
            if (data == null) continue;

            // The same file name in several folders gets merged, later entries win
            if (!transformations.TryGetValue(name, out var existing)) {
                transformations[name] = data;
                continue;
            }

            foreach (var kvp in data) existing[kvp.Key] = kvp.Value;
        }
    }
}
