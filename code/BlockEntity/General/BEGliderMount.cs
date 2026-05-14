namespace PurposefulStorage;

public class BEGliderMount : BEBasePSContainer {
    public override string[] AttributeCheck => ["psGlider"];

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int[] SectionSegmentCounts => [1];

    public BEGliderMount() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator) {
        MeshData currentMesh = blockMesh!.Clone();
        ItemStack[] stacks = GetContentStacks();

        tfMatrices ??= genTransformationMatrices();
        
        for (int i = 0; i < stacks.Length; i++) {
            ItemStack stack = stacks[i];
            if (stack == null) continue;

            string shapePath = ShapeReferences.GliderUnfolded;
            string? displayedShape = stack.GetDisplayedShape();

            MeshData? substituteMesh = SubstituteItemShape(Api, tesselator, displayedShape ?? shapePath);

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
            td.scaleY = 0.8f;

            td.offsetX = -0.2975f;
            td.offsetY = 1.18f;
            td.offsetZ = -0.25f;

            td.offsetRotY = -90;
        });
    }
}
