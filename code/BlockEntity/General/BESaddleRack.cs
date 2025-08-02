namespace PurposefulStorage;

public class BESaddleRack : BEBasePSContainer {
    public override string[] AttributeCheck => ["psSaddle"];

    public override int[] SectionSegmentCounts => [1];

    public BESaddleRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        float[][] tfMatrices = new float[SlotCount][];
        
        float x = 0.02f;
        float y = 0.0625f;
        float z = -0.01f;

        tfMatrices[0] = new Matrixf()
            .Translate(0.5f, 0, 0.5f)
            .RotateYDeg(block.Shape.rotateY)
            .Translate(x - 0.5f, y, z - 0.5f)
            .Values;

        return tfMatrices;
    }
}
