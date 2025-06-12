namespace PurposefulStorage;

public class BEResourceBin : BEBasePSContainer {
    public override int[] SectionSegmentCounts => new[] { 1 };
    public override int ItemsPerSegment => 6;

    public BEResourceBin() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck, 64)); }

    protected override void InitMesh() {
        base.InitMesh();

        if (capi == null) return;

        MeshData contentMesh = GenPartialContentMesh(capi, GetContentStacks(), tfMatrices, 0.62f, ShapeReferences.utilResourceBin);
        if (contentMesh != null) blockMesh.AddMeshData(contentMesh);
    }

    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator) {
        mesher.AddMeshData(blockMesh);
        return true;
    }

    protected override float[][] genTransformationMatrices() {
        float[][] tfMatrices = new float[24][];
        
        float[] x = {  .1f, .22f,  .2f,  .14f, .23f, .57f, .2f, .15f, .6f,  .7f, .65f, .55f, .2f,  .5f,   .1f,   .1f,  .6f, .57f, .1f, .5f,  .1f,  .4f,  .6f, .58f };
        float[] y = {    0, .01f, .02f,  .03f, .04f, .05f,   0, .06f,   0, .01f,    0, .05f,   0, .01f,  .01f,  .02f, .03f, .04f,   0,   0, .15f, .14f, .15f, .14f };
        float[] z = {-.05f,  .4f,  .2f, -.07f, .61f,  .3f, .4f, .55f, .1f,  .4f, .62f, .65f, .7f,    0, .075f, .065f,  .6f,  .5f, .5f, .1f, -.1f, .05f,  .4f, .75f };

        float[] ry = {   0,  -30,    0,    90,   -5,    0,   0,   90,  30,   45,  -10,    5,   2,   90,     5,    -5,   90,    0,   0,   0,    2,   25,   55,   90 };

        for (int i = 0; i < tfMatrices.Length; i++) {
            tfMatrices[i] = new Matrixf()
                .Translate(0.5f, 0, 0.5f)
                .RotateYDeg(block.Shape.rotateY + ry[i])
                .Scale(0.5f, 0.5f, 0.5f)
                .Translate(x[i] * 1.8f - 1.1f, y[i] * 1.8f, z[i] * 1.8f - 1.1f)
                .Values;
        }

        return tfMatrices;
    }
}
