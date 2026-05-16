namespace PurposefulStorage;

public class BETuningCylinderRack : BEBasePSContainer {
    public override string[] AttributeCheck => ["psTuningCylinders"];

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int[] SectionSegmentCounts => [8];

    public BETuningCylinderRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            if (block.Variant["type"] == "stand") {
                td.x = td.segment % 4 * 0.25f;
                td.y = td.segment / 4 * 0.275f;
                td.z = td.segment / 4 * -0.45f;

                td.offsetX = -0.375f;
                td.offsetY = 0.3f;
                td.offsetZ = -0.02f;

                td.offsetRotX = 90;
            }
            else {
                td.x = td.segment % 4 * 0.25f;
                td.y = td.segment / 4 * 0.4425f;

                td.offsetX = -0.375f;
                td.offsetY = 0.075f;
                td.offsetZ = -0.3525f;

                td.offsetRotX = 30;
            }
        });
    }
}
