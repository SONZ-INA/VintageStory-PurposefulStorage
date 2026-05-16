namespace PurposefulStorage;

public class BEHatRack : BEBasePSContainer {
    public override string AttributeTransformCode => "onHeadwareTransform";
    public override string[] AttributeCheck => ["psHeadware"];

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int[] SectionSegmentCounts => [8];

    public BEHatRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.scaleX = td.scaleY = td.scaleZ = 0.9f;

            td.x = td.segment % 2 * 0.435f;
            td.y = td.segment / 2 * 0.485f;

            td.offsetX = -0.215f;
            td.offsetY = -1.435f;
            td.offsetZ = -0.125f;

            td.offsetRotY = 90;
        });
    }
}
