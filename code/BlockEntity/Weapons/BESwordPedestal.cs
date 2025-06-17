namespace PurposefulStorage;

public class BESwordPedestal : BEBasePSContainer {
    public override string AttributeTransformCode => "onSwordsTransform";
    public override string[] AttributeCheck => new[] { "psSwords" };

    public override int[] SectionSegmentCounts => new[] { 1 };

    public BESwordPedestal() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        float[][] tfMatrices = new float[SlotCount][];

        tfMatrices[0] = new Matrixf()
            .Translate(0.5f, 0, 0.5f)
            .RotateYDeg(block.Shape.rotateY)
            .RotateZDeg(-90)
            .Translate(-1.4f, 0, -0.5f)
            .Values;

        return tfMatrices;
    }
}
