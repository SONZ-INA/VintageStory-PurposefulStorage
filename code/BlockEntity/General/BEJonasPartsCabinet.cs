namespace PurposefulStorage;

public class BEJonasPartsCabinet : BEBasePSContainer {
    public override string[] AttributeCheck => ["psJonasParts"];

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    protected override string CantPlaceMessage => "purposefulstorage:Can't place this item here, it's either not the right type, or not the right size for this section.";

    public override int[] SectionSegmentCounts => [13];

    public BEJonasPartsCabinet() {
        inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (id, inv) => {
            if (id != 6) return new ItemSlotPSUniversal(inv, AttributeCheck, 1, true);
            else return new ItemSlotPSUniversal(inv, new AssetLocation("game:jonasframes-gearbox02"), 1, true);
        });
    }

    protected override string getMeshCacheKey(ItemSlot slot) {
        int capacity = slot.StackSize + slot.GetRemainingSlotSpace(slot.Itemstack);
        int visible = GetStackedVisibleCount(capi, slot.Itemstack, "jonasparts", capacity);

        if (visible == 0) return base.getMeshCacheKey(slot); // no stacked shape - normal item and key

        return $"ps-stacked-{slot.Itemstack?.Collectible.Code}-{visible}";
    }

    protected override MeshData getOrCreateMesh(ItemSlot slot, int index) {
        MeshData? mesh = getMesh(slot);
        if (mesh != null) return mesh;

        int capacity = slot.StackSize + slot.GetRemainingSlotSpace(slot.Itemstack);
        mesh = GenStackedShapeMesh(capi, slot.Itemstack, "jonasparts", capacity);
        if (mesh == null) return base.getOrCreateMesh(slot, index); // fallback: normal item

        MeshCache[getMeshCacheKey(slot)] = mesh;
        return mesh;
    }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.offsetX = -0.25f;
            td.offsetZ = -0.255f;

            if (td.index < 6) {
                td.x = td.index / 2 * 0.42f;
                td.y = td.index % 2 * 0.485f;
            }

            if (td.index == 6) {
                td.x = 1.26f;
            }

            if (td.index > 6) {
                td.x = (td.index + 1) / 2 * 0.42f;
                td.y = td.index % 2 * 0.485f;
            }
        });
    }
}
