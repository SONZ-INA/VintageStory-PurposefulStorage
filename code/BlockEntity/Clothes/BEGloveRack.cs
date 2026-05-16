namespace PurposefulStorage;

public class BEGloveRack : BEBasePSContainer {
    public override string AttributeTransformCode => "onHandwareTransform";
    public override string[] AttributeCheck => ["psHandware"];

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int[] SectionSegmentCounts => [3];

    public BEGloveRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.y = td.segment * 0.31f;

            td.offsetY = 0.55f;
            td.offsetZ = 0.6f;

            td.offsetRotX = -110;
            td.offsetRotY = 90;
        });
    }
}
