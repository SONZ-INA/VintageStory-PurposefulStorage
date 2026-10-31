namespace PurposefulStorage;

public class BEArrowRack : BEBasePSContainer {
    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.ByBlock;

    public override string[] AttributeCheck => ["psArrows"];

    public override int[] SectionSegmentCounts => [3];

    public BEArrowRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck, 1, true)); }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.scaleX = td.scaleY = td.scaleZ = 0.75f;

            td.offsetX = -0.385f;
            td.offsetY = 0.22f;
            td.offsetZ = -0.035f;

            td.offsetRotY = -90;
            td.offsetRotX = 20;

            td.x = td.segment * 0.3125f;
        });
    }
}
