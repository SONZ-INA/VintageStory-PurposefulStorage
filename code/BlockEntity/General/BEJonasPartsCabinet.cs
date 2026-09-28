namespace PurposefulStorage;

public class BEJonasPartsCabinet : BEBasePSContainer {
    public override string[] AttributeCheck => ["psJonasParts"];

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int[] SectionSegmentCounts => [13];

    public BEJonasPartsCabinet() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck, 1, true)); }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            
        });
    }
}
