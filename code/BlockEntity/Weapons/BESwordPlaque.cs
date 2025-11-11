namespace PurposefulStorage;

public class BESwordPlaque : BEBasePSContainer {
    public BESwordPlaque() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (id, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        float[][] tfMatrices = new float[1][];

        float x = -0.15f;
        float y = 0f;
        float z = -0.39f;

        tfMatrices[0] = new Matrixf()
            .Translate(0.5f, 0, 0.5f)
            .RotateYDeg(block.Shape.rotateY)
            .RotateZDeg(-90)
            .Translate(x - 0.5f, y, z - 0.5f)
            .Values;

        return tfMatrices;
    }
}
