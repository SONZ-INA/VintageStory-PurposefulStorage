namespace PurposefulStorage;

public class BEWeaponRack : BEBasePSContainer {
    public override string AttributeTransformCode => "onLongweaponsTransform";
    public override string[] AttributeCheck => new[] { "psLongweapons" };

    public override int[] SectionSegmentCounts => new[] { 4 };

    public BEWeaponRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        float[][] tfMatrices = new float[SlotCount][];
        
        for (int segment = 0; segment < SectionSegmentCounts[0]; segment++) {
            float x = -0.4f;
            float y = -0.03f + segment * 0.25f;
            float z = -0.15f;

            tfMatrices[segment] = new Matrixf()
                .Translate(0.5f, 0, 0.5f)
                .RotateYDeg(block.Shape.rotateY)
                .Translate(x - 0.5f, y, z - 0.5f)
                .RotateXDeg(-25)
                .Values;
        }

        return tfMatrices;
    }
}
