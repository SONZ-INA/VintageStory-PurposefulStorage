[assembly: ModInfo(name: "Purposeful Storage", modID: "purposefulstorage")]

namespace PurposefulStorage;

public class Core : ModSystem {
    private readonly Dictionary<string, RestrictionData> restrictions = [];
    private readonly Dictionary<string, Dictionary<string, ModelTransform>> transformations = [];

    public override void Start(ICoreAPI api) {
        base.Start(api);

        PSRegistrations.RegisterBlockClasses(api);
        PSRegistrations.RegisterBlockEntityClasses(api);
    }

    public override void AssetsLoaded(ICoreAPI api) {
        base.AssetsLoaded(api);

        PSDataLoader.LoadAssets(api, restrictions, transformations);
    }

    public override void AssetsFinalize(ICoreAPI api) {
        base.AssetsFinalize(api);

        PSDataFinalizer.FinalizeAssets(api, restrictions, transformations);
    }
}
