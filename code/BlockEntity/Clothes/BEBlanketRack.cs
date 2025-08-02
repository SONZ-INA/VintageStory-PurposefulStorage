namespace PurposefulStorage;

public class BEBlanketRack : BEBasePSContainer {
    public override string[] AttributeCheck => ["psBlankets"];

    public override int[] SectionSegmentCounts => [4];

    public BEBlanketRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        float[][] tfMatrices = new float[SlotCount][];
        
        for (int segment = 0; segment < SectionSegmentCounts[0]; segment++) {
            int index = segment * ItemsPerSegment;

            float x = 0.06f;
            float y = 0.063f + segment * 0.225f;
            float z = 0.05f;

            tfMatrices[index] = new Matrixf()
                .Translate(0.5f, 0, 0.5f)
                .RotateYDeg(block.Shape.rotateY + 135)
                .Translate(x - 0.5f, y, z - 0.5f)
                .Values;
        }

        return tfMatrices;
    }
}
