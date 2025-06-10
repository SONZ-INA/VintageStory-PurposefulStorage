using System.Linq;

namespace PurposefulStorage;

public abstract class BEBasePSContainer : BlockEntityDisplay, IPurposefulStorageContainer {
    public InventoryGeneric inv;
    protected BasePSContainer block;
    protected MeshData blockMesh;

    public override InventoryBase Inventory => inv;
    public override string InventoryClassName => Block?.Code.FirstCodePart();
    public override string AttributeTransformCode => "on" + Block?.Code.FirstCodePart() + "Transform";

    public ITreeAttribute VariantAttributes { get; set; } = new TreeAttribute();
    public virtual string[] AttributeCheck => new[] { "ps" + GetType().Name.Replace("BE", "") };

    protected virtual string CantPlaceMessage => "";
    protected virtual InfoDisplayOptions InfoDisplay { get; set; } = InfoDisplayOptions.BySegment; 

    public virtual int[] SectionSegmentCounts { get; set; } = { 1 };
    public virtual int ItemsPerSegment { get; set; } = 1;
    public virtual int AdditionalSlots { get; set; } = 0;
    public virtual int SlotCount => SectionSegmentCounts.Sum() * ItemsPerSegment + AdditionalSlots;

    public override void Initialize(ICoreAPI api) {
        block ??= api.World.BlockAccessor.GetBlock(Pos) as BasePSContainer;
        base.Initialize(api);
        if (blockMesh == null) InitMesh();
    }

    protected virtual void InitMesh() {
        blockMesh = GenBlockVariantMesh(Api, GetVariantStack(this));
    }

    public override void OnBlockPlaced(ItemStack byItemStack = null) {
        base.OnBlockPlaced(byItemStack);

        if (byItemStack?.Attributes[BasePSContainer.PSAttributes] is ITreeAttribute tree) {
            if (VariantAttributes.Count == 0) VariantAttributes = tree;
        }
        
        InitMesh();
    }

    public virtual bool OnInteract(IPlayer byPlayer, BlockSelection blockSel) {
        ItemSlot slot = byPlayer.InventoryManager.ActiveHotbarSlot;

        if (slot.Empty) {
            return TryTake(byPlayer, blockSel);
        }
        else {
            bool canStore = false;
            foreach (var attribute in AttributeCheck) {
                if (slot.CanStoreInSlot(attribute)) {
                    canStore = true;
                    break;
                }
            }

            if (canStore) {
                AssetLocation sound = slot.Itemstack?.Block?.Sounds?.Place;

                if (TryPut(byPlayer, slot, blockSel)) {
                    Api.World.PlaySoundAt(sound ?? new AssetLocation("sounds/player/build"), byPlayer.Entity, byPlayer, true, 16);
                    return true;
                }
            }

            if (CantPlaceMessage != "") {
                (Api as ICoreClientAPI)?.TriggerIngameError(this, "cantplace", Lang.Get(CantPlaceMessage));
            }

            return false;
        }
    }

    protected virtual bool TryPut(IPlayer byPlayer, ItemSlot slot, BlockSelection blockSel) {
        int startIndex = blockSel.SelectionBoxIndex * ItemsPerSegment;
        if (startIndex >= inv.Count) return false;

        for (int i = 0; i < ItemsPerSegment; i++) {
            int currentIndex = startIndex + i;
            ItemStack currentStack = inv[currentIndex].Itemstack;

            if (inv[currentIndex].Empty || (currentStack.Collectible.Equals(slot.Itemstack.Collectible) && currentStack.StackSize < currentStack.Collectible.MaxStackSize)) {
                int moved = byPlayer.Entity.Controls.ShiftKey
                    ? slot.TryPutInto(Api.World, inv[currentIndex], inv[currentIndex].MaxSlotStackSize - inv[currentIndex].Itemstack?.StackSize ?? 64)
                    : slot.TryPutInto(Api.World, inv[currentIndex]);

                if (moved > 0) {
                    InitMesh();
                    MarkDirty();
                    (Api as ICoreClientAPI)?.World.Player.TriggerFpAnimation(EnumHandInteract.HeldItemInteract);
                    return true;
                }
            }
        }

        return false;
    }

    protected virtual bool TryTake(IPlayer byPlayer, BlockSelection blockSel) {
        int startIndex = blockSel.SelectionBoxIndex * ItemsPerSegment;
        if (startIndex >= inv.Count) return false;

        for (int i = ItemsPerSegment - 1; i >= 0; i--) {
            int currentIndex = startIndex + i;
            if (!inv[currentIndex].Empty) {
                ItemStack stack = byPlayer.Entity.Controls.ShiftKey
                    ? inv[currentIndex].TakeOutWhole()
                    : inv[currentIndex].TakeOut(1);

                if (byPlayer.InventoryManager.TryGiveItemstack(stack)) {
                    AssetLocation sound = stack.Block?.Sounds?.Place;
                    Api.World.PlaySoundAt(sound ?? new AssetLocation("sounds/player/build"), byPlayer.Entity, byPlayer, true, 16);
                }

                if (stack.StackSize > 0) {
                    Api.World.SpawnItemEntity(stack, Pos.ToVec3d().Add(0.5, 0.5, 0.5));
                }

                (Api as ICoreClientAPI)?.World.Player.TriggerFpAnimation(EnumHandInteract.HeldItemInteract);
                InitMesh();
                MarkDirty();
                return true;
            }
        }

        return false;
    }

    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator) {
        mesher.AddMeshData(blockMesh);
        base.OnTesselation(mesher, tesselator);
        return true;
    }

    protected virtual bool BaseRenderContents(ITerrainMeshPool mesher, ITesselatorAPI tesselator) {
        return base.OnTesselation(mesher, tesselator);
    }

    protected abstract override float[][] genTransformationMatrices();

    public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving) {
        base.FromTreeAttributes(tree, worldForResolving);

        VariantAttributes = tree[BasePSContainer.PSAttributes] is ITreeAttribute fsTree 
            ? fsTree 
            : new TreeAttribute();

        RedrawAfterReceivingTreeAttributes(worldForResolving);
    }

    public override void ToTreeAttributes(ITreeAttribute tree) {
        base.ToTreeAttributes(tree);
        if (VariantAttributes.Count != 0) {
            tree[BasePSContainer.PSAttributes] = VariantAttributes;
        }
    }

    public override void GetBlockInfo(IPlayer forPlayer, StringBuilder sb) {
        DisplayInfo(forPlayer, sb, inv, InfoDisplay, SlotCount, SectionSegmentCounts.Count(), ItemsPerSegment, SlotCount - AdditionalSlots);
    }
}
