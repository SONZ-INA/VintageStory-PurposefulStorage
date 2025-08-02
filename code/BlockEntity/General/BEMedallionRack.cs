namespace PurposefulStorage;

public class BEMedallionRack : BEBasePSContainer {
    public override string AttributeTransformCode => "onMedallionsTransform";
    public override string[] AttributeCheck => ["psMedallions"];

    public override int[] SectionSegmentCounts => [9];

    public BEMedallionRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        float[][] tfMatrices = new float[SlotCount][];

        for (int i = 0; i < SlotCount; i++) {
            float x = -0.3f + i / 3 * 0.32f;
            float y = 0.165f;
            float z = -0.3f + i % 3 * 0.32f;

            tfMatrices[i] = new Matrixf()
                .Translate(0.5f, 0, 0.5f)
                .RotateYDeg(block.Shape.rotateY + 90)
                .RotateZDeg(15)
                .Translate(x - 0.5f, y, z - 0.5f)
                .Values;
        }

        return tfMatrices;
    }
}
