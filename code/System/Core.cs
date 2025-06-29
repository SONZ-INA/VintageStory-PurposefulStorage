using System.Linq;
using static PurposefulStorage.Patches;

[assembly: ModInfo(name: "Purposeful Storage", modID: "purposefulstorage")]

namespace PurposefulStorage;

public class Core : ModSystem {
    public override double ExecuteOrder() => 1.01; // For the dynamic recipes to load, this must be after 1

    private readonly Dictionary<string, RestrictionData> restrictions = new();
    private readonly Dictionary<string, Dictionary<string, ModelTransform>> transformations = new();

    public override void Start(ICoreAPI api) {
        base.Start(api);

        // Coded Variants----------
        api.RegisterBlockClass("PurposefulStorage.BlockPSContainer", typeof(BasePSContainer));
        // ------------------------

        // Block Classes-----------
        api.RegisterBlockClass("PurposefulStorage.BlockWardrobe", typeof(BlockWardrobe));
        api.RegisterBlockClass("PurposefulStorage.BlockHatRack", typeof(BlockHatRack));

        api.RegisterBlockClass("PurposefulStorage.BlockGliderMount", typeof(BlockGliderMount));

        api.RegisterBlockClass("PurposefulStorage.BlockWeaponRack", typeof(BlockWeaponRack));
        // ------------------------

        // Block Entity Classes----
        //api.RegisterBlockEntityClass("PurposefulStorage.BEBeltHooks", typeof(BEBeltHooks));
        api.RegisterBlockEntityClass("PurposefulStorage.BEBlanketRack", typeof(BEBlanketRack));
        api.RegisterBlockEntityClass("PurposefulStorage.BEGloveRack", typeof(BEGloveRack));
        api.RegisterBlockEntityClass("PurposefulStorage.BEHatRack", typeof(BEHatRack));
        api.RegisterBlockEntityClass("PurposefulStorage.BENecklaceStand", typeof(BENecklaceStand));
        api.RegisterBlockEntityClass("PurposefulStorage.BEPantsRack", typeof(BEPantsRack));
        api.RegisterBlockEntityClass("PurposefulStorage.BEShoeRack", typeof(BEShoeRack));
        api.RegisterBlockEntityClass("PurposefulStorage.BEWardrobe", typeof(BEWardrobe));

        api.RegisterBlockEntityClass("PurposefulStorage.BEButterflyDisplayPanel", typeof(BEButterflyDisplayPanel));
        api.RegisterBlockEntityClass("PurposefulStorage.BEGearRack", typeof(BEGearRack));
        api.RegisterBlockEntityClass("PurposefulStorage.BEGliderMount", typeof(BEGliderMount));
        api.RegisterBlockEntityClass("PurposefulStorage.BESchematicRack", typeof(BESchematicRack));
        api.RegisterBlockEntityClass("PurposefulStorage.BETuningCylinderRack", typeof(BETuningCylinderRack));
        
        api.RegisterBlockEntityClass("PurposefulStorage.BEResourceBin", typeof(BEResourceBin));

        api.RegisterBlockEntityClass("PurposefulStorage.BESwordPedestal", typeof(BESwordPedestal));
        api.RegisterBlockEntityClass("PurposefulStorage.BEWeaponRack", typeof(BEWeaponRack));
        // ------------------------
    }

    public override void AssetsLoaded(ICoreAPI api) {
        base.AssetsLoaded(api);

        if (api.Side == EnumAppSide.Server) {
            RecipePatcher.SupportModdedIngredients(api);

            var restrictionGroupsServer = DiscoverRestrictionGroups(api);
            LoadData(api, restrictionGroupsServer);
        }
    }

    public override void AssetsFinalize(ICoreAPI api) {
        base.AssetsFinalize(api);

        foreach (CollectibleObject obj in api.World.Collectibles) {
            foreach (var restriction in restrictions) {
                transformations.TryGetValue(restriction.Key, out var transformation);
                PatchCollectibleWhitelist(obj, restriction, transformation);
            }
        }
    }

    #region CoreFunctions

    private Dictionary<string, string[]> DiscoverRestrictionGroups(ICoreAPI api) {
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

                if (!restrictionGroups.TryGetValue(folderName, out string[] value)) {
                    value = Array.Empty<string>();
                    restrictionGroups[folderName] = value;
                }

                var currentFiles = value.ToList();
                if (!currentFiles.Contains(fileName)) {
                    currentFiles.Add(fileName);
                    restrictionGroups[folderName] = currentFiles.ToArray();
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

    private void LoadData(ICoreAPI api, Dictionary<string, string[]> restrictionGroups) {
        foreach (var (category, names) in restrictionGroups) {
            foreach (var name in names) {
                string restrictionPath = $"purposefulstorage:config/restrictions/{category}/{name}.json".Replace("//", "/");
                string transformationPath = $"purposefulstorage:config/transformations/{category}/{name}.json".Replace("//", "/");

                restrictions[name] = api.LoadAsset<RestrictionData>(restrictionPath);

                if (api.Assets.Exists(transformationPath)) {
                    transformations[name] = api.LoadAsset<Dictionary<string, ModelTransform>>(transformationPath);
                }
            }
        }
    }

    #endregion
}
