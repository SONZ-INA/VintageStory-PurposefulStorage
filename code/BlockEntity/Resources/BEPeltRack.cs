namespace PurposefulStorage;

public class BEPeltRack : BEBasePSContainer {
    protected bool IsSmall => block?.Variant["type"] == "small";

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.ByBlock;

    public override int[] SectionSegmentCounts => IsSmall ? [1] : [26];
    public override int ItemsPerSegment => IsSmall ? 26 : 1;

    public BEPeltRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    public override void Initialize(ICoreAPI api) {
        block = (api.World.BlockAccessor.GetBlock(Pos) as BlockPeltRack)!;

        if (block.Variant["type"] == "small") {
            AttributeCheckOverride = ["psPeltsSmall"];
            this.RebuildInventory(api);
        }

        if (block.Variant["type"] == "normal") {
            AttributeCheckOverride = ["psPelts"];
            this.RebuildInventory(api);
        }

        if (block.Variant["type"] == "huge") {
            AttributeCheckOverride = ["psPeltsHuge"];
            this.RebuildInventory(api);
        }

        base.Initialize(api);
        inv.OnAcquireTransitionSpeed += Inventory_OnAcquireTransitionSpeed;
    }

    public virtual float Inventory_OnAcquireTransitionSpeed(EnumTransitionType transType, ItemStack stack, float baseMul) {
        if (transType == EnumTransitionType.Cure) {
            return 2;
        }

        return 1;
    }

    protected override float[][] genTransformationMatrices() {
        if (block.Variant["type"] == "small") {
            return TransformationGenerator.GenerateLayout(this, td => {
                td.offsetY = 0.1525f;
                td.offsetZ = 0.025f;

                td.offsetRotX = 11.5f;
                td.offsetRotY = StaticRandom(td.item, 10);

                td.y = td.item * 0.0275f;
            });
        }

        if (block.Variant["type"] == "normal") {
            return TransformationGenerator.GenerateLayout(this, td => {
                td.scaleX = td.scaleY = 0.85f;
                td.scaleZ = 0.825f;

                td.offsetX = -0.3f;
                td.offsetY = 1.075f;

                td.offsetRotX = 90 + StaticRandom(Pos.X + td.segment, 1f);
                td.offsetRotY = StaticRandom(Pos.Y + td.segment, 1.25f);
                td.offsetRotZ = -90 + StaticRandom(Pos.Z + td.segment, 1f);

                td.x = td.segment * 0.0625f;
                td.y = GenerateFalloff(td.segment, 26, 0.02f);
            });
        }

        if (block.Variant["type"] == "huge") {
            return TransformationGenerator.GenerateLayout(this, td => {
                td.scaleX = td.scaleY = td.scaleZ = 0.95f;

                td.offsetX = -0.3f;
                td.offsetY = 1.8f;
                td.offsetZ = -0.525f;

                td.offsetRotX = 90 + StaticRandom(Pos.X + td.segment, 1f);
                td.offsetRotY = StaticRandom(Pos.Y + td.segment, 1.25f);
                td.offsetRotZ = -90 + StaticRandom(Pos.Z + td.segment, 1f);

                td.x = td.segment * 0.0625f;
                td.y = GenerateFalloff(td.segment, 26, 0.02f);
            });
        }

        return [];
    }

    public override void GetBlockInfo(IPlayer forPlayer, StringBuilder sb) {
        base.GetBlockInfo(forPlayer, sb);

        if (block.Variant["type"] == "small") 
            return;

        sb.AppendLine();

        int slotIndex = forPlayer.CurrentBlockSelection.SelectionBoxIndex;
        if (slotIndex < 26 && !inv[slotIndex].Empty) {
            sb.AppendLine(inv[slotIndex].Itemstack?.GetName());
            sb.Append(TransitionInfoCompact(Api.World, inv[slotIndex], EnumTransitionType.Cure, TransitionDisplayMode.TimeLeft));
        }
    }
}
