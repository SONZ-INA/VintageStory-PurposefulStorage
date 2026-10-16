namespace PurposefulStorage;

public class BEApothecaryCabinet : BEBasePSAnimatable {
    protected new BlockApothecaryCabinet block = null!;

    protected override string ReferencedShape => ShapeReferences.ApothecaryCabinet;

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int[] SectionSegmentCounts => [3];

    private readonly AnimationData[] doorAnims = [
        new("dooropenl", 3f),
        new("dooropenr", 3f),
        new("dooropent", 3f)
    ];

    [TreeSerializable(false)] public bool DoorOpenL { get; set; }
    [TreeSerializable(false)] public bool DoorOpenR { get; set; }
    [TreeSerializable(false)] public bool DoorOpenT { get; set; }

    protected enum SlotType {
        LSegment = 0,
        RSegment = 1,
        TSegment = 2,
        LDoor = 3,
        RDoor = 4,
        TDoor = 5
    }

    public BEApothecaryCabinet() {
        inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (id, inv) => {
            if (id < 2) return new ItemSlotPSUniversal(inv, "psPoultices", 1, true);
            else return new ItemSlotPSUniversal(inv, "psBandages", 1, true);
        });
    }

    public override void Initialize(ICoreAPI api) {
        block = (api.World.BlockAccessor.GetBlock(Pos) as BlockApothecaryCabinet)!;
        base.Initialize(api);
        inv.OnAcquireTransitionSpeed += Inventory_OnAcquireTransitionSpeed;
    }

    public virtual float Inventory_OnAcquireTransitionSpeed(EnumTransitionType transType, ItemStack stack, float baseMul) {
        if (transType == EnumTransitionType.Dry) {
            return 0;
        }

        return 1;
    }

    public bool IsDoorOpen(int segment) => segment switch {
        0 => DoorOpenL,
        1 => DoorOpenR,
        2 => DoorOpenT,
        _ => false
    };

    private void SetDoorOpenState(int segment, bool open) {
        switch (segment) {
            case 0: DoorOpenL = open; break;
            case 1: DoorOpenR = open; break;
            case 2: DoorOpenT = open; break;
        }
    }

    public override bool OnInteract(IPlayer byPlayer, BlockSelection blockSel, string? overrideAttrCheck = null) {
        int index = blockSel.SelectionBoxIndex;
        if (index < 0 || index >= 6) return false;

        if (index >= 3) {
            ToggleDoor(index - 3, false, byPlayer);
            return true;
        }

        if (!IsDoorOpen(index)) {
            ToggleDoor(index, true, byPlayer);
            return true;
        }

        ItemSlot slot = byPlayer.InventoryManager.ActiveHotbarSlot;

        if (slot.Empty) {
            return TryTake(byPlayer, blockSel);
        }

        if (TryPut(byPlayer, slot, blockSel)) {
            return this.HandlePlacementEffects(slot.Itemstack, byPlayer);
        }

        (Api as ICoreClientAPI)?.TriggerIngameError(this, "cantplace", Lang.Get("purposefulstorage:Can't place this item here, it's either not the right type, or not the right size for this section."));
        return false;
    }

    #region Animation

    protected override void HandleAnimations() {
        if (AnimUtil == null) return;

        for (int i = 0; i < 3; i++) {
            ApplyDoorAnimation(i, IsDoorOpen(i));
        }
    }

    private void ApplyDoorAnimation(int segment, bool open) {
        if (AnimUtil == null) return;

        AnimationData anim = doorAnims[segment];
        if (open) AnimUtil.TryStartAnimation(anim.Code, anim.Speed);
        else AnimUtil.TryStopAnimation(anim.Code);
    }

    protected void ToggleDoor(int segment, bool open, IPlayer? byPlayer = null) {
        if (segment < 0 || segment >= 3) return;
        if (IsDoorOpen(segment) == open) return;

        SetDoorOpenState(segment, open);
        ApplyDoorAnimation(segment, open);

        if (byPlayer != null) {
            Api.World.PlaySoundAt(open ? block.soundCabinetOpen : block.soundCabinetClose, byPlayer.Entity, byPlayer, true, 16, 0.3f);
        }

        MarkDirty(true);
    }

    #endregion

    protected override string getMeshCacheKey(ItemSlot slot) {
        int capacity = slot.StackSize + slot.GetRemainingSlotSpace(slot.Itemstack);
        int visible = GetStackedVisibleCount(capi, slot.Itemstack, "apothecarycabinet", capacity);

        if (visible == 0) return base.getMeshCacheKey(slot); // no stacked shape - normal item and key

        return $"ps-stacked-{slot.Itemstack?.Collectible.Code}-{visible}";
    }

    protected override MeshData getOrCreateMesh(ItemSlot slot, int index) {
        MeshData? mesh = getMesh(slot);
        if (mesh != null) return mesh;

        int capacity = slot.StackSize + slot.GetRemainingSlotSpace(slot.Itemstack);
        mesh = GenStackedShapeMesh(capi, slot.Itemstack, "apothecarycabinet", capacity);
        if (mesh == null) return base.getOrCreateMesh(slot, index); // fallback - normal item

        MeshCache[getMeshCacheKey(slot)] = mesh;
        return mesh;
    }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            if (td.segment == 1) {
                td.offsetX = 0.4725f;
            }
        });
    }
}
