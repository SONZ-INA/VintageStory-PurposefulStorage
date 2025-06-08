namespace PurposefulStorage;

public class BEHatRack : BEBasePSContainer {
    public override string AttributeTransformCode => "onHeadwareTransform";
    public override string[] AttributeCheck => new[] { "psHeadware" };
    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int[] SectionSegmentCounts => new[] { 8 };

    public BEHatRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    #region Transformation Matrices

    protected override float[][] genTransformationMatrices() {
        float[][] tfMatrices = new float[SlotCount][];
        
        for (int segment = 0; segment < SectionSegmentCounts[0]; segment++) {
            for (int item = 0; item < ItemsPerSegment; item++) {
                int index = segment * ItemsPerSegment + item;

                float x = 0.1f;
                float y = -1.63f + segment / 2 * 0.5f + item * 0.125f;
                float z = -0.215f + segment % 2 * 0.42f;

                tfMatrices[index] = new Matrixf()
                    .Translate(0.5f, 0, 0.5f)
                    .RotateYDeg(block.Shape.rotateY + 90)
                    .Translate(x - 0.5f, y, z - 0.5f)
                    .Values;
            }
        }

        return tfMatrices;
    }

    #endregion
}
