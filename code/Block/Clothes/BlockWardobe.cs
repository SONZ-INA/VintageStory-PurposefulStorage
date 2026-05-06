using System.Linq;

namespace PurposefulStorage;

public class BlockWardrobe : BasePSContainer, IMultiBlockColSelBoxes {
    private WorldInteraction[]? itemSlottableInteractions;
    private WorldInteraction[]? wardrobeInteractions;

    public readonly AssetLocation soundWardrobeOpen = new(SoundReferences.WardrobeOpen);
    public readonly AssetLocation soundWardrobeClose = new(SoundReferences.WardrobeClose);

    private static readonly Cuboidf Skip = new(); // Skip selectionBox, to keep consistency between selectionBox indexes

    public override void OnLoaded(ICoreAPI api) {
        base.OnLoaded(api);

        itemSlottableInteractions = ObjectCacheUtil.GetOrCreate(api, "wardrobeItemInteractions", () => {
            List<ItemStack> clothesStackList = [];

            foreach (var obj in api.World.Collectibles) {
                if (obj.CanStoreInSlot("psFootware") || obj.CanStoreInSlot("psUpperbody")) {
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

        wardrobeInteractions = ObjectCacheUtil.GetOrCreate(api, "wardrobeInteractions", () => {
            return new WorldInteraction[] {
                new() {
                    ActionLangCode = "blockhelp-door-openclose",
                    MouseButton = EnumMouseButton.Right,
                    HotKeyCode = "shift",
                }
            };
        });
    }

    public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer) {
        if (selection.SelectionBoxIndex == 16) return wardrobeInteractions.Append(base.GetPlacedBlockInteractionHelp(world, selection, forPlayer));
        else return wardrobeInteractions.Append(itemSlottableInteractions.Append(base.GetPlacedBlockInteractionHelp(world, selection, forPlayer)));
    }

    #region MBColSelBoxes

    // Selection box for master block
    public override Cuboidf[] GetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos) {
        BEWardrobe? be = blockAccessor.GetBlockEntityExt<BEWardrobe>(pos);
        var boxes = base.GetSelectionBoxes(blockAccessor, pos);

        if (be == null) return boxes;

        Cuboidf wardrobeSelBox = base.GetSelectionBoxes(blockAccessor, pos).ElementAt(16).Clone();

        if (be.WardrobeOpen) {
            int[] selBoxIndexes = [0, 1, 3, 4, 6, 7, 8, 9, 10];
            List<Cuboidf> selBoxes = [];

            foreach (int index in selBoxIndexes) {
                selBoxes.Add(base.GetSelectionBoxes(blockAccessor, pos).ElementAt(index).Clone());
            }

            return [selBoxes[0], selBoxes[1], Skip, selBoxes[2], selBoxes[3], Skip, selBoxes[4], selBoxes[5], selBoxes[6], selBoxes[7], selBoxes[8]];
        }

        return [Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, wardrobeSelBox];
    }

    // Selection boxes for multiblock parts
    public Cuboidf[] MBGetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset) {
        // Selection Box indexes:
        // Shoes - 0-5
        // Upperbody - 6-15
        // Door - 16

        BEWardrobe? be = blockAccessor.GetBlockEntityExt<BEWardrobe>(pos);

        if (be?.WardrobeOpen == true) {
            List<Cuboidf> sBs = [];
            
            for (int i = 1; i < 16; i++) {
                if (i == 3) continue;
                sBs.Add(base.GetSelectionBoxes(blockAccessor, pos).ElementAt(i).Clone());
            }

            foreach (var selBox in sBs) {
                selBox.MBNormalizeSelectionBox(offset);
            }

            if (offset.Y != 0) {
                return [Skip, Skip, Skip, Skip, Skip, Skip, sBs[4], sBs[5], sBs[6], sBs[7], sBs[8], sBs[9], sBs[10], sBs[11], sBs[12], sBs[13]];
            }

            return [Skip, sBs[0], sBs[1], Skip, sBs[2], sBs[3], Skip, Skip, Skip, Skip, Skip, sBs[9], sBs[10], sBs[11], sBs[12], sBs[13]];
        }

        Cuboidf wardrobeSelBox = base.GetSelectionBoxes(blockAccessor, pos)[16].Clone();
        wardrobeSelBox.MBNormalizeSelectionBox(offset);

        return [Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, Skip, wardrobeSelBox];
    }

    public Cuboidf[] MBGetCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset) {
        return base.GetCollisionBoxes(blockAccessor, pos);
    }

    #endregion
}