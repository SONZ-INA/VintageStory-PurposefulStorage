namespace PurposefulStorage;

public class BERopeRack : BEBasePSContainer {
    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.ByBlock;

    public override int[] SectionSegmentCounts => [2];
    public override int ItemsPerSegment => 8;

    public BERopeRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.scaleX = td.scaleY = td.scaleZ = 0.85f;

            td.offsetRotZ = 90;
            td.offsetRotX = 90;

            td.offsetX = -0.335f;
            td.offsetY = 0.2375f;
            td.offsetZ = -0.125f;

            td.x = td.item * 0.11f;
            td.y = td.segment * 0.5f;
        });
    }
}
