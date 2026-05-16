namespace PurposefulStorage;

public class BEResourceBin : BEBasePSContainer {
    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.ByBlock;

    private static readonly ExplicitTransform DefaultTransformations = new (
        X: [  .1f, .22f,-.15f,  .38f,-.05f, .22f,-.2f,-.38f,-.11f,-.23f, .15f,-.19f, -.2f, .29f,  .1f,  .1f, .15f, .21f,-.23f, -.2f,  .1f, -.1f,-.21f, .35f ],
        Y: [    0, .01f,.021f, .031f,.039f,.049f,   0, .06f,    0, .01f,    0,.051f,    0,.009f,.011f,.018f,.028f, .08f,-.01f, .15f, .15f, .14f, .15f, .14f ],
        Z: [-.05f,  .2f, .27f, -.07f, -.2f,-.33f, .1f,-.23f,-.08f,-.11f, .28f,-.35f,  .2f,    0, .05f, .32f,-.12f,-.38f, .32f,-.36f, -.1f, .05f, -.1f,-.21f ],
        
        RX: [],
        RY: [   0,  -30,    0,    90,   -5,    0,   0,   90,   30,   45,  -10,    5,    2,   90,    5,    0,   90,    0,    0,    0,    2,   25,   55,   90 ],
        RZ: []
    );

    private static readonly ExplicitTransform StickTransformations = new (
        X: [],
        Y: [    0,    0,    0,     0,    0,    0,    0, .06f, .06f, .06f, .06f, .06f, .06f, .12f, .12f, .12f, .12f, .12f, .18f, .18f, .18f ],
        Z: [-.37f,-.25f,-.13f, -.01f, .11f, .23f, .35f, -.3f,-.18f,-.06f, .06f, .18f,  .3f,-.37f,-.25f,-.13f,-.01f, .11f,-.37f,-.25f,-.13f ],
        
        RX: [],
        RY: [  47,   40,   42,    52,   44,   45,   35,   45,   41,   46,   36,   43,   45,   47,   46,  48,    43,   49,   40,   47,   42 ],
        RZ: []
    );

    private static readonly ExplicitTransform StoneTransformations = new (
        X: [ -.33f, -.11f,  .11f,  .33f, -.33f, -.11f,  .11f,  .33f, -.33f, -.11f,  .11f,  .33f, -.33f, -.11f,  .11f,  .33f, -.34f, -.17f,     0,  .17f,  .34f, -.34f, -.17f,     0,  .17f,  .34f, -.33f, -.11f, .11f, .33f, -.34f, -.17f,     0,  .17f,  .34f ],
        Y: [     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,  .08f,  .08f,  .08f,  .08f,  .08f,  .08f,  .08f,  .08f,  .08f,  .08f,  .08f,  .08f, .08f, .08f,  .16f,  .16f,  .16f,  .16f,  .16f ],
        Z: [ -.35f, -.35f, -.35f, -.35f, -.14f, -.14f, -.14f, -.14f,  .09f,  .09f,  .09f,  .09f,   .3f,   .3f,   .3f,   .3f, -.32f, -.32f, -.32f, -.32f, -.32f, -.05f, -.05f, -.05f, -.05f, -.05f,  .17f,  .17f, .17f, .17f, -.25f, -.25f, -.25f, -.25f, -.25f ],

        RX: [],
        RY: [    1,    -1,     2,     0,     1,     3,     1,    -2,     1,     0,    -3,     1,     1,     3,    -4,     1,    91,    89,    93,    90,    88,    90,    91,    92,    89,    90,     0,    -1,    1,   -3,    91,    89,    92,    88,    93 ],
        RZ: []
    );

    private static readonly ExplicitTransform NuggetTransformations = new (
        X: [ -.31f, -.14f, -.02f,  .16f,  .32f, -.28f, -.17f,  .04f,  .13f,  .29f, -.34f, -.12f, -.01f,  .18f,  .34f, -.29f, -.15f,  .02f,  .14f,  .31f, -.32f, -.18f, -.03f,  .17f,  .28f, -.26f, -.07f,  .09f,  .24f, -.23f, -.10f,  .06f,  .27f, -.27f, -.06f,  .11f,  .22f, -.24f, -.09f,  .08f,  .26f ],
        Y: [     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,     0,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f,   .1f ],
        Z: [ -.32f, -.28f, -.34f, -.29f, -.31f, -.16f, -.14f, -.18f, -.12f, -.15f, -.02f,  .04f, -.01f,  .02f, -.03f,  .14f,  .17f,  .13f,  .18f,  .16f,  .28f,  .33f,  .31f,  .29f,  .34f, -.23f, -.27f, -.24f, -.26f, -.08f, -.06f, -.11f, -.07f,  .09f,  .07f,  .12f,  .06f,  .24f,  .22f,  .27f,  .23f ],

        RX: [    1,    -1,     0,     2,    -2,     0,     1,    -1,     2,     0,    -2,     1,     0,    -1,     2,     1,     0,    -2,     1,    -1,     0,     2,    -1,     0,     1,    -1,     0,     2,    -2,     1,    -1,     0,     1,     0,     2,    -1,     0,    -2,     1,     0,     2 ],
        RY: [   15,    82,   145,   210,   330,    45,   112,   195,   260,     5,    90,   165,   220,   315,    40,   120,   185,   250,    10,    65,   170,   240,   290,    55,   135,    25,   105,   180,   275,    60,   150,   225,   305,    95,   175,   265,    15,   130,   215,   295,    80 ],
        RZ: [    0,     2,    -1,     1,     0,    -2,     0,     1,    -1,     2,     1,    -1,     0,     2,    -2,     0,     1,    -2,     0,     1,    -1,     0,     2,    -1,     0,     2,    -1,     0,     1,    -1,     0,     2,    -2,     1,    -2,     0,     1,     0,     1,    -1,     2 ]
    );

    private ExplicitTransform CachedTransformations {
        get {
            string collectibleCode = inv[0].Itemstack?.Collectible.Code ?? "";

            if (collectibleCode == "game:stick")
                return StickTransformations;

            if (WildcardUtil.Match("*:stone-*", collectibleCode))
                return StoneTransformations;

            if (WildcardUtil.Match("*:nugget-*", collectibleCode))
                return NuggetTransformations;

            return DefaultTransformations;
        }
    }

    public BEResourceBin() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck, 8, true)); }

    protected override bool TryPut(IPlayer byPlayer, ItemSlot slot, BlockSelection blockSel) {
        if (slot.Itemstack?.Item == null) // For some reason, the ore chunks and blocks have the same code
            return false;

        return base.TryPut(byPlayer, slot, blockSel);
    }

    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator) {
        if (capi == null) return false;

        base.OnTesselation(mesher, tesselator);

        if (inv[0].StackSize > CachedTransformations.Length) {
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
        if (tfMatrices == null) {
            updateMeshes();
        }

        int loopUntil = Math.Min(CachedTransformations.Length, inv[0].StackSize);
        for (int i = 0; i < loopUntil; i++) {
            ItemSlot itemSlot = inv[0];
            if (!itemSlot.Empty && tfMatrices != null && !(itemSlot.Itemstack.Collectible?.Code == null)) {
                mesher.AddMeshData(getMesh(itemSlot), tfMatrices[i]);
            }
        }

        return true;
    }

    protected override float[][] genTransformationMatrices() {
        ItemStack? stack = inv[0].Itemstack;
        float heightOffset = 0f;

        if (stack?.StackSize > CachedTransformations.Length) {
            int capacity = stack.Collectible.MaxStackSize * 8;
            heightOffset = GetFillHeight(stack.StackSize, capacity, 0.6f);
        }

        return TransformationGenerator.GenerateExplicit(CachedTransformations, td => {
            td.preRotate = block.GetRotationAngle();
            td.offsetY += heightOffset;
        });
    }
}
