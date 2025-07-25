using System.Linq;

namespace PurposefulStorage;

public class BlockGliderMount : BasePSContainer, IMultiBlockColSelBoxes {
    // Selection box for master block
    public override Cuboidf[] GetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos) {
        return base.GetSelectionBoxes(blockAccessor, pos);
    }

    // Selection boxes for multiblock parts
    public Cuboidf[] MBGetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset) {
        BEGliderMount be = blockAccessor.GetBlockEntityExt<BEGliderMount>(pos);
        if (be != null) {
            Cuboidf selBox = base.GetSelectionBoxes(blockAccessor, pos).ElementAt(0).Clone();
            selBox.MBNormalizeSelectionBox(offset);

            return [selBox];
        }

        return base.GetSelectionBoxes(blockAccessor, pos);
    }

    public Cuboidf[] MBGetCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset) {
        return base.GetCollisionBoxes(blockAccessor, pos);
    }
}