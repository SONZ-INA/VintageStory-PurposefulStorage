namespace PurposefulStorage;

public class BEGearRack : BEBasePSContainer {
    public override string AttributeTransformCode => "onGearsTransform";
    public override string[] AttributeCheck => new[] { "psGears" };

    public override int[] SectionSegmentCounts => new[] { 2 };
    public override int ItemsPerSegment => 11;

    public BEGearRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        float[][] tfMatrices = new float[SlotCount][];
        
        for (int segment = 0; segment < SectionSegmentCounts[0]; segment++) {
            for (int item = 0; item < ItemsPerSegment; item++) {
                int index = segment * ItemsPerSegment + item;

                float x = -0.11f + segment * 0.5275f;
                float y = 0.05f + item * 0.08f;
                float z = -0.2f;

                float rx, rz;
                switch (block.Variant["side"]) {
                    case "east": rx = .75f; rz = .25f + segment * .5f; break;
                    case "north": rx = .25f + segment * .5f; rz = .25f; break;
                    case "west": rx = .25f; rz = .75f - segment * .5f; break;
                    case "south": rx = .75f - segment * .5f; rz = .75f; break;
                    default: rx = 0; rz = 0; break;
                }

                tfMatrices[index] = new Matrixf()
                    .Translate(rx, 0, rz)
                    .RotateYDeg(item * 22.5f)
                    .Translate(-rx, 0, -rz)
                    .Translate(0.5f, 0, 0.5f)
                    .RotateYDeg(block.Shape.rotateY)
                    .Scale(0.95f, 0.95f, 0.95f)
                    .Translate(x - 0.5f, y, z - 0.5f)
                    .Values;
            }
        }

        return tfMatrices;
    }
}
