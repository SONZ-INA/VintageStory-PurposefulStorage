namespace PurposefulStorage;

public class BESchematicRack : BEBasePSContainer {
    public override string[] AttributeCheck => new[] { "psSchematics" };

    public override int[] SectionSegmentCounts => new[] { 1 };

    public BESchematicRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        float[][] tfMatrices = new float[SlotCount][];
        
        float x = 0;
        float y = 0.125f;
        float z = -0.57f;

        tfMatrices[0] = new Matrixf()
            .Translate(0.5f, 0, 0.5f)
            .RotateYDeg(block.Shape.rotateY)
            .RotateXDeg(60)
            .Translate(x - 0.5f, y, z - 0.5f)
            .Values;

        return tfMatrices;
    }
}
