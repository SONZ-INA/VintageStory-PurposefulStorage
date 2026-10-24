namespace PurposefulStorage;

public class BEPeltRack : BEBasePSContainer {
    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.ByBlock;

    public override int[] SectionSegmentCounts => [12];

    public BEPeltRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.offsetX = -0.35f;
            td.offsetY = 1.875f;

            td.offsetRotX = 90;
            td.offsetRotZ = -90;

            td.x = td.segment * 0.06f;
        });
    }
}
