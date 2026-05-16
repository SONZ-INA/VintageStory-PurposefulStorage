namespace PurposefulStorage;

public class BESaddleRack : BEBasePSContainer {
    public override string[] AttributeCheck => ["psSaddle"];

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int[] SectionSegmentCounts => [1];

    public BESaddleRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.offsetY = 0.0625f;
        });
    }
}
