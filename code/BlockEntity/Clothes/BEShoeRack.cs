namespace PurposefulStorage;

public class BEShoeRack : BEBasePSContainer {
    public override string AttributeTransformCode => "onFootwareTransform";
    public override string[] AttributeCheck => ["psFootware"];

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int[] SectionSegmentCounts => [4];

    public BEShoeRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.scaleX = td.scaleY = td.scaleZ = 0.85f;

            td.x = td.segment % 2 * 0.44f;
            td.y = td.segment / 2 * 0.1925f;
            td.z = td.segment / 2 * -0.425f;

            td.offsetX = -0.22f;
            td.offsetY = 0.0625f;
            td.offsetZ = 0.23f;

            td.offsetRotY = 90;
        });
    }
}
