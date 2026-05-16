namespace PurposefulStorage;

public class BESpearRack : BEBasePSContainer {
    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int ItemsPerSegment => 10;

    public BESpearRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override bool TryPut(IPlayer byPlayer, ItemSlot slot, BlockSelection blockSel) {
        if (!slot.Itemstack.BelongsToSameGroupAs(inv[0].Itemstack)) {
            (Api as ICoreClientAPI)?.TriggerIngameError(this, "cantplace", Lang.Get("purposefulstorage:This item is not of the same type as the item in the container!"));
            return false;
        }

        return base.TryPut(byPlayer, slot, blockSel);
    }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.x = td.item % 5 * -0.1625f;
            td.z = td.item / 5 * 0.1f;
            td.rotZ = td.item / 5 == 0 ? -2 : 2;

            td.offsetX = td.item / 5 == 0 ? 0.3075f : 0.43f;
            td.offsetY = 1.3f;
            td.offsetZ = -0.225f;

            td.offsetRotX = -10;
            td.offsetRotY = -20;
            td.offsetRotZ = 90;
        });
    }
}
