namespace PurposefulStorage;

public class BEApothecaryCabinet : BEBasePSAnimatable {
    // Take into account the "displayShape" introduced in 1.22
    protected virtual string DisplayType => "shelf";
    private DisplayableAttributes? GetDisplayProps(ItemSlot slot) => BlockBehaviorDisplay.GetDisplayableAttributes(slot, DisplayType);
    // --------------------

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

    private const int SlotsPerSegment = 32;
    private static readonly int[] SegmentCapacities = [16, 16, 32]; // L, R, T

    public override int ItemsPerSegment => SlotsPerSegment;

    public BEApothecaryCabinet() {
        inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (id, inv) => {
            int segment = id / SlotsPerSegment;
            int local = id % SlotsPerSegment;

            if (local >= SegmentCapacities[segment]) return new ItemSlotPSUniversal(inv, "ps-unused", 0);

            return segment < 2
                ? new ItemSlotPSUniversal(inv, "psPoultices")
                : new ItemSlotPSUniversal(inv, "psBandages");
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

    protected override bool TryPut(IPlayer byPlayer, ItemSlot slot, BlockSelection blockSel) {
        int segment = blockSel.SelectionBoxIndex;
        int startIndex = segment * ItemsPerSegment;
        if (segment < 0 || segment >= SegmentCapacities.Length || startIndex >= inv.Count) return false;

        if (!CanInsertIntoSegment(null, slot.Itemstack!)) return false;

        if (CountItemsInSegment(startIndex) >= SegmentCapacities[segment]) return false;

        int moved = TryPutIntoSegment(slot, startIndex, byPlayer.Entity.Controls.CtrlKey);
        if (moved <= 0) return false;

        InitMesh();
        MarkDirty();
        (Api as ICoreClientAPI)?.World.Player.TriggerFpAnimation(EnumHandInteract.HeldItemInteract);
        return true;
    }

    protected override int TryPutIntoSegment(ItemSlot slot, int startIndex, bool ctrl) {
        int wanted = ctrl ? slot.StackSize : 1;
        int moved = 0;

        for (int i = 0; i < ItemsPerSegment && moved < wanted && !slot.Empty; i++) {
            ItemSlot target = inv[startIndex + i];
            if (!target.Empty || target.GetRemainingSlotSpace(slot.Itemstack) <= 0)
                continue;

            moved += slot.TryPutIntoBulk(Api.World, target, 1);
        }

        return moved;
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
        if (slot.Empty) return base.getMeshCacheKey(slot);

        DisplayableAttributes? attr = GetDisplayProps(slot);
        if (attr?.Shape != null) return $"{slot.Itemstack!.Collectible.Code}-{DisplayType}-{attr.Shape}";

        return base.getMeshCacheKey(slot);
    }

    protected override MeshData getOrCreateMesh(ItemSlot slot, int index) {
        MeshData? mesh = getMesh(slot);
        if (mesh != null) return mesh;
        if (slot.Empty) return base.getOrCreateMesh(slot, index);

        mesh = GenDisplayShapeMesh(slot);
        if (mesh == null) return base.getOrCreateMesh(slot, index);

        MeshCache[getMeshCacheKey(slot)] = mesh;
        return mesh;
    }

    private MeshData? GenDisplayShapeMesh(ItemSlot slot) {
        DisplayableAttributes? attr = GetDisplayProps(slot);
        if (attr?.Shape == null) return null;

        ItemStack stack = slot.Itemstack!;
        CompositeShape shapeLoc = attr.Shape;

        MeshData mesh = capi.TesselatorManager.CreateMesh("displayed item shape", shapeLoc, (shape, name) => {
            var textures = new Dictionary<string, AssetLocation>(shape.Textures);
            var collTextures = stack.Class == EnumItemClass.Item ? stack.Item.Textures : stack.Block.Textures;
            foreach (var val in collTextures) {
                textures[val.Key] = val.Value.Base;
            }

            return new ContainedTextureSource(capi, capi.BlockTextureAtlas, textures, $"For displayed item {stack.Collectible.Code}");
        },
            null
        );

        if (attr.Transform != null) {
            attr.Transform.EnsureDefaultValues();
            mesh.ModelTransform(attr.Transform);
        }

        return mesh;
    }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            if (td.segment == 2) {
                if (td.item % 2 != 0) { // Compress Bandages to 2:1 mesh ratio
                    td.hidden = true;
                    return;
                }

                td.scaleX =  0.8f;
                td.scaleY = 0.75f;
                td.scaleZ = 0.685f;

                td.offsetX = -0.4325f;
                td.offsetY = 0.775f;
                td.offsetZ = -0.3325f;

                td.offsetRotZ = -90;

                td.x = td.item % 16 * 0.05375f;
                td.z = td.item / 16 * 0.2f;

                return;
            }

            td.scaleX = td.scaleZ = 0.685f;
            td.scaleY = 0.5f;

            td.offsetX = -0.225f;
            td.offsetY = 0.0625f;
            td.offsetZ = -0.225f;

            td.x = td.segment == 1 ? 0.46f : 0;
            td.y = td.item * 0.03125f;
            td.offsetRotY = StaticRandom(td.index, 10);
        });
    }
}
