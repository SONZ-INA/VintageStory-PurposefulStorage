using System.Linq;

namespace PurposefulStorage;

public class BlockJonasPartsContainer : BasePSContainer, IMultiBlockColSelBoxes {
    private static readonly Cuboidf Skip = new(); // Skip selectionBox, to keep consistency between selectionBox indexes

    // Selection box for master block
    public override Cuboidf[] GetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos) {
        BEJonasPartsCabinet? be = blockAccessor.GetBlockEntityExt<BEJonasPartsCabinet>(pos);
        var boxes = base.GetSelectionBoxes(blockAccessor, pos);

        if (be == null) return boxes;

        return boxes[..7];
    }

    // Selection boxes for multiblock parts
    public Cuboidf[] MBGetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset) {
        BEJonasPartsCabinet? be = blockAccessor.GetBlockEntityExt<BEJonasPartsCabinet>(pos);
        var boxes = base.GetSelectionBoxes(blockAccessor, pos);

        if (be == null) return boxes;

        if (offset.X is 1 or -1 || offset.Z is 1 or -1) {
            Cuboidf[] selectionBoxes = [.. Enumerable.Repeat(Skip, 10)];

            for (int i = 4; i < 9; i++) {
                Cuboidf selBox = boxes[i].Clone();
                selBox.MBNormalizeSelectionBox(offset);
                selectionBoxes[i] = selBox;
            }

            return selectionBoxes;
        }

        if (offset.X is 2 or -2 || offset.Z is 2 or -2) {
            Cuboidf[] selectionBoxes = [.. Enumerable.Repeat(Skip, 13)];

            for (int i = 7; i < 13; i++) {
                Cuboidf selBox = boxes[i].Clone();
                selBox.MBNormalizeSelectionBox(offset);
                selectionBoxes[i] = selBox;
            }

            return selectionBoxes;
        }

        return boxes;
    }

    public Cuboidf[] MBGetCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset) {
        return base.GetCollisionBoxes(blockAccessor, pos);
    }
}