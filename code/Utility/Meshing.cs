using System.Linq;

namespace PurposefulStorage;

public static class Meshing {
    /// <summary>
    /// Generates a mesh for a block that has (or doesn't have) any attributes set to get textures from. Mainly used for many plank textures.
    /// skipElements is used to remove specific cubes from the model (recursive) before meshing.
    /// </summary>
    public static MeshData GenBlockVariantMesh(ICoreAPI api, ItemStack stackWithAttributes, string[] skipElements = null) {
        if (api is not ICoreClientAPI capi) return null;

        Block block = stackWithAttributes.Block;
        var variantData = GetBlockVariantData(capi, stackWithAttributes);

        if (skipElements?.Length > 0) {
            ShapeElement[] RemoveElements(ShapeElement[] elementArray) {
                var remainingElements = elementArray.Where(e => !skipElements.Contains(e.Name)).ToArray();
                foreach (var element in remainingElements) {
                    if (element.Children != null && element.Children.Length > 0) {
                        element.Children = RemoveElements(element.Children); // Recursively filter children
                    }
                }
                return remainingElements;
            }

            variantData.Item1.Elements = RemoveElements(variantData.Item1.Elements);
        }

        capi.Tesselator.TesselateShape("PS-TesselateShape", variantData.Item1, out MeshData blockMesh, variantData.Item2);

        float scale = block.Shape.Scale;
        if (scale != 1) blockMesh.Scale(new Vec3f(.5f, 0, .5f), scale, scale, scale);

        return blockMesh.BlockYRotation(block);
    }

    /// <summary>
    /// Changes the block shape to another shape. Please note that the textures should be the same for the substitute shape.
    /// </summary>
    public static MeshData SubstituteBlockShape(ICoreAPI Api, ITesselatorAPI tesselator, string shapePath, Block texturesFromBlock) {
        AssetLocation shapeLocation = new(shapePath);
        ITexPositionSource texSource = tesselator.GetTextureSource(texturesFromBlock);
        Shape shape = Api.Assets.TryGet(shapeLocation)?.ToObject<Shape>();
        if (shape == null) return null;

        tesselator.TesselateShape(null, shape, out MeshData mesh, texSource);
        return mesh;
    }

    /// <summary>
    /// Changes the item shape to another shape. Please note that the textures should be the same for the substitute shape.
    /// </summary>
    public static MeshData SubstituteItemShape(ICoreAPI Api, ITesselatorAPI tesselator, string shapePath) {
        if (Api is not ICoreClientAPI capi) return null;

        AssetLocation shapeLocation = new(shapePath);
        Shape shape = Api.Assets.TryGet(shapeLocation)?.ToObject<Shape>();
        if (shape == null) return null;

        ITexPositionSource texSource = new ShapeTextureSource(capi, shape, "PS-SubstituteItemTexSource");

        tesselator.TesselateShape(null, shape, out MeshData mesh, texSource);
        return mesh;
    }

    /// <summary>
    /// Generates the content mesh of the block's inventory. Mainly used for generating basket contents.
    /// </summary>
    public static MeshData GenContentMesh(ICoreClientAPI capi, ITextureAtlasAPI targetAtlas, ItemStack[] contents, float[,] transformationMatrix, float scaleValue = 1f, Dictionary<string, ModelTransform> modelTransformations = null) {
        if (capi == null) return null;

        MeshData nestedContentMesh = null;
        for (int i = 0; i < contents.Length; i++) {
            if (contents[i] == null || contents[i].Item == null) continue;

            string shapeLocation = contents[i].Item.Shape?.Base;
            if (shapeLocation == null) continue;

            Shape shape = capi.TesselatorManager.GetCachedShape(shapeLocation)?.Clone();
            //string shapeLocation = contents[i].Item.Shape?.Base.WithPathPrefixOnce("shapes/").WithPathAppendixOnce(".json").ToString();
            //Shape shape = capi.Assets.TryGet(shapeLocation)?.ToObject<Shape>().Clone();
            if (shape == null) continue;

            if (shape.Textures.Count == 0) {
                foreach (var texture in contents[i].Item.Textures) {
                    shape.Textures.Add(texture.Key, texture.Value.Base);
                }
            }

            UniversalShapeTextureSource texSource = new(capi, targetAtlas, shape, "PS-ContentTextureSource");

            foreach (var textureDict in shape.Textures) {
                CompositeTexture cTex = new(textureDict.Value);
                cTex.Bake(capi.Assets);
                texSource.textures[textureDict.Key] = cTex;
            }

            capi.Tesselator.TesselateShape("PS-TesselateContent", shape, out MeshData collectibleMesh, texSource);

            int offset = transformationMatrix.GetLength(1);
            if (i < offset) {
                if (modelTransformations != null) {
                    ModelTransform transformation = contents[i].Item.GetTransformation(modelTransformations);
                    if (transformation != null) collectibleMesh.ModelTransform(transformation);
                }

                float[] matrixTransform =
                    new Matrixf()
                    .Translate(0.5f, 0, 0.5f)
                    .RotateXDeg(transformationMatrix[3, i])
                    .RotateYDeg(transformationMatrix[4, i])
                    .RotateZDeg(transformationMatrix[5, i])
                    .Scale(scaleValue, scaleValue, scaleValue)
                    .Translate(transformationMatrix[0, i] - 0.84375f, transformationMatrix[1, i], transformationMatrix[2, i] - 0.8125f)
                    .Values;

                collectibleMesh.MatrixTransform(matrixTransform);
            }

            if (nestedContentMesh == null) nestedContentMesh = collectibleMesh;
            else nestedContentMesh.AddMeshData(collectibleMesh);
        }

        return nestedContentMesh;
    }

    /// <summary>
    /// Generates a mesh that will generate some item shapes, and then move them up as the stack size increases.
    /// Do note that generally, the item shapes should "cover" up the whole top so you can't see the bottom of the block, giving the illusion that it's full.
    /// </summary>
    public static MeshData GenPartialContentMesh(ICoreClientAPI capi, ItemStack[] contents, float[][] transformationMatrices, float maxHeight, string utilShapeLoc = null) {
        if (capi == null) return null;
        if (contents == null || contents.Length == 0 || contents[0] == null || contents[0].Item == null) return null;

        // Get item shape
        string shapeLocation = contents[0].Item.Shape?.Base;
        if (shapeLocation == null) return null;

        Shape shape = capi.TesselatorManager.GetCachedShape(shapeLocation)?.Clone();
        if (shape == null) return null;

        // Handle texture source
        ITexPositionSource texSource = new ShapeTextureSource(capi, shape, "PS-PartialContentMesh");
        capi.Tesselator.TesselateShape("PS-TesselatePartial", shape, out MeshData itemMesh, texSource);

        MeshData contentMesh = itemMesh.Clone();
        if (transformationMatrices?[0] != null) 
            contentMesh.MatrixTransform(transformationMatrices[0]);
        
        // Mesh some shapes
        for (int i = 1; i < Math.Min(contents[0].StackSize, transformationMatrices.Length); i++) {
            MeshData currentMesh = itemMesh.Clone();
            if (transformationMatrices?[i] != null)
                currentMesh.MatrixTransform(transformationMatrices[i]);
            
            contentMesh.AddMeshData(currentMesh);
        }

        // If more items are added, move content up
        if (contents[0].StackSize > transformationMatrices.Length) {
            // Calculate the total content amount
            float contentAmount = 0;
            foreach (var itemStack in contents) {
                contentAmount += itemStack?.StackSize ?? 0;
            }

            // Calculating new height
            int stackSizeDiv = contents[0].Collectible.MaxStackSize / 32;
            float step = maxHeight / (contents.Length * 32 * stackSizeDiv);
            float shapeHeight = contentAmount * step;

            contentMesh?.Translate(0, shapeHeight, 0);

            if (utilShapeLoc != null) {
                // Get util shape
                AssetLocation utilLoc = new(utilShapeLoc);
                Shape utilShape = Shape.TryGet(capi, utilLoc);

                if (utilShape == null) {
                    capi.Logger.Warning("[PurposefulStorage] non-existent utilShapeLoc passed. No content mesh will be generated.");
                    return null;
                }

                // Get util textures
                var textures = contents[0].Item.Textures;
                texSource = new ContainerTextureSource(capi, contents[0], textures.Values.FirstOrDefault());

                capi.Tesselator.TesselateShape("PS-TesselatePartialUtil", utilShape, out MeshData utilMesh, texSource);
        
                utilMesh.Translate(0, shapeHeight, 0);
                contentMesh.AddMeshData(utilMesh);
            }
        }

        return contentMesh;
    }
}
