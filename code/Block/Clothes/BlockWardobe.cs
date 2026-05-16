namespace PurposefulStorage;

public class BlockWardrobe : BasePSContainer, IMultiBlockColSelBoxes {
    private WorldInteraction[]? footwareInteractions;
    private WorldInteraction[]? upperbodywareInteractions;
    private WorldInteraction[]? doorOpenClose;

    public readonly AssetLocation soundWardrobeOpen = new(SoundReferences.WardrobeOpen);
    public readonly AssetLocation soundWardrobeClose = new(SoundReferences.WardrobeClose);

    private static readonly Cuboidf Skip = new(); // Skip selectionBox, to keep consistency between selectionBox indexes

    public override void OnLoaded(ICoreAPI api) {
        base.OnLoaded(api);

        doorOpenClose = ObjectCacheUtil.GetOrCreate(api, "wardrobeInteractions", () => {
            return new WorldInteraction[] {
                new() {
                    ActionLangCode = "blockhelp-door-openclose",
                    MouseButton = EnumMouseButton.Right
                }
            };
        });

        footwareInteractions = ObjectCacheUtil.GetOrCreate(api, "footwareInteractions", () => {
            List<ItemStack> clothesStackList = [];

            foreach (var obj in api.World.Collectibles) {
                if (obj.CanStoreInSlot("psFootware")) {
                    clothesStackList.Add(new ItemStack(obj));
                }
            }

            return new WorldInteraction[] {
                new() {
                    ActionLangCode = "blockhelp-groundstorage-add",
                    MouseButton = EnumMouseButton.Right,
                    Itemstacks = [.. clothesStackList]
                },
                new() {
                    ActionLangCode = "blockhelp-groundstorage-remove",
                    MouseButton = EnumMouseButton.Right,
                }
            };
        });

        upperbodywareInteractions = ObjectCacheUtil.GetOrCreate(api, "upperbodywareInteractions", () => {
            List<ItemStack> clothesStackList = [];

            foreach (var obj in api.World.Collectibles) {
                if (obj.CanStoreInSlot("psUpperbodyware")) {
                    clothesStackList.Add(new ItemStack(obj));
                }
            }

            return new WorldInteraction[] {
                new() {
                    ActionLangCode = "blockhelp-groundstorage-add",
                    MouseButton = EnumMouseButton.Right,
                    Itemstacks = [.. clothesStackList]
                },
                new() {
                    ActionLangCode = "blockhelp-groundstorage-remove",
                    MouseButton = EnumMouseButton.Right,
                }
            };
        });
    }

    public override WorldInteraction[]? GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer) {
        if (world.BlockAccessor.GetBlockEntity(selection.Position) is BEWardrobe bew) {
            if (selection.SelectionBoxIndex < 7) {
                return footwareInteractions;
            }

            if (selection.SelectionBoxIndex < 16) {
                return upperbodywareInteractions;
            }

            return doorOpenClose;
        }

        return null;
    }

    #region MBColSelBoxes

    // Selection box for master block
    public override Cuboidf[] GetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos) {
        BEWardrobe? be = blockAccessor.GetBlockEntityExt<BEWardrobe>(pos);
        var boxes = base.GetSelectionBoxes(blockAccessor, pos);

        if (be == null) return boxes;

        Cuboidf wardrobeSelBox = boxes[18].Clone();

        if (be.DoorOpen) {
            int[] selBoxIndexes = [0, 1, 3, 4, 6, 7, 8, 9, 10, 16];
            List<Cuboidf> selBoxes = [];

            foreach (int index in selBoxIndexes) {
                selBoxes.Add(boxes[index].Clone());
            }

            return [selBoxes[0], selBoxes[1], Skip, selBoxes[2], selBoxes[3], Skip, selBoxes[4], selBoxes[5], selBoxes[6], selBoxes[7], selBoxes[8], Skip, Skip, Skip, Skip, Skip, selBoxes[9]];
        }

        return [Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, wardrobeSelBox];
    }

    // Selection boxes for multiblock parts
    public Cuboidf[] MBGetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset) {
        // Selection Box indexes:
        // Shoes - 0-5
        // Upperbody - 6-15
        // LDoor - 16 / RDoor - 17
        // Wardrobe - 18

        BEWardrobe? be = blockAccessor.GetBlockEntityExt<BEWardrobe>(pos);
        var boxes = base.GetSelectionBoxes(blockAccessor, pos);

        if (be == null) return boxes;

        if (be.DoorOpen) {
            List<Cuboidf> sBs = [];

            for (int i = 0; i < boxes.Length; i++) {
                sBs.Add(boxes[i].Clone());
                sBs[i].MBNormalizeSelectionBox(offset);
            }

            if (offset.Y != 0) {
                return [Skip, Skip, Skip, Skip, Skip, Skip, sBs[6], sBs[7], sBs[8], sBs[9], sBs[10], sBs[11], sBs[12], sBs[13], sBs[14], sBs[15], sBs[16], sBs[17]];
            }

            return [sBs[0], sBs[1], sBs[2], sBs[3], sBs[4], sBs[5], sBs[6], sBs[7], sBs[8], sBs[9], sBs[10], sBs[11], sBs[12], sBs[13], sBs[14], sBs[15], sBs[16], sBs[17]];
        }

        Cuboidf wardrobeSelBox = boxes[18].Clone();
        wardrobeSelBox.MBNormalizeSelectionBox(offset);

        return [Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, wardrobeSelBox];
    }

    public Cuboidf[] MBGetCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset) {
        int rot = this.GetRotationAngle();

        bool collides = (BlockDirection)rot switch {
            BlockDirection.North => offset.Z == 0,
            BlockDirection.West => offset.X == 0,
            BlockDirection.South => offset.Z == 0,
            BlockDirection.East => offset.X == 0,
            _ => offset.Z == 0
        };

        if (!collides) return [];

        return [new Cuboidf(0, 0, 0, 1, 1, 1)];
    }

    #endregion
}