namespace PurposefulStorage;

public class BESwordPedestal : BEBasePSContainer {
    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public BESwordPedestal() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.offsetY = 0.9f;
            td.offsetRotZ = -90;
        });
    }
}
