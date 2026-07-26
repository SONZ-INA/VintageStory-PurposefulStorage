namespace PurposefulStorage;

public class BEResourceBin : BEBasePSContainer {
    public static RestrictionData ResourceBinData { get; set; } = new();

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.ByBlock;

    protected static readonly ExplicitTransform DefaultTransformations = new (
        X: [  .1f, .22f,-.15f,  .38f,-.05f, .22f,-.2f,-.38f,-.11f,-.23f, .15f,-.19f, -.2f, .29f,  .1f,  .1f, .15f, .21f,-.23f, -.2f,  .1f, -.1f,-.21f, .35f ],
        Y: [    0, .01f,.021f, .031f,.039f,.049f,   0, .06f,    0, .01f,    0,.051f,    0,.009f,.011f,.018f,.028f, .08f,-.01f, .15f, .15f, .14f, .15f, .14f ],
        Z: [-.05f,  .2f, .27f, -.07f, -.2f,-.33f, .1f,-.23f,-.08f,-.11f, .28f,-.35f,  .2f,    0, .05f, .32f,-.12f,-.38f, .32f,-.36f, -.1f, .05f, -.1f,-.21f ],
        
        RX: [],
        RY: [   0,  -30,    0,    90,   -5,    0,   0,   90,   30,   45,  -10,    5,    2,   90,    5,    0,   90,    0,    0,    0,    2,   25,   55,   90 ],
        RZ: []
    );

    protected static readonly ExplicitTransform StickTransformations = new (
        X: [],
        Y: [    0,    0,    0,     0,    0,    0,    0, .06f, .06f, .06f, .06f, .06f, .06f, .12f, .12f, .12f, .12f, .12f, .18f, .18f, .18f ],
        Z: [-.37f,-.25f,-.13f, -.01f, .11f, .23f, .35f, -.3f,-.18f,-.06f, .06f, .18f,  .3f,-.37f,-.25f,-.13f,-.01f, .11f,-.37f,-.25f,-.13f ],
        
        RX: [],
        RY: [  47,   40,   42,    52,   44,   45,   35,   45,   41,   46,   36,   43,   45,   47,   46,  48,    43,   49,   40,   47,   42 ],
        RZ: []
    );

    protected static readonly ExplicitTransform StoneTransformations = new (
        X: [ -.33f, -.11f,  .11f,  .33f, -.33f, -.11f,  .11f,  .33f, -.33f, -.11f,  .11f,  .33f, -.33f, -.11f,  .11f,  .33f, -.34f, -.17f,     0,  .17f,  .34f, -.34f, -.17f,     0,  .17f,  .34f, -.33f, -.11f, .11f, .33f, -.34f, -.17f,     0,  .17f,  .34f ],
        Y: [     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,  .08f,  .08f,  .08f,  .08f,  .08f,  .08f,  .08f,  .08f,  .08f,  .08f,  .08f,  .08f, .08f, .08f,  .16f,  .16f,  .16f,  .16f,  .16f ],
        Z: [ -.35f, -.35f, -.35f, -.35f, -.14f, -.14f, -.14f, -.14f,  .09f,  .09f,  .09f,  .09f,   .3f,   .3f,   .3f,   .3f, -.32f, -.32f, -.32f, -.32f, -.32f, -.05f, -.05f, -.05f, -.05f, -.05f,  .17f,  .17f, .17f, .17f, -.25f, -.25f, -.25f, -.25f, -.25f ],

        RX: [],
        RY: [    1,    -1,     2,     0,     1,     3,     1,    -2,     1,     0,    -3,     1,     1,     3,    -4,     1,    91,    89,    93,    90,    88,    90,    91,    92,    89,    90,     0,    -1,    1,   -3,    91,    89,    92,    88,    93 ],
        RZ: []
    );

    protected static readonly ExplicitTransform NuggetTransformations = new (
        X: [ -.35f, -.16f, -.04f,  .16f,  .32f, -.34f, -.19f,  .04f,  .13f,  .29f, -.36f, -.14f, -.03f,  .18f,  .34f, -.35f, -.17f,  .02f,  .14f,  .31f, -.36f, -.20f, -.05f,  .17f,  .28f, -.32f, -.11f,  .14f,  .32f, -.33f, -.15f,  .09f,  .32f, -.30f, -.09f,  .13f,  .25f, -.25f, -.10f,  .10f,  .27f, -.34f, -.21f, -.10f,     0,  .09f,  .19f,  .34f, -.30f, -.09f,  .04f,  .17f,  .32f, -.14f,    0, .14f ],
        Y: [     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .2f,   .2f,   .2f,   .2f,   .2f,   .2f,   .2f,   .2f,   .2f,   .2f,   .2f,   .2f,   .2f, .16f,  .2f ],
        Z: [ -.35f, -.30f, -.35f, -.31f, -.33f, -.18f, -.16f, -.20f, -.14f, -.17f, -.04f,  .03f, -.03f,  .02f, -.05f,  .13f,  .17f,  .13f,  .18f,  .16f,  .28f,  .33f,  .31f,  .29f,  .34f, -.28f, -.31f, -.28f, -.30f, -.11f, -.08f, -.13f, -.10f,  .07f,  .05f,  .10f,  .06f,  .24f,  .21f,  .27f,  .23f, -.35f, -.34f, -.35f, -.33f, -.34f, -.35f, -.32f, -.17f, -.17f, -.17f, -.17f, -.17f,     0,    0,    0 ],

        RX: [    1,    -1,     0,     2,    -2,     0,     1,    -1,     2,     0,    -2,     1,     0,    -1,     2,     1,     0,    -2,     1,    -1,     0,     2,    -1,     0,     1,    -1,     0,     2,    -2,     1,    -1,     0,     1,     0,     2,    -1,     0,    -2,     1,     0,     2,     1,    -2,     0,     2,    -1,     1,     0,     0,     2,    -1,     1,    -2,     1,    2,    0 ],
        RY: [   15,    82,   145,   210,   330,    45,   112,   195,   260,     5,    90,   165,   220,   315,    40,   120,   185,   250,    10,    65,   170,   240,   290,    55,   135,    25,   105,   180,   275,    60,   150,   225,   305,    95,   175,   265,    15,   130,   215,   295,    80,    35,   118,   201,   287,    64,   153,   332,    48,   137,   226,   314,    96,    90,   45,    2 ],
        RZ: [    4,     7,    -1,    -2,     6,    -2,     7,     5,    -1,    11,    -8,    -5,     9,     4,    -6,    -7,     3,    -2,     9,     1,    -9,     4,     2,    -4,     6,     2,    -5,     0,     3,    -9,     8,     7,    -6,     5,    -4,     8,     1,     9,     7,    -1,     7,    -7,     9,     0,     4,    -5,     7,     8,     1,   -12,     6,   -10,    -7,     7,   10,   -5 ]
    );

    protected static readonly ExplicitTransform FlaxTransformations = new (
        X: [  -.2f,   .2f,  -.2f,   .2f,  -.2f,   .2f,  -.2f,   .2f, -.2f,  .2f,     0,     0,     0,    0,   0, -.37f, -.37f,  .37f, .37f, -.22f,  .22f ],
        Y: [     0,     0,     0,     0,     0,     0,     0,     0,    0,    0,   .1f,   .1f,   .1f,  .1f, .1f,  .11f,   .1f,   .1f, .11f,   .2f,   .2f ],
        Z: [ -.33f, -.33f, -.17f, -.17f, -.01f, -.01f,  .15f,  .15f,  .3f,  .3f, -.33f, -.17f, -.01f, .15f, .3f, -.22f,  .18f, -.22f, .18f,  -.2f,  -.2f ],

        RX: [],
        RY: [    1,    -1,     2,     0,     1,     3,     1,    -2,  180,  180,      2,    1,    -1,    3, 180,    90,    90,   -90,  -90,    81,   -77 ],
        RZ: []
    );

    protected static readonly ExplicitTransform FiberTransformations = new (
        X: [  -.2f,   .2f,  -.2f,   .2f,  -.2f,   .2f,  -.2f,   .2f, -.2f,  .2f,     0,     0,     0,    0,    0, -.34f, -.34f,  .34f, .34f, -.22f,  .22f, -.22f,  .22f, -.22f, .22f, -.22f, .22f, -.22f,  .22f, -.22f,  .22f, -.22f, .22f, -.22f, .22f ],
        Y: [     0,     0,     0,     0,     0,     0,     0,     0,    0,    0,  .05f,  .05f,  .05f, .05f, .05f,  .06f,  .05f,  .05f, .06f,   .1f,   .1f,   .1f,   .1f,   .1f,  .1f,   .1f,  .1f,  .15f,  .15f,  .15f,  .15f,  .15f, .15f,   .2f,  .2f ],
        Z: [ -.33f, -.33f, -.17f, -.17f, -.01f, -.01f,  .15f,  .15f,  .3f,  .3f, -.33f, -.17f, -.01f, .15f,  .3f, -.22f,  .18f, -.22f, .18f, -.34f, -.34f, -.18f, -.18f,     0,    0,  .18f, .18f, -.31f, -.31f, -.14f, -.14f,  .04f, .04f,  -.3f, -.3f ],

        RX: [    0,     1,     2,     1,    -1,     2,    -1,     3,   -1,    1,     0,    -2,     1,   -2,    3,     3,    -1,     1,    2,    -1,     0,     0,     1,    -2,    3,     1,   -2,    -1,    -2,     3,     2,     0,    1,     1,    2 ],
        RY: [    1,    -1,     2,     0,     1,     3,     1,    -2,  180,  180,     2,     1,    -1,    3,  180,   105,   105,  -110, -110,     1,    -3,     2,     0,     1,   -1,    -2,    0,    -1,     2,    -3,    -2,     0,    1,    10,   -5 ],
        RZ: [    0,     1,     2,    -1,     0,    -1,     3,     2,    1,    0,    -1,     1,     2,   -2,    3,     1,    -2,     1,    2,     0,     1,     0,     1,    -1,    0,     1,    2,     1,    -2,     0,     1,    -1,    2,     2,    0 ]
    );

    protected static readonly ExplicitTransform PeatTransformations = new (
        X: [ -.22f,  .22f, -.22f,  .22f, -.22f,  .22f, -.22f,  .22f, -.22f, .22f, -.219f, .219f ],
        Y: [  .22f,  .22f,  .22f,  .22f,  .22f,  .22f,  .22f,  .22f,  .17f, .17f,   .33f,  .33f ],
        Z: [ -.43f, -.43f, -.30f, -.30f, -.17f, -.17f, -.04f, -.04f,  .17f, .17f,  -.29f, -.29f ],

        RX: [   90,    90,    90,    90,    90,    90,    90,    90,    50,   50,      0,     0 ],
        RY: [   90,    90,    90,    90,    90,    90,    90,    90,    90,   90,     90,    90 ],
        RZ: [    0,     1,     2,    -1,     0,    -1,     3,     2,     1,    0,     -1,     1 ]
    );

    protected ExplicitTransform? CachedTransformations {
        get {
            if (inv[0].Empty || ResourceBinData?.GroupingCodes == null)
                return DefaultTransformations;

            string collectibleCode = inv[0].Itemstack?.Collectible.Code ?? "";

            foreach (var group in ResourceBinData.GroupingCodes) {
                foreach (var pattern in group.Value) {
                    if (WildcardUtil.Match(pattern, collectibleCode)) {
                        return group.Key switch {
                            "stick" => StickTransformations,
                            "stone" => StoneTransformations,
                            "nugget" => NuggetTransformations,
                            "flax" => FlaxTransformations,
                            "fiber" => FiberTransformations,
                            "peat" => PeatTransformations,
                            "powder" => null, // Returning null means powder rendering logic
                            _ => DefaultTransformations
                        };
                    }
                }
            }

            return DefaultTransformations;
        }
    }

    public BEResourceBin() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck, 8, true)); }

    protected override bool TryPut(IPlayer byPlayer, ItemSlot slot, BlockSelection blockSel) {
        // For some reason, the ore chunks and blocks have the same code
        // And for some reason, peatbrick is categorized as a block...
        if (slot.Itemstack?.Item == null && slot.Itemstack?.Collectible?.Code != "game:peatbrick") 
            return false;

        return base.TryPut(byPlayer, slot, blockSel);
    }

    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator) {
        if (capi == null) return false;

        mesher.AddMeshData(blockMesh);

        var cachedTransforms = CachedTransformations;

        if (cachedTransforms == null) {
            MeshData? powderMesh = GenLiquidyMesh(capi, inv[0], ShapeReferences.utilResourceBinPowder, 10.2f, true);
            if (powderMesh != null) {
                mesher.AddMeshData(powderMesh.BlockYRotation(block));
            }

            return true;
        }

        ExplicitTransform transforms = cachedTransforms.Value;

        if (inv[0].StackSize > transforms.Length) {
            ItemStack stack = inv[0].Itemstack!;
            int capacity = stack.Collectible.MaxStackSize * 8;

            MeshData? utilCube = GenFillCubeMesh(capi, stack, capacity, 0.6f, ShapeReferences.utilResourceBin);
            if (utilCube != null) {
                mesher.AddMeshData(utilCube.BlockYRotation(block));
            }
        }

        return BaseRenderContents(mesher, tesselator);
    }

    // Vanilla method for rendering items, just adjusted a little bit
    protected override bool BaseRenderContents(ITerrainMeshPool mesher, ITesselatorAPI tesselator) {
        var cachedTransforms = CachedTransformations;
        if (cachedTransforms == null) return true;

        ExplicitTransform transforms = cachedTransforms.Value;

        if (tfMatrices == null) {
            updateMeshes();
        }

        int loopUntil = Math.Min(transforms.Length, inv[0].StackSize);
        for (int i = 0; i < loopUntil; i++) {
            ItemSlot itemSlot = inv[0];
            if (!itemSlot.Empty && tfMatrices != null && !(itemSlot.Itemstack.Collectible?.Code == null)) {
                mesher.AddMeshData(getMesh(itemSlot), tfMatrices[i]);
            }
        }

        return true;
    }

    protected override float[][] genTransformationMatrices() {
        var cachedTransforms = CachedTransformations;
        if (cachedTransforms == null) return [];

        ExplicitTransform transforms = cachedTransforms.Value;

        ItemStack? stack = inv[0].Itemstack;
        float heightOffset = 0f;

        if (stack?.StackSize > transforms.Length) {
            int capacity = stack.Collectible.MaxStackSize * 8;
            heightOffset = GetFillHeight(stack.StackSize, capacity, 0.6f);
        }

        return TransformationGenerator.GenerateExplicit(transforms, td => {
            td.preRotate = block.GetRotationAngle();
            td.offsetY += heightOffset;
        });
    }
}
