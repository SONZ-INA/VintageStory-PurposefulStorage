namespace PurposefulStorage;

public class BESpearRack : BEBasePSContainer {
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
        float[][] tfMatrices = new float[SlotCount][];

        for (int item = 0; item < ItemsPerSegment; item++) {
            float x = 2.3f;
            float y = -0.45f + item % 5 * 0.15f + item / 5 * 0.025f;
            float z = -0.225f + item / 5 * 0.125f;

            tfMatrices[item] = new Matrixf()
                .Translate(0.5f, 0, 0.5f)
                .RotateYDeg(block.Shape.rotateY)
                .RotateZDeg(90)
                .Translate(x - 0.5f, y, z - 0.5f)
                .RotateXDeg(-15)
                .RotateYDeg(10)
                .Values;
        }

        return tfMatrices;
    }
}
