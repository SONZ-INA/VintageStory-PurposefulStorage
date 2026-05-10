namespace PurposefulStorage;

public class BEWeaponRack : BEBasePSContainer {
    public override string AttributeTransformCode => "onLongweaponsTransform";
    public override string[] AttributeCheck => ["psLongweapons"];

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int[] SectionSegmentCounts => [4];

    public BEWeaponRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.y = td.segment * 0.25f;

            td.offsetX = -0.4f;
            td.offsetY = 0.18f;
            td.offsetZ = -0.2f;

            td.offsetRotX = -25;
        });
    }
}
