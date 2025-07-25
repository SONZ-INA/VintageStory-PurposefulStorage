namespace PurposefulStorage;

internal class ItemSlotPSUniversal : ItemSlot {
    public override int MaxSlotStackSize => maxSlotStackSize;
    private readonly int maxSlotStackSize;
    private readonly string[] attributeCheck;

    public ItemSlotPSUniversal(InventoryBase inventory, string[] attributeCheck, int maxSlotStackSize = 1) : base(inventory) {
        this.inventory = inventory;
        this.attributeCheck = attributeCheck;
        this.maxSlotStackSize = maxSlotStackSize;
    }

    public ItemSlotPSUniversal(InventoryBase inventory, string attributeCheck, int maxSlotStackSize = 1) : base(inventory) {
        this.inventory = inventory;
        this.attributeCheck = [attributeCheck];
        this.maxSlotStackSize = maxSlotStackSize;
    }

    public override bool CanTakeFrom(ItemSlot slot, EnumMergePriority priority = EnumMergePriority.AutoMerge) {
        bool canStore = false;
        foreach (var attribute in attributeCheck) {
            if (slot.CanStoreInSlot(attribute)) {
                canStore = true;
                break;
            }
        }

        return canStore && base.CanTakeFrom(slot, priority);
    }

    public override bool CanHold(ItemSlot slot) {
        bool canStore = false;
        foreach (var attribute in attributeCheck) {
            if (slot.CanStoreInSlot(attribute)) {
                canStore = true;
                break;
            }
        }

        return canStore && base.CanHold(slot);
    }
}
