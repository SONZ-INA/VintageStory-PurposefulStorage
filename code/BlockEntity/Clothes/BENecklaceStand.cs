namespace PurposefulStorage;

public class BENecklaceStand : BEBasePSContainer {
    public override string AttributeTransformCode => "onNeckwareTransform";
    public override string[] AttributeCheck => ["psNeckware"];

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int[] SectionSegmentCounts => [6];

    public BENecklaceStand() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override bool TryPut(IPlayer byPlayer, ItemSlot slot, BlockSelection blockSel) {
        if (slot.Itemstack?.Collectible?.Code.Path.EndsWith("marketeer") == true) // Marketeer backlisted
            return false;

        return base.TryPut(byPlayer, slot, blockSel);
    }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.x = td.segment % 3 * 0.2815f;
            td.y = td.segment / 3 * 0.4365f;

            td.offsetX = -0.2825f;
            td.offsetY = -1.2225f;
            td.offsetZ = -0.1375f;

            td.offsetRotY = 90;
        });
    }
}
