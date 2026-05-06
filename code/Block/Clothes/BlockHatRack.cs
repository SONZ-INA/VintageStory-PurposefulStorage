using System.Linq;

namespace PurposefulStorage;

public class BlockHatRack : BasePSContainer, IMultiBlockColSelBoxes {
    private static readonly Cuboidf Skip = new(); // Skip selectionBox, to keep consistency between selectionBox indexes

    // Selection box for master block
    public override Cuboidf[] GetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos) {
        return base.GetSelectionBoxes(blockAccessor, pos);
    }

    // Selection boxes for multiblock parts
    public Cuboidf[] MBGetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset) {
        BEHatRack? be = blockAccessor.GetBlockEntityExt<BEHatRack>(pos);
        var boxes = base.GetSelectionBoxes(blockAccessor, pos);

        if (be == null) return boxes;
        
        List<Cuboidf> selBoxes = [];

        for (int i = 4; i < 8; i++) {
            selBoxes.Add(base.GetSelectionBoxes(blockAccessor, pos).ElementAt(i).Clone());
            selBoxes[i - 4].MBNormalizeSelectionBox(offset);
        }

        return [Skip, Skip, Skip, Skip, selBoxes[0], selBoxes[1], selBoxes[2], selBoxes[3]];
    }

    public Cuboidf[] MBGetCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset) {
        return base.GetCollisionBoxes(blockAccessor, pos);
    }
}