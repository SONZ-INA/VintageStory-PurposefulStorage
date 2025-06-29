namespace PurposefulStorage;

public class BEButterflyDisplayPanel : BEBasePSContainer {
    public override string[] AttributeCheck => new[] { "psButterflies" };

    public override int[] SectionSegmentCounts => new[] { 4 };

    public BEButterflyDisplayPanel() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        float[][] tfMatrices = new float[SlotCount][];
        
        for (int i = 0; i < SlotCount; i++) {
            float x = -0.4f - i / 2 * 0.8f;
            float y = -0.775f;
            float z = 0.375f - i % 2 * 0.8f;

            tfMatrices[i] = new Matrixf()
                .Translate(0.5f, 0, 0.5f)
                .RotateYDeg(block.Shape.rotateY - 90)
                .RotateZDeg(-90)
                .Scale(0.6f, 0.6f, 0.6f)
                .Translate(x - 0.5f, y, z - 0.5f)
                .Values;
        }

        return tfMatrices;
    }
}
