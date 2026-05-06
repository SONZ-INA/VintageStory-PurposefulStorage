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

    #endregion
}