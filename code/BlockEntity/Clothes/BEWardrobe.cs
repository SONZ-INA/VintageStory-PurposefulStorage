namespace PurposefulStorage;

public class BEWardrobe : BEBasePSAnimatable {
    protected new BlockWardrobe block;

    protected override string ReferencedShape => 
        block.Variant["type"] == "wooden" 
            ? ShapeReferences.WardrobeWooden 
            : base.ReferencedShape;

    public override int[] SectionSegmentCounts => new[] { 6, 10 };

    [TreeSerializable(false)] public bool WardrobeOpen { get; set; }

    public BEWardrobe() {
        inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (id, inv) => {
            if (id < 6) return new ItemSlotPSUniversal(inv, "psFootware");
            else return new ItemSlotPSUniversal(inv, "psUpperbodyware");
        });
    }

    public override void Initialize(ICoreAPI api) {
        block = api.World.BlockAccessor.GetBlock(Pos) as BlockWardrobe;
        base.Initialize(api);
    }

    public override bool OnInteract(IPlayer byPlayer, BlockSelection blockSel) {
        ItemSlot slot = byPlayer.InventoryManager.ActiveHotbarSlot;

        // Open/Close wardrobe
        if (byPlayer.Entity.Controls.ShiftKey) {
            if (!WardrobeOpen) ToggleWardrobeDoor(true, byPlayer);
            else ToggleWardrobeDoor(false, byPlayer);

            MarkDirty(true);
            return true;
        }

        // Take/Put items
        if (slot.Empty) {
            if (WardrobeOpen) 
                return TryTake(byPlayer, blockSel);

            return false;
        }
        else {
            if (WardrobeOpen) {
                if (slot.CanStoreInSlot("psFootware") || slot.CanStoreInSlot("psUpperbodyware")) {
                    AssetLocation sound = slot.Itemstack?.Block?.Sounds?.Place;

                    if (TryPut(byPlayer, slot, blockSel)) {
                        Api.World.PlaySoundAt(sound ?? new AssetLocation("sounds/player/build"), byPlayer.Entity, byPlayer, true, 16);
                        MarkDirty();
                        return true;
                    }
                }
            }

            (Api as ICoreClientAPI)?.TriggerIngameError(this, "cantplace", Lang.Get("purposefulstorage:This item cannot be placed in this container."));
            return false;
        }
    }

    #region Animation

    protected override void HandleAnimations() {
        if (animUtil != null) {
            if (WardrobeOpen) ToggleWardrobeDoor(true);
            else ToggleWardrobeDoor(false);
        }
    }

    private void ToggleWardrobeDoor(bool open, IPlayer byPlayer = null) {
        if (open) {
            if (animUtil.activeAnimationsByAnimCode.ContainsKey("wardrobeopen") == false) {
                animUtil.StartAnimation(new AnimationMetaData() {
                    Animation = "wardrobeopen",
                    Code = "wardrobeopen",
                    AnimationSpeed = 3f,
                    EaseOutSpeed = 1,
                    EaseInSpeed = 2
                });
            }

            if (byPlayer != null) Api.World.PlaySoundAt(block.soundWardrobeOpen, byPlayer.Entity, byPlayer, true, 16, 0.3f);
        }
        else {
            if (animUtil.activeAnimationsByAnimCode.ContainsKey("wardrobeopen") == true)
                animUtil.StopAnimation("wardrobeopen");
            
            if (byPlayer != null) Api.World.PlaySoundAt(block.soundWardrobeClose, byPlayer.Entity, byPlayer, true, 16, 0.3f);
        }

        WardrobeOpen = open;
    }

    #endregion

    #region Transformation Matrices

    protected override float[][] genTransformationMatrices() {
        float[][] tfMatrices = new float[SlotCount][];
        int currentIndex = 0;

        for (int sectionIndex = 0; sectionIndex < SectionSegmentCounts.Length; sectionIndex++) {
            int segmentsInSection = SectionSegmentCounts[sectionIndex];

            float[][] sectionMatrices = GenerateSectionMatrices(sectionIndex, segmentsInSection);

            for (int i = 0; i < sectionMatrices.Length; i++) {
                tfMatrices[currentIndex] = sectionMatrices[i];
                currentIndex++;
            }
        }

        return tfMatrices;
    }

    private float[][] GenerateSectionMatrices(int sectionIndex, int segmentCount) {
        int itemsInSection = segmentCount * ItemsPerSegment;
        float[][] sectionMatrices = new float[itemsInSection][];

        return sectionIndex switch {
            0 => GenerateMatrices1(segmentCount),
            1 => GenerateMatrices2(segmentCount),
            _ => sectionMatrices,
        };
    }

    private float[][] GenerateMatrices1(int segmentCount) {
        int itemsInSection = segmentCount * ItemsPerSegment;
        float[][] matrices = new float[itemsInSection][];

        for (int segment = 0; segment < segmentCount; segment++) {
            float x = segment < 3 ? -0.17f : 0.25f;
            float y = segment < 3 ? 0.08f : 0.2575f;
            float z = -0.12f + (segment % 3 * 0.62f);

            matrices[segment] = new Matrixf()
                .Translate(0.5f, 0, 0.5f)
                .RotateYDeg(block.Shape.rotateY)
                .RotateYDeg(90)
                .Translate(x - 0.5f, y, z - 0.5f)
                .Values;
        }

        return matrices;
    }

    private float[][] GenerateMatrices2(int segmentCount) {
        int itemsInSection = segmentCount * ItemsPerSegment;
        float[][] matrices = new float[itemsInSection][];

        for (int segment = 0; segment < segmentCount; segment++) {

            float x = -0.75f + (segment * 0.3825f);
            float y = 0.135f;
            float z = -0.04f;

            matrices[segment] = new Matrixf()
                .Translate(0.5f, 0, 0.5f)
                .RotateYDeg(block.Shape.rotateY)
                .Scale(.5f, 1, 1)
                .Translate(x - 0.5f, y, z - 0.5f)
                .Values;
        }

        return matrices;
    }

    #endregion
}
