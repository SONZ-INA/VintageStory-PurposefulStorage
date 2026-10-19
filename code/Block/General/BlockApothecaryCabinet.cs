namespace PurposefulStorage;

public class BlockApothecaryCabinet : BasePSContainer {
    private WorldInteraction[]? doorOpenClose;
    private WorldInteraction[]? poulticeInteractions;
    private WorldInteraction[]? bandageInteractions;

    public readonly AssetLocation soundCabinetOpen = new(SoundReferences.ApothecaryOpen);
    public readonly AssetLocation soundCabinetClose = new(SoundReferences.ApothecaryClose);

    private static readonly Cuboidf Skip = new(); // Skip selectionBox, to keep consistency between selectionBox indexes

    public override void OnLoaded(ICoreAPI api) {
        base.OnLoaded(api);

        doorOpenClose = ObjectCacheUtil.GetOrCreate(api, "cabinetInteractions", () => {
            return new WorldInteraction[] {
                new() {
                    ActionLangCode = "blockhelp-door-openclose",
                    MouseButton = EnumMouseButton.Right
                }
            };
        });

        poulticeInteractions = ObjectCacheUtil.GetOrCreate(api, "poulticeInteractions", () => {
            List<ItemStack> stackList = [];

            foreach (var obj in api.World.Collectibles) {
                if (obj.CanStoreInSlot("psPoultices")) {
                    stackList.Add(new ItemStack(obj));
                }
            }

            var stackArray = stackList.ToArray();

            return new WorldInteraction[] {
                new() {
                    ActionLangCode = "blockhelp-groundstorage-add",
                    MouseButton = EnumMouseButton.Right,
                    Itemstacks = stackArray
                },
                new() {
                    ActionLangCode = "blockhelp-groundstorage-addbulk",
                    MouseButton = EnumMouseButton.Right,
                    HotKeyCode = "ctrl",
                    Itemstacks = stackArray
                },
                new() {
                    ActionLangCode = "blockhelp-groundstorage-remove",
                    MouseButton = EnumMouseButton.Right,
                    RequireFreeHand = true
                }
            };
        });

        bandageInteractions = ObjectCacheUtil.GetOrCreate(api, "bandageInteractions", () => {
            List<ItemStack> stackList = [];

            foreach (var obj in api.World.Collectibles) {
                if (obj.CanStoreInSlot("psBandages")) {
                    stackList.Add(new ItemStack(obj));
                }
            }

            var stackArray = stackList.ToArray();

            return new WorldInteraction[] {
                new() {
                    ActionLangCode = "blockhelp-groundstorage-add",
                    MouseButton = EnumMouseButton.Right,
                    Itemstacks = stackArray
                },
                new() {
                    ActionLangCode = "blockhelp-groundstorage-addbulk",
                    MouseButton = EnumMouseButton.Right,
                    HotKeyCode = "ctrl",
                    Itemstacks = stackArray
                },
                new() {
                    ActionLangCode = "blockhelp-groundstorage-remove",
                    MouseButton = EnumMouseButton.Right,
                    RequireFreeHand = true
                }
            };
        });
    }

    public override WorldInteraction[]? GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer) {
        if (world.BlockAccessor.GetBlockEntity(selection.Position) is BEApothecaryCabinet beac) {
            int index = selection.SelectionBoxIndex;

            if (index >= 3 && index < 6) return doorOpenClose;

            if (index >= 0 && index < 3) {
                if (!beac.IsDoorOpen(index)) return doorOpenClose;
                return index == 2 ? bandageInteractions : poulticeInteractions;
            }
        }

        return base.GetPlacedBlockInteractionHelp(world, selection, forPlayer);
    }

    public override Cuboidf[] GetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos) {
        Cuboidf[] boxes = base.GetSelectionBoxes(blockAccessor, pos);

        if (blockAccessor.GetBlockEntity(pos) is not BEApothecaryCabinet beac)
            return boxes;

        Cuboidf[] sBs = new Cuboidf[boxes.Length];
        for (int i = 0; i < boxes.Length; i++) {
            bool isDoorBox = i >= 3 && i < 6;
            sBs[i] = isDoorBox && !beac.IsDoorOpen(i - 3) ? Skip : boxes[i];
        }

        return sBs;
    }
}