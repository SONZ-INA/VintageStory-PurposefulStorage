
using PurposefulStorage;

public class BEArrowRack : BEBasePSContainer {
    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.ByBlock;

    public override string[] AttributeCheck => ["psArrows"];
    public override int[] SectionSegmentCounts => [3];

    private const int MeshesPerSegment = 25;

    public BEArrowRack() {
        inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck, 1, true));
    }

    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator) {
        if (tfMatrices == null) {
            updateMeshes();
        }

        mesher.AddMeshData(blockMesh);

        for (int i = 0; i < inv.Count; i++) {
            ItemSlot slot = inv[i];
            if (slot.Empty) continue;

            MeshData? mesh = getMesh(slot);
            if (mesh == null) continue;

            int startIndex = i * MeshesPerSegment;

            float[][] segmentMatrices = tfMatrices![
                startIndex..(startIndex + MeshesPerSegment)
            ];

            RenderStackedMeshes(mesher, mesh, slot, segmentMatrices);
        }

        return true;
    }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.scaleX = td.scaleY = td.scaleZ = 0.65f;

            td.offsetX = -0.41f;
            td.offsetY = 0.065f;

            td.offsetRotY = -90;

            td.rotY = StaticRandom(td.stackItem, 2);
            td.rotX = td.stackItem / 5 * 2;
            td.rotZ = StaticRandom(td.stackItem, 1);

            td.x = td.segment * 0.3125f + td.stackItem % 5 * 0.052f;
            td.y = td.stackItem / 5 * 0.05f;
        }, MeshesPerSegment);
    }
}
