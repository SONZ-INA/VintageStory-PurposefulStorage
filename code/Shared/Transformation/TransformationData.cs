namespace PurposefulStorage;

/// <summary>
/// A hierarchical state container that stores translation, rotation, and scale properties. Its BuildMatrix method handles the complex 3D math by chaining transforms in a specific order. <br />
/// Use offsets for item-specific manipulation!
/// </summary>
public class TransformationData() {
    public int index;
    public int section;
    public int segment;
    public int item;

    public float preRotate = 0;
    public float x, y, z;
    public float offsetX, offsetY, offsetZ;
    public float rotX, rotY, rotZ;
    public float offsetRotX, offsetRotY, offsetRotZ;
    public float scaleX, scaleY, scaleZ;
    public float offsetOriginX, offsetOriginY, offsetOriginZ;

    public bool hidden;

    /// <summary>
    /// Resets all properties to 0, except preRotate.
    /// </summary>
    public void Reset() {
        x = y = z = 0;
        offsetX = offsetY = offsetZ = 0;
        rotX = rotY = rotZ = 0;
        offsetRotX = offsetRotY = offsetRotZ = 0;
        scaleX = scaleY = scaleZ = 1;
        offsetOriginX = offsetOriginY = offsetOriginZ = 0;
        hidden = false;
    }

    /// <summary>
    /// Builds the matrix and returns the float array to be used for the current item.
    /// </summary>
    public float[] BuildMatrix() {
        Matrixf mat = new();

        if (hidden) {
            return mat.Scale(0.01f, 0.01f, 0.01f).Values;
        }

        mat.Translate(0.5f, 0, 0.5f);

        // Handle block rotation
        mat.RotateYDeg(preRotate);

        // Handle segment locations
        mat.Translate(x, y, z);
        mat.Rotate(rotX * GameMath.DEG2RAD, rotY * GameMath.DEG2RAD, rotZ * GameMath.DEG2RAD);

        // Handle item offsets
        mat.Translate(offsetX, offsetY, offsetZ);
        mat.RotateXDeg(offsetRotX);
        mat.RotateYDeg(offsetRotY);
        mat.RotateZDeg(offsetRotZ);
        mat.Translate(offsetOriginX, offsetOriginY, offsetOriginZ);
        mat.Scale(scaleX, scaleY, scaleZ);

        mat.Translate(-0.5f, 0, -0.5f);

        return mat.Values;
    }

    /// <summary>
    /// Safely extracts a ModelTransform from an ItemStack's attributes and applies it to this TransformationData.<br/>
    /// <b>Doesn't apply origin properties!</b>
    /// </summary>
    public void ApplyModelTransform(ItemStack? stack, string attributeTransformCode) {
        if (stack?.Collectible?.Attributes == null || string.IsNullOrEmpty(attributeTransformCode))
            return;

        ModelTransform? modelTransform = stack.Collectible.Attributes[attributeTransformCode]?.AsObject<ModelTransform>();
        if (modelTransform == null)
            return;

        modelTransform.EnsureDefaultValues();

        offsetX += modelTransform.Translation.X;
        offsetY += modelTransform.Translation.Y;
        offsetZ += modelTransform.Translation.Z;

        offsetRotX += modelTransform.Rotation.X;
        offsetRotY += modelTransform.Rotation.Y;
        offsetRotZ += modelTransform.Rotation.Z;

        scaleX *= modelTransform.ScaleXYZ.X;
        scaleY *= modelTransform.ScaleXYZ.Y;
        scaleZ *= modelTransform.ScaleXYZ.Z;
    }
}