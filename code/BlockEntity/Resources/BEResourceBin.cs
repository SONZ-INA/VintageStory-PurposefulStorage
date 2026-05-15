namespace PurposefulStorage;

public class BEResourceBin : BEBasePSContainer {
    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.ByBlock;

    private static readonly ExplicitTransform CachedTransformations = new (
        X: [  .1f, .22f,-.15f,  .39f,-.05f, .22f,-.2f,-.38f,-.11f,-.23f, .15f,-.19f, -.2f, .29f,  .1f,  .1f, .15f, .21f,-.23f, -.2f,  .1f, -.1f,-.21f, .35f ],
        Y: [    0, .01f, .02f,  .03f, .04f, .05f,   0, .06f,    0, .01f,    0, .05f,    0, .01f, .01f, .02f, .03f, .08f,-.01f, .15f, .15f, .14f, .15f, .14f ],
        Z: [-.05f,  .2f, .27f, -.07f, -.2f,-.33f, .1f,-.23f,-.08f,-.11f, .28f,-.35f,  .2f,    0, .05f, .32f,-.12f,-.38f, .32f,-.36f, -.1f, .05f, -.1f,-.21f ],
        
        RX: [],
        RY: [   0,  -30,    0,    90,   -5,    0,   0,   90,   30,   45,  -10,    5,    2,   90,    5,    0,   90,    0,    0,    0,    2,   25,   55,   90 ],
        RZ: []
    );

    public BEResourceBin() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck, 6, true)); }

    protected override bool TryPut(IPlayer byPlayer, ItemSlot slot, BlockSelection blockSel) {
        if (slot.Itemstack?.Item == null) // For some reason, the ore chunks and blocks have the same code
            return false;

        return base.TryPut(byPlayer, slot, blockSel);
    }

    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator) {
        if (capi == null) return false;

        base.OnTesselation(mesher, tesselator);

        ItemStack? stack = inv[0].Itemstack;
        int capacity = stack?.Collectible.MaxStackSize * 6 ?? 384;

        MeshData? utilCube = GenFillCubeMesh(capi, stack, capacity, 0.6f, ShapeReferences.utilResourceBin);
        if (utilCube != null) {
            mesher.AddMeshData(utilCube.BlockYRotation(block));
        }

        return BaseRenderContents(mesher, tesselator);
    }

    // Vanilla method for rendering items, just adjusted a little bit
    protected override bool BaseRenderContents(ITerrainMeshPool mesher, ITesselatorAPI tesselator) {
        if (tfMatrices == null) {
            updateMeshes();
        }

        for (int i = 0; i < CachedTransformations.Length; i++) {
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

        if (stack != null) {
            int capacity = stack.Collectible.MaxStackSize * 6;
            heightOffset = GetFillHeight(stack.StackSize, capacity, 0.6f);
        }

        return TransformationGenerator.GenerateExplicit(CachedTransformations, td => {
            td.offsetY += heightOffset;
        });
    }
}
