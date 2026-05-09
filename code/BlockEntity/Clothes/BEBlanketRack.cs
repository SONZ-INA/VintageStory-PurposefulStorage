namespace PurposefulStorage;

public class BEBlanketRack : BEBasePSContainer {
    public override string AttributeTransformCode => "onBlanketsTransform";
    public override string[] AttributeCheck => ["psBlankets"];

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int[] SectionSegmentCounts => [4];

    public BEBlanketRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.y = td.segment * 0.2265f;

            td.offsetZ = -0.075f;
            td.offsetY = 0.0625f;

            td.offsetRotY = -45;
        });
    }
}
