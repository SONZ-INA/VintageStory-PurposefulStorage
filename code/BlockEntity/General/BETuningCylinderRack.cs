namespace PurposefulStorage;

public class BETuningCylinderRack : BEBasePSContainer {
    public override string[] AttributeCheck => new[] { "psTuningCylinders" };

    public override int[] SectionSegmentCounts => new[] { 8 };

    public BETuningCylinderRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        float[][] tfMatrices = new float[SlotCount][];

        for (int segment = 0; segment < SectionSegmentCounts[0]; segment++) {
            if (block.Variant["type"] == "stand") {
                float x = -0.375f + segment % 4 * 0.25f;
                float y = -segment / 4 * 0.47f;
                float z = -0.3f - segment / 4 * 0.2825f;

                tfMatrices[segment] = new Matrixf()
                    .Translate(0.5f, 0, 0.5f)
                    .RotateYDeg(block.Shape.rotateY)
                    .RotateXDeg(90)
                    .Translate(x - 0.5f, y, z - 0.5f)
                    .Values;
            }
            else {
                float x = -0.375f + segment % 4 * 0.25f;
                float y = -0.12f + segment / 4 * 0.4f;
                float z = -0.345f - segment / 4 * 0.22f;

                tfMatrices[segment] = new Matrixf()
                    .Translate(0.5f, 0, 0.5f)
                    .RotateYDeg(block.Shape.rotateY)
                    .RotateXDeg(30)
                    .Translate(x - 0.5f, y, z - 0.5f)
                    .Values;
            }
        }

        return tfMatrices;
    }
}
