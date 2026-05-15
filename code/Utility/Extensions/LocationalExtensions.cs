namespace PurposefulStorage;

public static class LocationalExtensions {
    /// <summary>
    /// Finds and returns the transformation for the given collectible object from the provided dictionary.<br/>
    /// Used to apply item-specific position/rotation/scale adjustments to avoid clipping or misalignment.
    /// </summary>
    public static ModelTransform? GetTransformation(this CollectibleObject obj, Dictionary<string, ModelTransform> transformations) {
        foreach (KeyValuePair<string, ModelTransform> transformation in transformations) {
            if (WildcardUtil.Match(transformation.Key, obj.Code.ToString())) return transformation.Value;
        }

        return null;
    }

    /// <summary>
    /// Safely gets a value from the array. 
    /// Returns 0f if the array is null, the index is out of bounds, or the value at the index is null.
    /// </summary>
    public static float GetValue(float?[] array, int index) {
        if (index >= array.Length) return 0f;
        return array[index] ?? 0f;
    }

    /// <summary>
    /// Applies the given <see cref="ModelTransform"/> to the matrix, modifying its translation, rotation, and scale.
    /// </summary>
    public static void ApplyModelTransformToMatrixF(this Matrixf mat, ModelTransform? transformation) {
        if (transformation == null) return;

        mat.Translate(0.5f, 0, 0.5f);

        if (!transformation.Translation.Equals(new FastVec3f(0, 0, 0))) {
            mat.Translate(transformation.Translation.X, transformation.Translation.Y, transformation.Translation.Z);
        }

        if (!transformation.Rotation.Equals(new FastVec3f(0, 0, 0))) {
            mat.RotateXDeg(transformation.Rotation.X);
            mat.RotateYDeg(transformation.Rotation.Y);
            mat.RotateZDeg(transformation.Rotation.Z);
        }

        if (!transformation.ScaleXYZ.Equals(new FastVec3f(1f, 1f, 1f))) {
            mat.Scale(transformation.ScaleXYZ.X, transformation.ScaleXYZ.Y, transformation.ScaleXYZ.Z);
        }

        mat.Translate(-0.5f, 0, -0.5f);
    }

    /// <summary>
    /// Vanilla method for applying the AttributeTransformCode transformations per-item.
    /// </summary>
    public static void ApplyDefaultTranforms(this MeshData mesh, ItemStack stack, string attributeTransformCode) {
        ModelTransform? modelTransform = stack.Collectible.Attributes?[attributeTransformCode].AsObject<ModelTransform>();

        if (modelTransform != null) {
            modelTransform.EnsureDefaultValues();
            mesh.ModelTransform(modelTransform);
        }

        if (stack.Class == EnumItemClass.Item && (stack.Item.Shape == null || stack.Item.Shape.VoxelizeTexture)) {
            mesh.Rotate(MathF.PI / 2f, 0f, 0f);
            mesh.Scale(0.33f, 0.33f, 0.33f);
            mesh.Translate(0f, -15f / 32f, 0f);
        }
    }

    /// <summary>
    /// Calculates and returns the horizontal rotation angle of a block relative to the player’s position,
    /// rounding it to the nearest 22.5 degrees.<br/> Useful for meshing blocks that can rotate freely (e.g., buckets/baskets).
    /// </summary>
    public static float GetBlockMeshAngle(IPlayer byPlayer, BlockSelection blockSel, bool val) {
        if (!val) return 0;

        BlockPos targetPos = blockSel.DidOffset ? blockSel.Position.AddCopy(blockSel.Face.Opposite) : blockSel.Position;
        double dx = byPlayer.Entity.Pos.X - (targetPos.X + blockSel.HitPosition.X);
        double dz = byPlayer.Entity.Pos.Z - (targetPos.Z + blockSel.HitPosition.Z);
        float angleHor = (float)Math.Atan2(dx, dz);

        float deg22dot5rad = GameMath.PIHALF / 4;
        float roundRad = ((int)Math.Round(angleHor / deg22dot5rad)) * deg22dot5rad;
        return roundRad;
    }

    /// <summary>
    /// Returns the rotation angle in degrees for the block based on its variant suffix (e.g., "-north", "-south").<br/>
    /// For this method to work properly, it *must* align with the rotation defined in the block's type.
    /// </summary>
    public static int GetRotationAngle(this Block? block) {
        string blockPath = block?.Code.Path ?? "";

        return blockPath switch {
            var path when path.EndsWith("-north") => 0,
            var path when path.EndsWith("-west") => 90,
            var path when path.EndsWith("-south") => 180,
            var path when path.EndsWith("-east") => 270,
            _ => 0
        };
    }
}
