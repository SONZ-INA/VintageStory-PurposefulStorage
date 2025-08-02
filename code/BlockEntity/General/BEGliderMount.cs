namespace PurposefulStorage;

public class BEGliderMount : BEBasePSContainer {
    public override string[] AttributeCheck => ["psGlider"];

    public override int[] SectionSegmentCounts => [1];

    public BEGliderMount() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator) {
        MeshData currentMesh = blockMesh.Clone();

        ItemStack[] stack = GetContentStacks();
        if (stack[0]?.Item != null) {
            MeshData substituteShape = SubstituteItemShape(Api, tesselator, ShapeReferences.GliderUnfolded);
            currentMesh.AddMeshData(substituteShape.MatrixTransform(genTransformationMatrices()[0]));
        }

        mesher.AddMeshData(currentMesh);
        return true;
    }

    protected override float[][] genTransformationMatrices() {
        float[][] tfMatrices = new float[SlotCount][];
        
        float x = -0.24f;
        float y = 1.48f;
        float z = 0.295f;

        tfMatrices[0] = new Matrixf()
            .Translate(0.5f, 0, 0.5f)
            .RotateYDeg(block.Shape.rotateY - 90)
            .Scale(1, 0.8f, 1)
            .Translate(x - 0.5f, y, z - 0.5f)
            .Values;

        return tfMatrices;
    }
}
