namespace PurposefulStorage;

public class BEButterflyDisplayPanel : BEBasePSContainer {
    public override string[] AttributeCheck => ["psButterflies"];

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int[] SectionSegmentCounts => [4];

    public BEButterflyDisplayPanel() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.scaleX = td.scaleY = td.scaleZ = 0.6f;

            td.x = td.segment % 2 * 0.4875f;
            td.y = td.segment / 2 * 0.5f;

            td.offsetX = -0.225f;
            td.offsetY = 0.225f;
            td.offsetZ = -0.46f;

            td.offsetRotY = -90;
            td.offsetRotZ = -90;
        });
    }
}
