namespace PurposefulStorage;

public class BEWeaponRack : BEBasePSContainer {
    public override string AttributeTransformCode => "onLongweaponsTransform";
    public override string[] AttributeCheck => ["psLongweapons"];

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int[] SectionSegmentCounts => [4];

    public BEWeaponRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    public override void Initialize(ICoreAPI api) {
        base.Initialize(api);
        inv.OnAcquireTransitionSpeed += Inventory_OnAcquireTransitionSpeed;
    }

    public virtual float Inventory_OnAcquireTransitionSpeed(EnumTransitionType transType, ItemStack stack, float baseMul) {
        if (transType == EnumTransitionType.Dry) {
            return 5;
        }

        return 1;
    }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.y = td.segment * 0.25f;

            td.offsetX = -0.4f;
            td.offsetY = 0.18f;
            td.offsetZ = -0.2f;

            td.offsetRotX = -25;
        });
    }

    public override void GetBlockInfo(IPlayer forPlayer, StringBuilder sb) {
        base.GetBlockInfo(forPlayer, sb);

        int slotIndex = forPlayer.CurrentBlockSelection.SelectionBoxIndex;
        sb.Append(TransitionInfoCompact(Api.World, inv[slotIndex], EnumTransitionType.Dry, TransitionDisplayMode.Percentage));
    }
}
