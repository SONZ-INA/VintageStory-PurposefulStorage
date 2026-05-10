namespace PurposefulStorage;

public class BESwordPlaque : BEBasePSContainer {
    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public BESwordPlaque() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (id, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.offsetY = 0.15f;
            td.offsetZ = -0.39f;
            td.offsetRotZ = -90;
        });
    }
}
