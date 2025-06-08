using static PurposefulStorage.Patches;

[assembly: ModInfo(name: "Purposeful Storage", modID: "purposefulstorage")]

namespace PurposefulStorage;

public class Core : ModSystem {
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
        // ------------------------

        // Block Entity Classes----
        api.RegisterBlockEntityClass("PurposefulStorage.BEHatRack", typeof(BEHatRack));
        api.RegisterBlockEntityClass("PurposefulStorage.BEWardrobe", typeof(BEWardrobe));
        // ------------------------
    }

    public override void AssetsLoaded(ICoreAPI api) {
        base.AssetsLoaded(api);

        if (api.Side == EnumAppSide.Server) {
            RecipePatcher.SupportModdedIngredients(api);

            Dictionary<string, string[]> restrictionGroupsServer = new() {
                ["clothes"] = new[] { "blankets", "footware", "headware", "upperbodyware", "waistware" },
            };

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
}
