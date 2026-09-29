namespace PurposefulStorage;

public class BEJonasPartsCabinet : BEBasePSContainer {
    public override string[] AttributeCheck => ["psJonasParts"];

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    protected override string CantPlaceMessage => "purposefulstorage:Can't place this item here, it's either not the right type, or not the right size for this section.";

    public override int[] SectionSegmentCounts => [13];

    public BEJonasPartsCabinet() {
        inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (id, inv) => {
            if (id != 6) return new ItemSlotPSUniversal(inv, AttributeCheck, 1, true);
            else return new ItemSlotPSUniversal(inv, new AssetLocation("game:jonasframes-gearbox02"), 1, true);
        });
    }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.offsetX = -0.275f;
            td.offsetY = 0.0275f;
            td.offsetZ = -0.2f;

            if (td.index < 6) {
                td.x = td.index / 2 * 0.42f;
                td.y = td.index % 2 * 0.5f;
            }

            if (td.index == 6) {
                td.x = 1.26f;
            }

            if (td.index > 6) {
                td.x = (td.index + 1) / 2 * 0.42f;
                td.y = td.index % 2 * 0.5f;
            }
        });
    }
}
