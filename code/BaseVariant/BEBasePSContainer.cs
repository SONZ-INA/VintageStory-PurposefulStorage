using System.Linq;

namespace PurposefulStorage;

public abstract class BEBasePSContainer : BlockEntityDisplay, IPurposefulStorageContainer {
    public InventoryGeneric inv = null!;
    protected BasePSContainer block = null!;
    protected MeshData? blockMesh;

    public override InventoryBase Inventory => inv;
    public override string InventoryClassName => (Block?.Code.FirstCodePart() ?? "-temp-");
    public override string AttributeTransformCode => "on" + (Block?.Code.FirstCodePart() ?? "-temp-") + "Transform";

    public ITreeAttribute VariantAttributes { get; set; } = new TreeAttribute();
    public virtual string[] AttributeCheck => ["ps" + GetType().Name.Replace("BE", "")];

    protected abstract InfoDisplayOptions InfoDisplay { get; }

    protected virtual string CantPlaceMessage => "";

    public virtual int[] SectionSegmentCounts { get; set; } = [1];
    public virtual int ItemsPerSegment { get; set; } = 1;
    public virtual int AdditionalSlots { get; set; } = 0;
    public virtual int SlotCount => SectionSegmentCounts.Sum() * ItemsPerSegment + AdditionalSlots;

    protected bool isBulk = false;

    public override void Initialize(ICoreAPI api) {
        block ??= (api.World.BlockAccessor.GetBlock(Pos) as BasePSContainer)!;
        
        base.Initialize(api);

        if (blockMesh == null) InitMesh();

        inv.OnGetAutoPushIntoSlot = (_, _) => null;
        inv.OnGetAutoPullFromSlot = _ => null;

        foreach (var inv in Inventory) {
            ItemSlotPSUniversal? psSlot = inv as ItemSlotPSUniversal;
            if (psSlot?.isBulk == true) {
                isBulk = true;
                break;
            }
        }
    }

    protected virtual void InitMesh() {
        blockMesh = GenBlockVariantMesh(Api, this.GetVariantStack());
    }

    public override void OnBlockPlaced(ItemStack byItemStack) {
        base.OnBlockPlaced(byItemStack);

        if (byItemStack?.Attributes[PSAttributes] is ITreeAttribute tree) {
            if (VariantAttributes.Count == 0) VariantAttributes = tree;
        }

        InitMesh();
    }

    public virtual bool OnInteract(IPlayer byPlayer, BlockSelection blockSel, string? overrideAttrCheck = null) {
        ItemSlot slot = byPlayer.InventoryManager.ActiveHotbarSlot;

        bool shift = byPlayer.Entity.Controls.ShiftKey;

        bool placeBulk = isBulk && shift;
        bool placeSingle = !isBulk && !shift && !slot.Empty;

        if (placeBulk || placeSingle) {
            if (slot.Empty) return false;

            var checks = overrideAttrCheck != null
                ? [overrideAttrCheck]
                : AttributeCheck;

            if (checks.Any(slot.CanStoreInSlot)) {
                if (TryPut(byPlayer, slot, blockSel)) {
                    return this.HandlePlacementEffects(slot.Itemstack, byPlayer);
                }
            }

            if (CantPlaceMessage != "") {
                (Api as ICoreClientAPI)?.TriggerIngameError(this, "cantplace", Lang.Get(CantPlaceMessage));
            }

            return false;
        }

        return TryTake(byPlayer, blockSel);
    }

    protected virtual bool TryPut(IPlayer byPlayer, ItemSlot slot, BlockSelection blockSel) {
        int segmentIndex = blockSel.SelectionBoxIndex;

        int startIndex = segmentIndex * ItemsPerSegment;
        if (startIndex >= inv.Count) return false;

        ItemStack incoming = slot.Itemstack!;

        if (!CanInsertIntoSegment(inv[startIndex].Itemstack, incoming))
            return false;

        if (!isBulk) {
            int limit = GetSegmentLimit(incoming);
            int count = CountItemsInSegment(startIndex);

            if (count >= limit)
                return false;
        }

        bool ctrl = byPlayer.Entity.Controls.CtrlKey;
        int moved = TryPutIntoSegment(slot, startIndex, ctrl);

        if (moved > 0) {
            InitMesh();
            MarkDirty();
            (Api as ICoreClientAPI)?.World.Player.TriggerFpAnimation(EnumHandInteract.HeldItemInteract);
            return true;
        }

        return false;
    }

    protected virtual int TryPutIntoSegment(ItemSlot slot, int startIndex, bool ctrl) {
        int moved = 0;

        for (int i = 0; i < ItemsPerSegment; i++) {
            int idx = startIndex + i;
            ItemSlot target = inv[idx];

            if (target.Empty || target.Itemstack!.Collectible == slot.Itemstack!.Collectible) {
                var fsSlot = (ItemSlotPSUniversal)target;
                int available = fsSlot.GetRemainingSlotSpace(slot.Itemstack!);
                if (available == 0) continue;

                moved = slot.TryPutIntoBulk(Api.World, target, ctrl ? available : 1);
                if (moved > 0) {
                    // If it's bulk, continue placing items iteratively
                    if (moved <= slot.StackSize && ctrl) {
                        continue;
                    }

                    break;
                }
            }
        }

        return moved;
    }

    protected virtual bool TryTake(IPlayer byPlayer, BlockSelection blockSel) {
        int startIndex = blockSel.SelectionBoxIndex * ItemsPerSegment;
        if (startIndex >= inv.Count) return false;

        ItemStack? stack = TryTakeFromSegment(byPlayer, startIndex);
        if (stack == null) return false;

        if (byPlayer.InventoryManager.TryGiveItemstack(stack)) {
            this.HandlePlacementEffects(stack, byPlayer);
        }

        if (stack.StackSize > 0) {
            Api.World.SpawnItemEntity(stack, Pos.ToVec3d().Add(0.5, 0.5, 0.5));
        }

        InitMesh();
        return true;
    }

    protected virtual ItemStack? TryTakeFromSegment(IPlayer byPlayer, int startIndex) {
        for (int i = ItemsPerSegment - 1; i >= 0; i--) {
            int idx = startIndex + i;

            if (!inv[idx].Empty) {
                return byPlayer.Entity.Controls.CtrlKey
                    ? inv[idx].TakeOut(inv[idx].Itemstack!.Collectible.MaxStackSize)
                    : inv[idx].TakeOut(1);
            }
        }

        return null;
    }

    protected virtual bool TryTakeFromSlot(IPlayer byPlayer, ItemSlot slot, int quantity = 1) {
        if (!slot.Empty) {
            ItemStack stack = slot.TakeOut(quantity);

            if (byPlayer.InventoryManager.TryGiveItemstack(stack)) {
                this.HandlePlacementEffects(stack, byPlayer);
            }

            if (stack.StackSize > 0) {
                Api.World.SpawnItemEntity(stack, Pos.ToVec3d().Add(0.5, 0.5, 0.5));
            }

            InitMesh();
            return true;
        }

        return false;
    }

    protected virtual int GetSegmentLimit(ItemStack? stack) {
        return ItemsPerSegment;
    }

    public virtual int CountItemsInSegment(int startIndex) {
        int count = 0;

        for (int i = 0; i < ItemsPerSegment; i++) {
            if (!inv[startIndex + i].Empty) {
                count++;
            }
        }

        return count;
    }

    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator) {
        mesher.AddMeshData(blockMesh);
        base.OnTesselation(mesher, tesselator);
        return true;
    }

    protected virtual bool BaseRenderContents(ITerrainMeshPool mesher, ITesselatorAPI tesselator) {
        return base.OnTesselation(mesher, tesselator);
    }

    protected abstract override float[][]? genTransformationMatrices();

    public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldForResolving) {
        base.FromTreeAttributes(tree, worldForResolving);

        VariantAttributes = tree[PSAttributes] is ITreeAttribute psTree
            ? psTree
            : new TreeAttribute();

        RedrawAfterReceivingTreeAttributes(worldForResolving);
    }

    public override void ToTreeAttributes(ITreeAttribute tree) {
        base.ToTreeAttributes(tree);
        if (VariantAttributes.Count != 0) {
            tree[PSAttributes] = VariantAttributes;
        }
    }

    public override void GetBlockInfo(IPlayer forPlayer, StringBuilder sb) {
        DisplayInfo(forPlayer, sb, inv, InfoDisplay, SlotCount, SectionSegmentCounts.Length, ItemsPerSegment, SlotCount - AdditionalSlots);
    }
}
