namespace PurposefulStorage;

public class BENecklaceStand : BEBasePSContainer {
    public override string[] AttributeCheck => ["psNeckware"];

    public override int[] SectionSegmentCounts => [6];

    public BENecklaceStand() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override bool TryPut(IPlayer byPlayer, ItemSlot slot, BlockSelection blockSel) {
        if (slot.Itemstack?.Collectible?.Code.Path.EndsWith("marketeer") == true) // Marketeer backlisted
            return false;

        return base.TryPut(byPlayer, slot, blockSel);
    }

    protected override float[][] genTransformationMatrices() {
        float[][] tfMatrices = new float[SlotCount][];
        
        for (int segment = 0; segment < SectionSegmentCounts[0]; segment++) {
            int index = segment * ItemsPerSegment;

            float x = -0.1f;
            float y = segment / 3 * 0.43525f;
            float z = segment % 3 * 0.275f;

            tfMatrices[index] = new Matrixf()
                .Translate(0.5f, 0, 0.5f)
                .RotateYDeg(block.Shape.rotateY + 90)
                .Translate(x - 0.28f, y - 1.2225f, z - 0.78f)
                .Values;
        }

        return tfMatrices;
    }
}
