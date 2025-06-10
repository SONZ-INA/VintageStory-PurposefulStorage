namespace PurposefulStorage;

public class BEBeltHooks : BEBasePSContainer {
    public override string[] AttributeCheck => new[] { "psWaistware" };

    public override int[] SectionSegmentCounts => new[] { 4 };
    public override int ItemsPerSegment => 3;

    public BEBeltHooks() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        float[][] tfMatrices = new float[SlotCount][];
        
        for (int segment = 0; segment < SectionSegmentCounts[0]; segment++) {
            for (int item = 0; item < ItemsPerSegment; item++) {
                int index = segment * ItemsPerSegment + item;

                float x = -0.05f + segment % 2 * 0.335f + segment / 2 * 0.35f;
                float y = -1.43f + segment / 4 * 0.225f + item * 0.125f;
                float z = -0.27f + segment % 2 * 0.335f + segment / 2 * -0.35f;

                tfMatrices[index] = new Matrixf()
                    .Translate(0.5f, 0, 0.5f)
                    .RotateYDeg(block.Shape.rotateY)
                    .RotateXDeg(90)
                    .RotateYDeg(45)
                    .Translate(x - 0.5f, y, z - 0.5f)
                    .Values;
            }
        }

        return tfMatrices;
    }
}
