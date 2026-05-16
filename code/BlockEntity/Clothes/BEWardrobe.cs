namespace PurposefulStorage;

public class BEWardrobe : BEBasePSAnimatable {
    protected new BlockWardrobe block = null!;

    protected override string ReferencedShape => 
        block.Variant["type"] == "wooden" 
            ? ShapeReferences.WardrobeWooden 
            : base.ReferencedShape;

    public override string AttributeTransformCode => "onLowerbodywareTransform";
    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int[] SectionSegmentCounts => [6, 10];

    private readonly AnimationData DoorOpenAnim = new ("dooropen", 3f);

    [TreeSerializable(false)] public bool DoorOpen { get; set; }

    private enum SlotType {
        Segments = 15,
        LDoor = 16,
        RDoor = 17,
        ClosedWardrobe = 18
    }

    public BEWardrobe() {
        inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (id, inv) => {
            if (id < 6) return new ItemSlotPSUniversal(inv, "psFootware");
            else return new ItemSlotPSUniversal(inv, [ "psUpperbodyware", "psShoulderware", "psLowerbodyware" ]);
        });
    }

    public override void Initialize(ICoreAPI api) {
        block = (api.World.BlockAccessor.GetBlock(Pos) as BlockWardrobe)!;
        base.Initialize(api);
    }

    public override bool OnInteract(IPlayer byPlayer, BlockSelection blockSel, string? overrideAttrCheck = null) {
        ItemSlot slot = byPlayer.InventoryManager.ActiveHotbarSlot;

        // Open/Close wardrobe
        switch ((SlotType)blockSel.SelectionBoxIndex) {
            case SlotType.ClosedWardrobe:
                ToggleDoor(true, byPlayer);
                MarkDirty(true);
                return true;

            case SlotType.LDoor:
            case SlotType.RDoor:
                ToggleDoor(false, byPlayer);
                MarkDirty(true);
                return true;
        }

        // Take/Put items
        if (slot.Empty) {
            if (DoorOpen) {
                return TryTake(byPlayer, blockSel);
            }
        }
        else {
            if (DoorOpen) {
                if (slot.CanStoreInSlot([ "psFootware", "psUpperbodyware", "psShoulderware", "psLowerbodyware" ])) {
                    if (TryPut(byPlayer, slot, blockSel)) {
                        return this.HandlePlacementEffects(slot.Itemstack, byPlayer);
                    }
                }
            }

            (Api as ICoreClientAPI)?.TriggerIngameError(this, "cantplace", Lang.Get("purposefulstorage:This item cannot be placed in this container."));
        }

        return false;
    }

    #region Animation

    protected override void HandleAnimations() {
        if (AnimUtil != null) {
            if (DoorOpen) ToggleDoor(true);
            else ToggleDoor(false);
        }
    }

    private void ToggleDoor(bool open, IPlayer? byPlayer = null) {
        if (open) {
            AnimUtil.TryStartAnimation(DoorOpenAnim.Code, DoorOpenAnim.Speed);

            if (byPlayer != null) {
                Api.World.PlaySoundAt(block.soundWardrobeOpen, byPlayer.Entity, byPlayer, true, 16, 0.3f);
            }
        }
        else {
            AnimUtil.TryStopAnimation(DoorOpenAnim.Code);
            
            if (byPlayer != null) Api.World.PlaySoundAt(block.soundWardrobeClose, byPlayer.Entity, byPlayer, true, 16, 0.3f);
        }

        DoorOpen = open;
    }

    #endregion

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            if (td.section == 0) {
                td.x = td.segment % 3 * 0.625f;
                td.y = td.segment / 3 * 0.1875f;
                td.z = td.segment / 3 * -0.35f;

                td.offsetX = -0.125f;
                td.offsetY = 0.0675f;
                td.offsetZ = 0.15f;

                td.offsetRotY = 90;
            }

            if (td.section == 1) {
                td.x = td.segment * 0.1875f;

                td.offsetX = -0.35f;
                td.offsetY = 0.13f;
                td.offsetZ = -0.035f;

                td.scaleX = 0.5f;
            }
        });
    }
}
