using System.Collections.Concurrent;
using System.Linq;

namespace PurposefulStorage;

public static class MeshExtensions {
    private static readonly ConcurrentDictionary<string, int> stackedShapeCounts = new();

    /// <summary>
    /// Rotates the mesh around the Y-axis based on the block's predefined <c>rotateY</c> value.<br/>
    /// Useful for aligning meshes with the block's in-world orientation.
    /// </summary>
    public static MeshData? BlockYRotation(this MeshData? mesh, Block? block)
        => mesh?.Rotate(new Vec3f(0.5f, 0.5f, 0.5f), 0, (block?.Shape.rotateY ?? 0) * GameMath.DEG2RAD, 0);

    /// <summary>
    /// Returns the shelf displayed shape defined in the attributes of an Item.
    /// </summary>
    public static string? GetDisplayedShape(this ItemStack stack)
        => stack.ItemAttributes?["displayable"]?["shelf"]?["shape"]?["base"]?.AsString();

    /// <summary>
    /// Returns how many root shapes should be visible for the given stack size.
    /// </summary>
    public static int GetVisibleShapeCount(int content, int capacity, int shapeCount) {
        if (content <= 0 || capacity <= 0 || shapeCount <= 0) return 0;
        content = Math.Min(content, capacity);
        return (content * shapeCount + capacity - 1) / capacity;
    }

    /// <summary>
    /// Number of root shapes in the resolved shape file (cached). 0 if not found.
    /// </summary>
    public static int GetStackedShapeCount(ICoreClientAPI? capi, ItemStack? stack, string shapePath) {
        if (capi == null || stack?.Collectible == null || string.IsNullOrEmpty(shapePath)) return 0;

        AssetLocation loc = ResolveStackedShapeLocation(shapePath, stack);
        string key = loc.ToString();

        if (stackedShapeCounts.TryGetValue(key, out int count)) return count;

        count = Shape.TryGet(capi, loc)?.Elements?.Length ?? 0;
        if (count > 0) stackedShapeCounts[key] = count; // don't cache misses
        return count;
    }

    /// <summary>
    /// How many root shapes GenStackedShapeMesh would render for this stack. Use for cache keys.
    /// </summary>
    public static int GetStackedVisibleCount(ICoreClientAPI? capi, ItemStack? stack, string shapePath, int capacity) {
        if (stack == null) return 0;
        return GetVisibleShapeCount(stack.StackSize, capacity, GetStackedShapeCount(capi, stack, shapePath));
    }

    /// <summary>
    /// Resolves a direct file path or builds one from the folder and item code path for the location of stacked shapes.<br/>
    /// Used for GenStackedShapeMesh method.
    /// </summary>
    public static AssetLocation ResolveStackedShapeLocation(string source, ItemStack stack) {
        AssetLocation loc = source.Contains(':')
            ? new AssetLocation(source)
            : new AssetLocation("purposefulstorage", $"shapes/stacks/{source}");

        if (!source.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) {
            loc = new AssetLocation(loc.Domain, $"{loc.Path.TrimEnd('/')}/{stack.Collectible.Code.Path}");
        }

        return loc.WithPathPrefixOnce("shapes/").WithPathAppendixOnce(".json");
    }

    /// <summary>
    /// Updates the texture key for all faces in the shape’s root element and its children.
    /// </summary>
    public static void ChangeTextureKey(this Shape shape, string key) {
        foreach (var face in shape.Elements[0].FacesResolved!) {
            face.Texture = key;
        }

        foreach (var child in shape.Elements[0].Children!) {
            foreach (var face in child.FacesResolved!) {
                face?.Texture = key;
            }
        }
    }

    /// <summary>
    /// Replaces the texture key of all resolved faces in the first <see cref="ShapeElement"/> and its child elements within the given <see cref="Shape"/>.
    /// </summary>
    public static void ChangeShapeTextureKey(this Shape shape, string key) {
        foreach (var face in shape.Elements[0].FacesResolved!) {
            face.Texture = key;
        }

        foreach (var child in shape.Elements[0].Children!) {
            foreach (var face in child.FacesResolved!) {
                face?.Texture = key;
            }
        }
    }

    /// <summary>
    /// Recursively removes elements and their children whose names are in skipElements.
    /// </summary>
    public static ShapeElement[] RemoveElements(ShapeElement[] elementArray, string?[] skipElements) {
        var remainingElements = elementArray.Where(e => !skipElements.Contains(e.Name)).ToArray();
        foreach (var element in remainingElements) {
            if (element.Children != null && element.Children.Length > 0) {
                element.Children = RemoveElements(element.Children, skipElements); // Recursively filter children
            }
        }

        return remainingElements;
    }

    /// <summary>
    /// If the shape file doesn't have any textures defined, transfers the textures from the itemtype itself.
    /// </summary>
    public static void TransferItemtypeTextures(this Shape shape, ItemStack stack) {
        if (stack.Item == null && stack.Block == null) 
            return;

        if (shape.Textures.Count != 0)
            return;

        var collectibleTextures = stack.Item?.Textures ?? stack.Block?.Textures;
        if (collectibleTextures == null) return;

        foreach (var texture in collectibleTextures) {
            shape.Textures.Add(texture.Key, texture.Value.Base);
        }
    }

    /// <summary>
    /// Returns a pie texture source based on the 'inPieProperties' attribute.
    /// </summary>
    public static ITexPositionSource? GetPieTexture(ICoreClientAPI capi, ItemStack? stack, Shape? shape) {
        if (capi == null || shape == null || stack == null)
            return null;

        var pieProps = stack?.ItemAttributes?["inPieProperties"];
        if (pieProps?.Exists != true)
            return null;

        var texturePath = pieProps["texture"]?.ToString();

        if (string.IsNullOrEmpty(texturePath))
            return null;

        var textureLoc = new AssetLocation(texturePath);

        // Apply to shape
        shape.Textures.Clear();
        shape.Textures["surface"] = textureLoc;

        return new ShapeTextureSource(capi, shape, "PS-LiquidyTextureSource");
    }

    /// <summary>
    /// Returns a texture source defined by the item's 'inContainerTexture' attribute.
    /// </summary>
    public static ITexPositionSource? GetContainerTextureSource(ICoreClientAPI capi, ItemStack? stack) {
        if (capi == null || stack == null)
            return null;

        var texAttr = stack?.ItemAttributes?["inContainerTexture"];
        if (texAttr?.Exists != true)
            return null;

        var texture = texAttr.AsObject<CompositeTexture>();
        return new ContainerTextureSource(capi, stack, texture);
    }

    /// <summary>
    /// Returns a texture source using the item's first available texture.
    /// </summary>
    public static ITexPositionSource? GetItemTextureSource(ICoreClientAPI capi, ItemStack? stack) {
        if (capi == null || stack == null)
            return null;

        var firstTexture = stack?.Item?.Textures?.Values?.FirstOrDefault();
        if (firstTexture == null)
            return null;

        return new ContainerTextureSource(capi, stack, firstTexture);
    }

    /// <summary>
    /// Returns the Fill Height of utilCube content height. Used for the GenPartialContentMesh() method.
    /// </summary>
    public static float GetFillHeight(float content, float capacity, float maxHeight) {
        if (capacity <= 0) return 0;
        return maxHeight * GameMath.Clamp(content / capacity, 0f, 1f);
    }
}
