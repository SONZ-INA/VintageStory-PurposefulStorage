namespace PurposefulStorage;

public static class PSDataFinalizer {
    /// <summary>
    /// Applies all defined restrictions and optional transformations to collectible items needed for this mod.
    /// </summary>
    public static void FinalizeAssets(ICoreAPI api, Dictionary<string, RestrictionData> restrictions, Dictionary<string, Dictionary<string, ModelTransform>> transformations) {
        foreach (CollectibleObject obj in api.World.Collectibles) {
            if (obj?.Code == null) continue;

            foreach (var restriction in restrictions) {
                PatchCollectibleWhitelist(obj, restriction);
            }

            foreach (var transformation in transformations) {
                PatchCollectibleTransformation(obj, transformation);
            }
        }
    }

    /// <summary>
    /// Checks if a collectible matches a restriction and applies the "ps{name}" tag.
    /// </summary>
    private static void PatchCollectibleWhitelist(CollectibleObject obj, KeyValuePair<string, RestrictionData> restrictionData) {
        bool shouldPatch = false;

        string token = restrictionData.Key;
        RestrictionData data = restrictionData.Value;

        if (data.BlacklistedCodes != null) {
            if (WildcardUtil.Match(data.BlacklistedCodes, obj.Code.ToString())) {
                return;
            }
        }

        if (data.GroupingCodes != null && data.GroupingCodes.Count > 0) {
            foreach (var group in data.GroupingCodes.Values) {
                foreach (var code in group) {
                    if (WildcardUtil.Match(code, obj.Code.ToString())) {
                        shouldPatch = true;
                        break;
                    }
                }
                if (shouldPatch) break;
            }
        }
        else {
            if (obj.CheckTypedRestriction(data) || WildcardUtil.Match(data.CollectibleCodes, obj.Code.ToString())) {
                shouldPatch = true;
            }
        }

        if (!shouldPatch) return;

        obj.EnsureAttributesNotNull();
        obj.Attributes.Token!["ps" + token] = JToken.FromObject(true);
    }

    /// <summary>
    /// Writes "on{name}Transform" onto the collectible if one of the file's wildcard entries matches its code. Independent of restrictions.
    /// </summary>
    private static void PatchCollectibleTransformation(CollectibleObject obj, KeyValuePair<string, Dictionary<string, ModelTransform>> transformationData) {
        ModelTransform? transformation = obj.GetTransformation(transformationData.Value);
        if (transformation == null) return;

        obj.EnsureAttributesNotNull();
        obj.Attributes.Token!["on" + transformationData.Key + "Transform"] = JToken.FromObject(transformation);
    }
}
