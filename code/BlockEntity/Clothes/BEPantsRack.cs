namespace PurposefulStorage;

public class BEPantsRack : BEBasePSContainer {
    public override string AttributeTransformCode => "onLowerbodywareTransform";
    public override string[] AttributeCheck => ["psLowerbodyware"];

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int[] SectionSegmentCounts => [2];
    public override int ItemsPerSegment => 5;

    public BEPantsRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override bool TryPut(IPlayer byPlayer, ItemSlot slot, BlockSelection blockSel) {
        if (slot.Itemstack?.Collectible?.Code.Path.Contains("skirt") == true) // Skirts are blacklisted
            return false;

        return base.TryPut(byPlayer, slot, blockSel);
    }

    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator) {
        MeshData currentMesh = blockMesh!.Clone();
        ItemStack[] stacks = GetContentStacks();

        tfMatrices ??= genTransformationMatrices();

        for (int i = 0; i < stacks.Length; i++) {
            ItemStack stack = stacks[i];
            if (stack == null) continue;

            string shapePath = ShapeReferences.utilPants;
            string? displayedShape = stack.GetDisplayedShape();

            MeshData? substituteMesh = SubstituteItemShape(Api, tesselator, displayedShape ?? shapePath, stack);

            if (substituteMesh != null) {
                currentMesh.AddMeshData(
                    substituteMesh.MatrixTransform(tfMatrices[i])
                );
            }
        }

        mesher.AddMeshData(currentMesh);
        return true;
    }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.scaleX = 0.9f;

            td.y = td.segment * 0.42f + td.item * 0.065f;

            td.offsetX = 0.01f;
        });
    }
}
