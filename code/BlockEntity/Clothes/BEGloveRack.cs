namespace PurposefulStorage;

public class BEGloveRack : BEBasePSContainer {
    public override string[] AttributeCheck => ["psHandware"];

    public override int[] SectionSegmentCounts => [3];

    public BEGloveRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        float[][] tfMatrices = new float[SlotCount][];
        
        for (int segment = 0; segment < SectionSegmentCounts[0]; segment++) {
            int index = segment * ItemsPerSegment;

            float x = segment * 0.3f;
            float y = -segment * 0.1f;

            tfMatrices[index] = new Matrixf()
                .Translate(0.5f, 0, 0.5f)
                .RotateYDeg(block.Shape.rotateY + 90)
                .RotateXDeg(180)
                .RotateZDeg(-70)
                .Translate(x - 0.28f, y - 0.75f, -0.5f)
                .Values;
        }

        return tfMatrices;
    }
}
