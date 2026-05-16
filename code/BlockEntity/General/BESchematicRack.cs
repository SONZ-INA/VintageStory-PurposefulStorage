namespace PurposefulStorage;

public class BESchematicRack : BEBasePSContainer {
    public override string[] AttributeCheck => ["psSchematics"];

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int[] SectionSegmentCounts => [1];

    public BESchematicRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.offsetX = -0.025f;
            td.offsetY = 0.55f;
            td.offsetZ = -0.1765f;

            td.offsetRotX = 60;
        });
    }
}
