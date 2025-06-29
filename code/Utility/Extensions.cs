﻿using System.Linq;
using System.Text.Json;
using Vintagestory.ServerMods;

namespace PurposefulStorage;

public static class Extensions {
    #region JSONExtensions

    public static void EnsureAttributesNotNull(this CollectibleObject obj) => obj.Attributes ??= new JsonObject(new JObject());
    public static T LoadAsset<T>(this ICoreAPI api, string path) => api.Assets.Get(new AssetLocation(path)).ToObject<T>();

    #endregion

    #region MeshExtensions

    /// <summary>
    /// Rotates the mesh in a specific cardinal direction the block has.
    /// </summary>
    public static MeshData BlockYRotation(this MeshData mesh, Block block) {
        return mesh?.Rotate(new Vec3f(0.5f, 0.5f, 0.5f), 0, block.Shape.rotateY * GameMath.DEG2RAD, 0);
    }

    /// <summary>
    /// Returns the angle that the block is placed at. Used for meshing blocks that can rotate freely (eg. baskets).
    /// </summary>
    public static float GetBlockMeshAngle(IPlayer byPlayer, BlockSelection blockSel, bool val) {
        if (val) {
            BlockPos targetPos = blockSel.DidOffset ? blockSel.Position.AddCopy(blockSel.Face.Opposite) : blockSel.Position;
            double dx = byPlayer.Entity.Pos.X - (targetPos.X + blockSel.HitPosition.X);
            double dz = byPlayer.Entity.Pos.Z - (targetPos.Z + blockSel.HitPosition.Z);
            float angleHor = (float)Math.Atan2(dx, dz);

            float deg22dot5rad = GameMath.PIHALF / 4;
            float roundRad = ((int)Math.Round(angleHor / deg22dot5rad)) * deg22dot5rad;
            return roundRad;
        }
        return 0;
    }

    /// <summary>
    /// Block Variant Data needed for blocks to mesh their variant textures properly. Avoid using in animation mesh generation.
    /// </summary>
    public static (Shape, ITexPositionSource) GetBlockVariantData(ICoreClientAPI capi, ItemStack stackWithAttributes) {
        Block block = stackWithAttributes.Block;

        string shapeLocation = block.Shape.Base.WithPathPrefixOnce("shapes/").WithPathAppendixOnce(".json");
        Shape shape = capi.Assets.TryGet(shapeLocation)?.ToObject<Shape>().Clone();
        if (shape == null) return (null, null);

        if (shape.Textures.Count == 0) {
            foreach (var texture in block.Textures) {
                shape.Textures.Add(texture.Key, texture.Value.Base);
            }
        }

        var stexSource = new ShapeTextureSource(capi, shape, "FS-TextureSource");

        // Custom Textures
        if (stackWithAttributes.Attributes[BasePSContainer.PSAttributes] is ITreeAttribute tree && block.Attributes["variantTextures"].Exists) {
            foreach (var pair in block.Attributes["variantTextures"].AsObject<Dictionary<string, string[]>>()) {
                string[] texPaths = pair.Value;

                foreach (var attr in tree) {
                    string key = attr.Key;
                    string value = attr.Value?.GetValue()?.ToString() ?? ""; // Null safety when removing a mod from a world.

                    foreach (string texPath in texPaths.Reverse()) { // Reverse to start from the end texture paths (patched textures), first one serving as default
                        if (texPath.Contains($"{{{key}}}")) {
                            string fullTexPath = texPath.Replace($"{{{key}}}", value);

                            if (capi.Assets.TryGet(new AssetLocation(fullTexPath).WithPathPrefixOnce("textures/").WithPathAppendixOnce(".png")) == null) {
                                continue;
                            }

                            shape.Textures[key] = fullTexPath;

                            var ctex = new CompositeTexture(fullTexPath);

                            //BlendedOverlayTexture overlay = new() {
                            //    Base = new AssetLocation("purposefulstorage:variants/overlay/shelves/wood")
                            //};

                            //ctex.BlendedOverlays ??= Array.Empty<BlendedOverlayTexture>();
                            //ctex.BlendedOverlays.Append(overlay);

                            ctex.Bake(capi.Assets);
                            stexSource.textures[pair.Key] = ctex;
                            break;
                        }
                    }
                }
            }
        }

        return (shape, stexSource);
    }

    /// <summary>
    /// Apply variant textures from block attributes. Used in animations. Don't use GetBlockVariantData in animations!
    /// </summary>
    public static void ApplyVariantTextures(this Shape shape, BEBasePSContainer pscontainer) {
        var variantTextures = pscontainer.Block.Attributes?["variantTextures"]?.AsObject<Dictionary<string, string[]>>();

        if (variantTextures == null) return;
        if (pscontainer.VariantAttributes == null) return;

        foreach (var texture in variantTextures) {
            string[] textureValues = texture.Value;

            foreach (string originalTexVal in textureValues.Reverse()) {
                string textureValue = originalTexVal;

                foreach (var attr in pscontainer.VariantAttributes) {
                    string paramPlaceholder = "{" + attr.Key + "}";
                    string paramValue = attr.Value.ToString();
                    textureValue = textureValue.Replace(paramPlaceholder, paramValue);
                }

                if (textureValue.Contains('{') || textureValue.Contains('}')) continue;

                if ((pscontainer.Api as ICoreClientAPI)?.Assets.TryGet(new AssetLocation(textureValue).WithPathPrefixOnce("textures/").WithPathAppendixOnce(".png")) == null) {
                    continue;
                }

                shape.Textures[texture.Key] = textureValue;
                break;
            }
        }
    }

    /// <summary>
    /// Get the stack from the entity that has the attributes needed to mesh the textures properly. Use in GetBlockVariantMesh().
    /// </summary>
    public static ItemStack GetVariantStack(BEBasePSContainer entity) {
        var stack = new ItemStack(entity.Block);
        if (entity.VariantAttributes.Count != 0) {
            stack.Attributes[BasePSContainer.PSAttributes] = entity.VariantAttributes;
        }

        return stack;
    }

    public static void ChangeShapeTextureKey(Shape shape, string key) {
        foreach (var face in shape.Elements[0].FacesResolved) {
            face.Texture = key;
        }

        foreach (var child in shape.Elements[0].Children) {
            foreach (var face in child.FacesResolved) {
                if (face != null) face.Texture = key;
            }
        }
    }

    public static void ApplyModelTransformToMatrixF(this Matrixf mat, ModelTransform transformation) {
        if (transformation == null) return;

        mat.Translate(0.5f, 0, 0.5f);

        if (transformation.Translation != null) {
            mat.Translate(transformation.Translation.X, transformation.Translation.Y, transformation.Translation.Z);
        }

        if (transformation.Rotation != null) {
            mat.RotateXDeg(transformation.Rotation.X);
            mat.RotateYDeg(transformation.Rotation.Y);
            mat.RotateZDeg(transformation.Rotation.Z);
        }

        if (transformation.ScaleXYZ != null) {
            mat.Scale(transformation.ScaleXYZ.X, transformation.ScaleXYZ.Y, transformation.ScaleXYZ.Z);
        }

        mat.Translate(-0.5f, 0, -0.5f);
    }

    /// <summary>
    /// For blocks that can have a variety of items, uses a hashing function to disperse cached meshes.
    /// </summary>
    public static int GetStackCacheHashCodeFNV(ItemStack[] contentStack) {
        if (contentStack == null) return 0;

        unchecked {
            // FNV-1 hash since any other simpler one ends up colliding
            const uint FNV_OFFSET_BASIS = 2166136261;
            const uint FNV_32_PRIME = 16777619;

            uint hash = FNV_OFFSET_BASIS;

            hash = (hash ^ (uint)contentStack.Length.GetHashCode()) * FNV_32_PRIME;

            for (int i = 0; i < contentStack.Length; i++) {
                if (contentStack[i] == null) continue;

                uint collectibleHash = (uint)(contentStack[i].Collectible != null ? contentStack[i].Collectible.Code.GetHashCode() : 0);
                hash = (hash ^ collectibleHash) * FNV_32_PRIME;
            }

            return (int)hash;
        }
    }

    /// <summary>
    /// Returns the dictionary used for caching meshes.
    /// </summary>
    public static Dictionary<string, MultiTextureMeshRef> GetCacheDictionary(ICoreClientAPI capi, string meshCacheKey) {
        if (capi.ObjectCache.TryGetValue(meshCacheKey, out object obj)) {
            return obj as Dictionary<string, MultiTextureMeshRef>;
        }
        else {
            var dict = new Dictionary<string, MultiTextureMeshRef>();
            capi.ObjectCache[meshCacheKey] = dict;
            return dict;
        }
    }

    #endregion

    #region GeneralBlockExtensions

    public static T GetBlockEntityExt<T>(this IBlockAccessor blockAccessor, BlockPos pos) where T : BlockEntity {
        if (blockAccessor.GetBlockEntity<T>(pos) is T blockEntity) {
            return blockEntity;
        }

        if (blockAccessor.GetBlock(pos) is BlockMultiblock multiblock) {
            BlockPos multiblockPos = new(pos.X + multiblock.OffsetInv.X, pos.Y + multiblock.OffsetInv.Y, pos.Z + multiblock.OffsetInv.Z, pos.dimension);
            return blockAccessor.GetBlockEntity<T>(multiblockPos);
        }

        return null;
    }

    public static ModelTransform GetTransformation(this CollectibleObject obj, Dictionary<string, ModelTransform> transformations) {
        foreach (KeyValuePair<string, ModelTransform> transformation in transformations) {
            if (WildcardUtil.Match(transformation.Key, obj.Code.ToString())) return transformation.Value;
        }
        return null;
    }

    public static void LoadVariantsCreative(ICoreAPI api, Block block) {
        string blockSide = block.Variant["side"];
        string dropSide = "east";

        string properties = block.GetBehavior<BlockBehaviorHorizontalOrientable>()?.propertiesAtString;
        if (properties != null) {
            JsonDocument jsonDoc = JsonDocument.Parse(properties);
            dropSide = jsonDoc.RootElement.GetProperty("dropBlockFace").GetString() ?? "east";
        }

        if (blockSide != null && blockSide != dropSide) return;

        var materials = block.Attributes["materials"].AsObject<RegistryObjectVariantGroup>();
        string material = "";
        StandardWorldProperty props = null;

        if (materials?.LoadFromProperties != null) {
            material = materials.LoadFromProperties.ToString().Split('/')[1];
            props = api.Assets.TryGet(materials.LoadFromProperties.WithPathPrefixOnce("worldproperties/").WithPathAppendixOnce(".json"))?.ToObject<StandardWorldProperty>();
        }

        var stacks = new List<JsonItemStack>();

        if (block.Attributes?["skipDefault"]?.AsBool() != true) {
            var defaultBlock = new JsonItemStack() {
                Code = block.Code,
                Type = EnumItemClass.Block,
                Attributes = new JsonObject(JToken.Parse("{}"))
            };
            defaultBlock.Resolve(api.World, block.Code);
            stacks.Add(defaultBlock);
        }

        if (props != null && material != "") {
            foreach (var prop in props.Variants) {
                string psAttributesJson = $"{{ \"{material}\": \"{prop.Code.Path}\" }}";
                string attributesJson = "{ \"PSAttributes\": " + psAttributesJson + " }";

                var jstack = new JsonItemStack() {
                    Code = block.Code,
                    Type = EnumItemClass.Block,
                    Attributes = new JsonObject(JToken.Parse(attributesJson))
                };

                jstack.Resolve(api.World, block.Code);
                stacks.Add(jstack);
            }
        }

        block.CreativeInventoryStacks = new CreativeTabAndStackList[] {
            new() { Stacks = stacks.ToArray(), Tabs = new string[] { "general", "decorative", "purposefulstorage" }}
        };
    }

    public static string GetMaterialNameLocalized(this ItemStack itemStack, bool includeParenthesis = true) {
        if (itemStack.Attributes["PSAttributes"] is not ITreeAttribute tree)
            return "";

        foreach (var pair in tree) {
            switch (pair.Key) {
                case "wood":
                    return (includeParenthesis ? "(" : "") + Lang.Get("game:material-" + pair.Value) + (includeParenthesis ? ")" : "");
                case "rock":
                    return (includeParenthesis ? "(" : "") + Lang.Get("game:rock-" + pair.Value) + (includeParenthesis ? ")" : "");
            }
        }

        return "";
    }

    public static void MBNormalizeSelectionBox(this Cuboidf selectionBox, Vec3i offset) {
        // Make sure that the selection boxes defined in blocktype .json file are defined with the following rotations:
        // { "*-north": 0, "*-east": 270, "*-west": 90, "*-south": 180 }
        // Otherwise, the selection boxes won't correctly normalize
        selectionBox.X1 += offset.X;
        selectionBox.X2 += offset.X;
        selectionBox.Y1 += offset.Y;
        selectionBox.Y2 += offset.Y;
        selectionBox.Z1 += offset.Z;
        selectionBox.Z2 += offset.Z;
    }

    public static int GetRotationAngle(Block block) {
        // This one's more-less hardcoded that these rotations always have to "align" with the ones defined in blocktype but oh well.
        string blockPath = block.Code.Path;
        if (blockPath.EndsWith("-north")) return 0;
        if (blockPath.EndsWith("-south")) return 180;
        if (blockPath.EndsWith("-east")) return 270;
        if (blockPath.EndsWith("-west")) return 90;
        return 0;
    }

    #endregion

    #region CheckExtensions

    public static bool CheckTypedRestriction(this CollectibleObject obj, RestrictionData data) => data.CollectibleTypes?.Contains(obj.Code.Domain + ":" + obj.GetType().Name) == true;

    public static bool CanStoreInSlot(this ItemSlot slot, string attributeWhitelist) {
        if (!(slot?.Itemstack?.Collectible?.Attributes?[attributeWhitelist].AsBool() == true)) return false;
        if (slot?.Inventory?.ClassName == "hopper") return false;
        return true;
    }

    public static bool CanStoreInSlot(this ItemSlot slot, string[] attributeWhitelist) {
        if (slot?.Inventory?.ClassName == "hopper") return false;
        
        foreach (string attribute in attributeWhitelist) {
            if (slot?.Itemstack?.Collectible?.Attributes?[attribute].AsBool() == true) return true;
        }

        return false;
    }

    public static bool CanStoreInSlot(this CollectibleObject obj, string attributeWhitelist) {
        return obj?.Attributes?[attributeWhitelist].AsBool() == true;
    }

    #endregion
}
