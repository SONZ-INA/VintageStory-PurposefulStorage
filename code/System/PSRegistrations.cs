namespace PurposefulStorage;

/// <summary>
/// Handles registering all Purposeful Storage Block and BlockEntity classes.
/// </summary>
public static class PSRegistrations {
    /// <summary>
    /// Registers all Block classes for Purposeful Storage.
    /// </summary>
    public static void RegisterBlockClasses(ICoreAPI api) {
        // BaseVariants
        api.RegisterBlockClass("PurposefulStorage.BlockPSContainer", typeof(BasePSContainer));

        // Clothes
        api.RegisterBlockClass("PurposefulStorage.BlockWardrobe", typeof(BlockWardrobe));
        api.RegisterBlockClass("PurposefulStorage.BlockHatRack", typeof(BlockHatRack));

        // General
        api.RegisterBlockClass("PurposefulStorage.BlockGliderMount", typeof(BlockGliderMount));

        // Weapons
        api.RegisterBlockClass("PurposefulStorage.BlockSpearRack", typeof(BlockSpearRack));
        api.RegisterBlockClass("PurposefulStorage.BlockWeaponRack", typeof(BlockWeaponRack));
    }

    /// <summary>
    /// Registers all BlockEntity classes for Purposeful Storage.
    /// </summary>
    public static void RegisterBlockEntityClasses(ICoreAPI api) {
        // Clothes
        api.RegisterBlockEntityClass("PurposefulStorage.BEBeltHooks", typeof(BEBeltHooks));
        api.RegisterBlockEntityClass("PurposefulStorage.BEBlanketRack", typeof(BEBlanketRack));
        api.RegisterBlockEntityClass("PurposefulStorage.BEGloveRack", typeof(BEGloveRack));
        api.RegisterBlockEntityClass("PurposefulStorage.BEHatRack", typeof(BEHatRack));
        api.RegisterBlockEntityClass("PurposefulStorage.BENecklaceStand", typeof(BENecklaceStand));
        api.RegisterBlockEntityClass("PurposefulStorage.BEPantsRack", typeof(BEPantsRack));
        api.RegisterBlockEntityClass("PurposefulStorage.BEShoeRack", typeof(BEShoeRack));
        api.RegisterBlockEntityClass("PurposefulStorage.BEWardrobe", typeof(BEWardrobe));

        // General
        api.RegisterBlockEntityClass("PurposefulStorage.BEButterflyDisplayPanel", typeof(BEButterflyDisplayPanel));
        api.RegisterBlockEntityClass("PurposefulStorage.BEGearRack", typeof(BEGearRack));
        api.RegisterBlockEntityClass("PurposefulStorage.BEGliderMount", typeof(BEGliderMount));
        api.RegisterBlockEntityClass("PurposefulStorage.BEMedallionRack", typeof(BEMedallionRack));
        api.RegisterBlockEntityClass("PurposefulStorage.BESaddleRack", typeof(BESaddleRack));
        api.RegisterBlockEntityClass("PurposefulStorage.BESchematicRack", typeof(BESchematicRack));
        api.RegisterBlockEntityClass("PurposefulStorage.BETuningCylinderRack", typeof(BETuningCylinderRack));
        api.RegisterBlockEntityClass("PurposefulStorage.BETuningCylinderRack", typeof(BETuningCylinderRack));

        // Resources
        api.RegisterBlockEntityClass("PurposefulStorage.BEResourceBin", typeof(BEResourceBin));

        // Weapons
        api.RegisterBlockEntityClass("PurposefulStorage.BESpearRack", typeof(BESpearRack));
        api.RegisterBlockEntityClass("PurposefulStorage.BESwordPedestal", typeof(BESwordPedestal));
        api.RegisterBlockEntityClass("PurposefulStorage.BESwordPlaque", typeof(BESwordPlaque));
        api.RegisterBlockEntityClass("PurposefulStorage.BEWeaponRack", typeof(BEWeaponRack));
    }
}
