namespace PurposefulStorage;

public static class InfoDisplay {
    public enum InfoDisplayOptions {
        ByBlock,
        ByShelf,
        BySegment
    }

    public static void DisplayInfo(IPlayer forPlayer, StringBuilder sb, InventoryGeneric inv, InfoDisplayOptions displaySelection, int slotCount, int segmentsPerShelf = 0, int itemsPerSegment = 0, int skipSlotsFrom = -1, int selectedSegment = -1) {
        if (selectedSegment == -1 && forPlayer.CurrentBlockSelection != null)
            selectedSegment = forPlayer.CurrentBlockSelection.SelectionBoxIndex;

        if (displaySelection != InfoDisplayOptions.ByBlock && selectedSegment == -1) return;

        int start = 0, end = slotCount;

        switch (displaySelection) {
            case InfoDisplayOptions.ByBlock:
                start = slotCount - 1;
                end = -1;
                break;
            case InfoDisplayOptions.ByShelf:
                int itemsPerShelf = segmentsPerShelf * itemsPerSegment;
                int selectedShelf = selectedSegment / segmentsPerShelf * itemsPerShelf;
                start = selectedShelf;
                end = selectedShelf + itemsPerShelf;
                break;
            case InfoDisplayOptions.BySegment:
                start = selectedSegment * itemsPerSegment;
                end = start + itemsPerSegment;
                break;
        }

        // Dictionary to group items
        Dictionary<string, int> groupedItems = new();

        for (int i = start; i != end; i = displaySelection == InfoDisplayOptions.ByBlock ? i - 1 : i + 1) {
            if (i >= slotCount) break;
            if (skipSlotsFrom != -1 && i >= skipSlotsFrom) break;
            if (inv[i].Empty) continue;

            ItemStack stack = inv[i].Itemstack;
            string itemName = stack.GetName();

            if (groupedItems.ContainsKey(itemName)) {
                groupedItems[itemName] += stack.StackSize;
            }
            else {
                groupedItems[itemName] = stack.StackSize;
            }
        }

        foreach (var kvp in groupedItems) {
            sb.Append(kvp.Key);
            if (kvp.Value > 1) sb.Append(" x" + kvp.Value);
            sb.AppendLine();
        }
    }
}
