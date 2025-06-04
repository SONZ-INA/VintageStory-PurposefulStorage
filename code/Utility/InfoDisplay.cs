namespace PurposefulStorage;

public static class InfoDisplay {
    public enum InfoDisplayOptions {
        ByBlock,
        ByShelf,
        BySegment,
        ByBlockGrouped,
        BySegmentGrouped
    }

    public static string GetNameAndStackSize(ItemStack stack) => stack.GetName() + " x" + stack.StackSize;

    public static void DisplayInfo(IPlayer forPlayer, StringBuilder sb, InventoryGeneric inv, InfoDisplayOptions displaySelection, int slotCount, int segmentsPerShelf = 0, int itemsPerSegment = 0, bool skipLine = true, int skipSlotsFrom = -1, int selectedSegment = -1) {
        if (skipLine) sb.AppendLine(); // Space in between to be in line with vanilla

        List<ItemSlot> itemSlotList = new();
        foreach (var slot in inv) {
            itemSlotList.Add(slot);
        }

        switch (displaySelection) {
            case InfoDisplayOptions.ByBlockGrouped:
                ByBlockMerged(itemSlotList.ToArray(), sb);
                return;
            case InfoDisplayOptions.BySegmentGrouped:
                int fromSlot = forPlayer.CurrentBlockSelection.SelectionBoxIndex * itemsPerSegment;
                sb.Append(ItemInfoGrouped(inv, fromSlot, fromSlot + itemsPerSegment));
                return;
        }

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

        for (int i = start; i != end; i = displaySelection == InfoDisplayOptions.ByBlock ? i - 1 : i + 1) {
            if (i >= slotCount) break;
            if (skipSlotsFrom != -1 && i >= skipSlotsFrom) break;
            if (inv[i].Empty) continue;

            ItemStack stack = inv[i].Itemstack;
            sb.Append(stack.GetName());
            if (stack.StackSize > 1) sb.Append(" x" + stack.StackSize);
            sb.AppendLine();
        }
    }

    public static string ItemInfoCompact(ItemSlot contentSlot, bool withStackName = true, bool withStackSize = true) {
        if (contentSlot.Empty) return "";

        StringBuilder dsc = new();
        if (withStackName) dsc.Append(contentSlot.Itemstack.GetName());
        if (withStackSize && contentSlot.StackSize > 1) dsc.Append(" x" + contentSlot.StackSize);

        dsc.AppendLine();
        return dsc.ToString();
    }

    public static string ItemInfoGrouped(InventoryGeneric inv, int start, int end) {
        if (inv.Empty) return "";

        StringBuilder dsc = new();
        Dictionary<string, int> grouped = new();

        // Group items by their name and sum their stack sizes
        for (int i = start; i < end; i++) {
            if (i >= inv.Count || inv[i].Empty) continue;

            ItemStack stack = inv[i].Itemstack;
            if (stack == null) continue;

            string itemKey = stack.GetName();

            if (!grouped.TryGetValue(itemKey, out int currentCount)) {
                currentCount = 0;
            }

            grouped[itemKey] = currentCount + stack.StackSize;
        }

        // Display grouped items with their total count
        foreach (var group in grouped) {
            string itemName = group.Key;
            int totalCount = group.Value;

            dsc.Append(itemName + " x" + totalCount);
            dsc.AppendLine();
        }

        return dsc.ToString();
    }

    public static void ByBlockMerged(ItemSlot[] slots, StringBuilder sb) {
        if (slots == null || slots.Length == 0) return;

        ItemStack firstStack = slots[0].Itemstack?.Clone();
        if (firstStack == null) return;

        int totalStackSize = firstStack.StackSize;

        for (int i = 1; i < slots.Length; i++) {
            ItemStack stack = slots[i].Itemstack;
            if (stack == null) break; // Subsequent slots can't have items if the current one is empty.
            totalStackSize += stack.StackSize;
        }

        firstStack.StackSize = totalStackSize;

        sb.Append(firstStack.GetName());
        if (totalStackSize > 1) sb.Append(" x" + totalStackSize);
        sb.AppendLine();
    }

    public static string ItemInfoSummary(ItemSlot[] contentSlots) {
        StringBuilder dsc = new();

        if (contentSlots == null || contentSlots.Length == 0) {
            dsc.Append("Empty");
            return dsc.ToString();
        }

        Dictionary<string, int> itemCounts = new();
        int totalItems = 0;

        foreach (var slot in contentSlots) {
            if (slot.Empty) continue;

            var stack = slot.Itemstack;
            string itemName = stack.GetName();

            if (!itemCounts.TryGetValue(itemName, out int currentCount)) {
                currentCount = 0;
            }

            itemCounts[itemName] = currentCount + stack.StackSize;
            totalItems += stack.StackSize;
        }

        if (totalItems > 0) {
            foreach (var item in itemCounts) {
                dsc.AppendLine($"{item.Key} x{item.Value}");
            }
        }
        else {
            dsc.AppendLine("Empty");
        }

        return dsc.ToString();
    }
}
