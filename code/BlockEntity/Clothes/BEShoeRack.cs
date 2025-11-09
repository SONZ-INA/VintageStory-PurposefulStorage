namespace PurposefulStorage;

public class BEShoeRack : BEBasePSContainer {
    public override string AttributeTransformCode => "onFootwareTransform";
    public override string[] AttributeCheck => ["psFootware"];

    public override int[] SectionSegmentCounts => [4];

    public BEShoeRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        float[][] tfMatrices = new float[SlotCount][];
        
        for (int segment = 0; segment < SectionSegmentCounts[0]; segment++) {
            int index = segment * ItemsPerSegment;

            float x = -0.275f + segment / 2 * 0.5f;
            float y = 0.063f + segment / 2 * 0.24f;
            float z = -0.265f + segment % 2 * 0.535f;

            tfMatrices[index] = new Matrixf()
                .Translate(0.5f, 0, 0.5f)
                .RotateYDeg(block.Shape.rotateY + 90)
                .Scale(.85f, .85f, .85f)
                .Translate(x - 0.5f, y, z - 0.5f)
                .Values;
        }

        return tfMatrices;
    }
}
