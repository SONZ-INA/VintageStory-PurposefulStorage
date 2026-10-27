namespace PurposefulStorage;

public class BlockPeltRack : BasePSContainer, IMultiBlockColSelBoxes {
    // Selection box for master block
    public override Cuboidf[] GetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos) {
        return base.GetSelectionBoxes(blockAccessor, pos);
    }

    // Selection boxes for multiblock parts
    public Cuboidf[] MBGetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset) {
        BEPeltRack? be = blockAccessor.GetBlockEntityExt<BEPeltRack>(pos);
        var boxes = base.GetSelectionBoxes(blockAccessor, pos);

        if (be == null) return boxes;

        List<Cuboidf> sbs = [];
        for (int i = 0; i < 27; i++) {
            var sb = boxes[i].Clone();
            sb.MBNormalizeSelectionBox(offset);
            sbs.Add(sb);
        }

        return [..sbs];
    }

    public Cuboidf[] MBGetCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset) {
        return base.GetCollisionBoxes(blockAccessor, pos);
    }
}