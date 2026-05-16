namespace PurposefulStorage;

public class BEBeltHooks : BEBasePSContainer {
    public override string AttributeTransformCode => "onWaistwareTransform";
    public override string[] AttributeCheck => ["psWaistware"];

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int[] SectionSegmentCounts => [4];

    public BEBeltHooks() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.x = td.segment % 2 * 0.475f;
            td.y = td.segment / 2 * 0.5f;

            td.offsetX = -0.055f;
            td.offsetY = -1.3f;
            td.offsetZ = -0.2725f;

            td.rotX = 90;
            td.rotY = 45;
        });
    }
}
