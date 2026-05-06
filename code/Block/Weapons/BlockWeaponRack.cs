using System.Linq;

namespace PurposefulStorage;

public class BlockWeaponRack : BasePSContainer, IMultiBlockColSelBoxes {
    // Selection box for master block
    public override Cuboidf[] GetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos) {
        return base.GetSelectionBoxes(blockAccessor, pos);
    }

    // Selection boxes for multiblock parts
    public Cuboidf[] MBGetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset) {
        BEWeaponRack? be = blockAccessor.GetBlockEntityExt<BEWeaponRack>(pos);
        var boxes = base.GetSelectionBoxes(blockAccessor, pos);

        if (be == null) return boxes;

        List<Cuboidf> selBoxes = [];

        for (int i = 0; i < 4; i++) {
            selBoxes.Add(base.GetSelectionBoxes(blockAccessor, pos).ElementAt(i).Clone());
            selBoxes[i].MBNormalizeSelectionBox(offset);
        }

        return [.. selBoxes];
    }

    public Cuboidf[] MBGetCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset) {
        return base.GetCollisionBoxes(blockAccessor, pos);
    }
}