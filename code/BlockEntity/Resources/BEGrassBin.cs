namespace PurposefulStorage;

public class BEGrassBin : BEBasePSContainer {
    public override int[] SectionSegmentCounts => new[] { 1 };
    public override int ItemsPerSegment => 6;

    public BEGrassBin() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck, 64)); }

    protected override void InitMesh() {
        base.InitMesh();

        if (capi == null) return;

        MeshData contentMesh = GenPartialContentMesh(capi, GetContentStacks(), ShapeReferences.utilGrassBin, tfMatrices, 0.5f);
        if (contentMesh != null) blockMesh.AddMeshData(contentMesh);
    }

    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator) {
        mesher.AddMeshData(blockMesh);
        return true;
    }

    protected override float[][] genTransformationMatrices() {
        float[][] tfMatrices = new float[10][];
        
        float[] x = { .1f, .22f,  .2f,   .1f, .23f, .15f, .2f,  .12f, .05f,    0 };
        float[] y = {   0, .01f, .02f,  .03f, .04f, .05f,   0,  .01f, .25f, .26f };
        float[] z = {-.1f,  .4f,  .2f, -.12f, .61f,    0, .4f,  .55f, -.1f, .25f };

        float[] ry = {  0,  -30,    0,    90,   -5,    0,   0,    90,    0,   45 };

        for (int i = 0; i < tfMatrices.Length; i++) {
            tfMatrices[i] = new Matrixf()
                .Translate(0.5f, 0, 0.5f)
                .RotateYDeg(block.Shape.rotateY + ry[i])
                .Scale(0.8f, 0.8f, 0.8f)
                .Translate(x[i] - 0.62f, y[i], z[i] - 0.8f)
                .Values;
        }

        return tfMatrices;
    }
}
