using System.Linq;

namespace PurposefulStorage;

public class BlockHatRack : BasePSContainer, IMultiBlockColSelBoxes {
    // Selection box for master block
    public override Cuboidf[] GetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos) {
        return base.GetSelectionBoxes(blockAccessor, pos);
    }

    // Selection boxes for multiblock parts
    public Cuboidf[] MBGetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset) {
        Cuboidf skip = new(); // Skip selectionBox, to keep consistency between selectionBox indexes

        BEHatRack be = blockAccessor.GetBlockEntityExt<BEHatRack>(pos);
        if (be != null) {
            List<Cuboidf> selBoxes = [];

            for (int i = 4; i < 8; i++) {
                selBoxes.Add(base.GetSelectionBoxes(blockAccessor, pos).ElementAt(i).Clone());
                selBoxes[i - 4].MBNormalizeSelectionBox(offset);
            }

            return [skip, skip, skip, skip, selBoxes[0], selBoxes[1], selBoxes[2], selBoxes[3]];
        }

        return base.GetSelectionBoxes(blockAccessor, pos);
    }

    public Cuboidf[] MBGetCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset) {
        return base.GetCollisionBoxes(blockAccessor, pos);
    }
}