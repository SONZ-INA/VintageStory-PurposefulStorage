namespace PurposefulStorage;

public class BEGearRack : BEBasePSContainer {
    public override string AttributeTransformCode => "onGearsTransform";
    public override string[] AttributeCheck => ["psGears"];

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int[] SectionSegmentCounts => [2];
    public override int ItemsPerSegment => 12;

    public BEGearRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.scaleX = td.scaleZ = 0.95f;
            td.scaleY = 0.92f;

            td.x = -0.25f + td.segment * 0.5f;
            td.y = td.item * 0.072f;
            td.z = -0.24f;
            
            td.rotY = td.item * 22.5f;

            td.offsetOriginX = 0.15f;
            td.offsetOriginZ = 0.05f;

            td.offsetY = 0.0625f;
        });
    }
}
