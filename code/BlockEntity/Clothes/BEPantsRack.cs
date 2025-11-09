namespace PurposefulStorage;

public class BEPantsRack : BEBasePSContainer {
    public override string AttributeTransformCode => "onLowerbodywareTransform";
    public override string[] AttributeCheck => ["psLowerbodyware"];

    public override int[] SectionSegmentCounts => [2];
    public override int ItemsPerSegment => 5;

    public BEPantsRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override bool TryPut(IPlayer byPlayer, ItemSlot slot, BlockSelection blockSel) {
        if (slot.Itemstack?.Collectible?.Code.Path.Contains("skirt") == true) // Skirts are blacklisted
            return false;

        return base.TryPut(byPlayer, slot, blockSel);
    }

    public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tesselator) {
        MeshData currentMesh = blockMesh.Clone();

        ItemStack[] stacks = GetContentStacks();
        for (int i = 0; i < stacks.Length; i++) {
            MeshData substituteMeshes = SubstituteItemShapes(Api, tesselator, ShapeReferences.utilPants, stacks[i]);
            if (substituteMeshes != null) currentMesh.AddMeshData(substituteMeshes.MatrixTransform(tfMatrices[i]));
        }

        mesher.AddMeshData(currentMesh);
        return true;
    }

    protected override float[][] genTransformationMatrices() {
        float[][] tfMatrices = new float[SlotCount][];
        
        for (int segment = 0; segment < SectionSegmentCounts[0]; segment++) {
            for (int item = 0; item < ItemsPerSegment; item++) {
                int index = segment * ItemsPerSegment + item;

                float x = 0;
                float y = -0.01f + segment * 0.43f + item * 0.06f;
                float z = -0.02f;

                tfMatrices[index] = new Matrixf()
                    .Translate(0.5f, 0, 0.5f)
                    .RotateYDeg(block.Shape.rotateY)
                    .Scale(0.9f, 1, 1)
                    .Translate(x - 0.5f, y, z - 0.5f)
                    .Values;
            }
        }

        return tfMatrices;
    }
}
