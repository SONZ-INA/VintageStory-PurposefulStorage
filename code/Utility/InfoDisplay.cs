namespace PurposefulStorage;

public static class InfoDisplay {
    public enum InfoDisplayOptions {
        ByBlock,
        ByShelf,
        BySegment
    }

    #region // Public Methods -------------------------------------------------------------------------------------------------------------------------------

    public static void DisplayInfo(IPlayer forPlayer, StringBuilder sb, InventoryGeneric inv, InfoDisplayOptions displaySelection, int slotCount, int segmentsPerShelf = 0, int itemsPerSegment = 0, int skipSlotsFrom = -1, int selectedSegment = -1) {
        if (selectedSegment == -1 && forPlayer.CurrentBlockSelection != null)
            selectedSegment = forPlayer.CurrentBlockSelection.SelectionBoxIndex;

        if (displaySelection != InfoDisplayOptions.ByBlock && selectedSegment == -1)
            return;

        ProcessGroupedDisplay(sb, inv, displaySelection, slotCount, segmentsPerShelf, itemsPerSegment, skipSlotsFrom, selectedSegment);
    }

    public static string TransitionInfoCompact(IWorldAccessor world, ItemSlot? contentSlot, EnumTransitionType transitionType, TransitionDisplayMode displayMode) {
        if (contentSlot == null || contentSlot.Empty) return "";

        TransitionState? state = contentSlot.Itemstack?.Collectible.UpdateAndGetTransitionState(world, contentSlot, transitionType);
        if (state == null) return "";

        float rateMul = contentSlot.Itemstack!.Collectible.GetTransitionRateMul(world, contentSlot, transitionType);
        if (rateMul <= 0) return "";

        if (displayMode == TransitionDisplayMode.Percentage && state.TransitionLevel > 0) {
            return GetTransitionPercentageText(transitionType, state.TransitionLevel);
        }

        double hoursLeft = state.TransitionLevel > 0
            ? state.TransitionHours / rateMul * (1 - state.TransitionLevel)
            : state.FreshHoursLeft / rateMul;

        return GetTimeRemainingText(world, hoursLeft, transitionType);
    }

    #endregion

    #region // Private Info Methods -------------------------------------------------------------------------------------------------------------------------

    private static void ProcessGroupedDisplay(StringBuilder sb, InventoryGeneric inv, InfoDisplayOptions displaySelection, int slotCount, int segmentsPerShelf, int itemsPerSegment, int skipSlotsFrom, int selectedSegment) {
        var (start, end) = GetIterationBounds(displaySelection, slotCount, segmentsPerShelf, itemsPerSegment, selectedSegment);

        Dictionary<string, int> groupedItems = [];
        int step = displaySelection == InfoDisplayOptions.ByBlock ? -1 : 1;

        for (int i = start; i != end; i += step) {
            if (i >= slotCount) break;
            if (skipSlotsFrom != -1 && i >= skipSlotsFrom) break;
            if (inv[i].Empty) continue;

            ItemStack stack = inv[i].Itemstack!;
            string itemName = stack.GetName();

            if (groupedItems.ContainsKey(itemName)) {
                groupedItems[itemName] += stack.StackSize;
            }
            else {
                groupedItems[itemName] = stack.StackSize;
            }
        }

        AppendGroupedItems(sb, groupedItems);
    }

    private static void AppendGroupedItems(StringBuilder sb, Dictionary<string, int> groupedItems) {
        foreach (var kvp in groupedItems) {
            sb.Append(kvp.Key);
            if (kvp.Value > 1) sb.Append(" x" + kvp.Value);
            sb.AppendLine();
        }
    }

    #endregion

    #region // Private Helpers ------------------------------------------------------------------------------------------------------------------------------

    private static (int start, int end) GetIterationBounds(InfoDisplayOptions displaySelection, int slotCount, int segmentsPerShelf, int itemsPerSegment, int selectedSegment) {
        return displaySelection switch {
            InfoDisplayOptions.ByBlock => (slotCount - 1, -1),
            InfoDisplayOptions.ByShelf => CalculateShelfBounds(selectedSegment, segmentsPerShelf, itemsPerSegment),
            InfoDisplayOptions.BySegment => (selectedSegment * itemsPerSegment, (selectedSegment * itemsPerSegment) + itemsPerSegment),
            _ => (0, slotCount)
        };
    }

    private static (int start, int end) CalculateShelfBounds(int selectedSegment, int segmentsPerShelf, int itemsPerSegment) {
        int itemsPerShelf = segmentsPerShelf * itemsPerSegment;
        int selectedShelf = selectedSegment / segmentsPerShelf * itemsPerShelf;
        return (selectedShelf, selectedShelf + itemsPerShelf);
    }

    private static string GetTransitionPercentageText(EnumTransitionType transitionType, float transitionLevel) {
        int percent = (int)Math.Round(transitionLevel * 100);

        return transitionType switch {
            EnumTransitionType.Perish => Lang.Get("{0}% spoiled", percent),
            EnumTransitionType.Dry => Lang.Get("itemstack-dryable-dried", percent),
            EnumTransitionType.Cure => Lang.Get("itemstack-curable-curing", percent),
            EnumTransitionType.Ripen => Lang.Get("itemstack-ripenable-ripening", percent),
            EnumTransitionType.Melt => Lang.Get("itemstack-meltable-melted", percent),
            _ => ""
        };
    }

    private static string GetTimeRemainingText(IWorldAccessor world, double hoursLeft, EnumTransitionType transitionType, string? actionVerb = null) {
        string prefix = transitionType switch {
            EnumTransitionType.Cure => "<font color=\"#bd5424\">" + Lang.Get("Curing") + "</font>: ",
            EnumTransitionType.Dry => "<font color=\"#d6ba7a\">" + Lang.Get("Drying") + "</font>: ",
            _ => ""
        };

        if (string.IsNullOrEmpty(actionVerb)) {
            actionVerb = transitionType switch {
                EnumTransitionType.Perish => "fresh for",
                EnumTransitionType.Ripen => "will ripen in",
                EnumTransitionType.Cure => "foodshelves:Will cure in",
                EnumTransitionType.Dry => "foodshelves:Will dry in",
                EnumTransitionType.Melt => "foodshelves:Will melt in",
                _ => ""
            };
        }

        if (string.IsNullOrEmpty(actionVerb)) return "";

        double hoursPerDay = world.Calendar.HoursPerDay;
        double daysLeft = hoursLeft / hoursPerDay;

        if (daysLeft >= world.Calendar.DaysPerYear) {
            return prefix + Lang.Get($"{actionVerb} {{0}} years", Math.Round(daysLeft / world.Calendar.DaysPerYear, 1));
        }

        if (hoursLeft > hoursPerDay) {
            return prefix + Lang.Get($"{actionVerb} {{0}} days", Math.Round(daysLeft, 1));
        }

        return prefix + Lang.Get($"{actionVerb} {{0}} hours", Math.Round(hoursLeft, 1));
    }


    #endregion
}