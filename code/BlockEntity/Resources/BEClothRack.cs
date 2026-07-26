namespace PurposefulStorage;

public class BEClothRack : BEBasePSContainer {
    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.ByBlock;

    public override int[] SectionSegmentCounts => [4];

    public BEClothRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck, 1, true)); }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.offsetZ = -0.29f;
            td.offsetY = 0.04f;

            td.rotX = 8;

            td.y = td.segment * 0.25f;
        });
    }
}
