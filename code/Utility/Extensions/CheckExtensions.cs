﻿namespace PurposefulStorage;

public static class CheckExtensions {
    /// <summary>
    /// Determines whether the given collectible object matches any of the allowed types specified in the <see cref="RestrictionData"/>.<br/>
    /// This check compares the object's runtime class name (e.g. <c>BlockCheese</c>) against entries in the <c>CollectibleTypes</c> list
    /// using reflection.
    /// </summary>
    public static bool CheckTypedRestriction(this CollectibleObject obj, RestrictionData data) 
        => data.CollectibleTypes?.Contains(obj.Code.Domain + ":" + obj.GetType().Name) == true;

    /// <summary>
    /// Determines whether the collectible in the given slot has the specified attribute set to true,
    /// indicating that it is allowed to be stored.<br/> Prevents storage if the slot belongs to a hopper.
    /// </summary>
    public static bool CanStoreInSlot(this ItemSlot slot, string attributeWhitelist) {
        if (!(slot?.Itemstack?.Collectible?.Attributes?[attributeWhitelist].AsBool() == true)) return false;
        if (slot?.Inventory?.ClassName == "hopper") return false;
        return true;
    }

    /// <summary>
    /// Determines whether the collectible in the given slot has **any** of the specified attributes set to true,
    /// allowing it to be stored.<br/> Prevents storage if the slot belongs to a hopper.
    /// </summary>
    public static bool CanStoreInSlot(this ItemSlot slot, string[] attributeWhitelist) {
        if (slot?.Inventory?.ClassName == "hopper") return false;

        foreach (string attribute in attributeWhitelist) {
            if (slot?.Itemstack?.Collectible?.Attributes?[attribute].AsBool() == true) return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the collectible has the specified attribute set to true,
    /// allowing it to be stored in compatible containers.
    /// </summary>
    public static bool CanStoreInSlot(this CollectibleObject obj, string attributeWhitelist) {
        return obj?.Attributes?[attributeWhitelist].AsBool() == true;
    }
}