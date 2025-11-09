using System.Linq;

namespace PurposefulStorage;

public static class MeshExtensions {
    /// <summary>
    /// Rotates the mesh around the Y-axis based on the block's predefined <c>rotateY</c> value.<br/>
    /// Useful for aligning meshes with the block's in-world orientation.
    /// </summary>
    public static MeshData BlockYRotation(this MeshData mesh, Block block)
        => mesh?.Rotate(new Vec3f(0.5f, 0.5f, 0.5f), 0, block.Shape.rotateY * GameMath.DEG2RAD, 0);

    /// <summary>
    /// Updates the texture key for all faces in the shape’s root element and its children.
    /// </summary>
    public static void ChangeTextureKey(this Shape shape, string key) {
        foreach (var face in shape.Elements[0].FacesResolved) {
            face.Texture = key;
        }

        foreach (var child in shape.Elements[0].Children) {
            foreach (var face in child.FacesResolved) {
                if (face != null) face.Texture = key;
            }
        }
    }

    /// <summary>
    /// Recursively removes elements and their children whose names are in skipElements.
    /// </summary>
    public static ShapeElement[] RemoveElements(ShapeElement[] elementArray, string[] skipElements) {
        var remainingElements = elementArray.Where(e => !skipElements.Contains(e.Name)).ToArray();
        foreach (var element in remainingElements) {
            if (element.Children != null && element.Children.Length > 0) {
                element.Children = RemoveElements(element.Children, skipElements); // Recursively filter children
            }
        }

        return remainingElements;
    }
}
