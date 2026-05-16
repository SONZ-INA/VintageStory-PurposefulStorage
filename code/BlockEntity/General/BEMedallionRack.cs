namespace PurposefulStorage;

public class BEMedallionRack : BEBasePSContainer {
    public override string AttributeTransformCode => "onMedallionsTransform";
    public override string[] AttributeCheck => ["psMedallions"];

    protected override InfoDisplayOptions InfoDisplay => InfoDisplayOptions.BySegment;

    public override int[] SectionSegmentCounts => [9];

    public BEMedallionRack() { inv = new InventoryGeneric(SlotCount, InventoryClassName + "-0", Api, (_, inv) => new ItemSlotPSUniversal(inv, AttributeCheck)); }

    protected override float[][] genTransformationMatrices() {
        return TransformationGenerator.GenerateLayout(this, td => {
            td.x = td.segment % 3 * 0.3125f;
            td.y = td.segment / 3 * 0.0825f;
            td.z = td.segment / 3 * -0.3125f;
            
            td.offsetX = -0.2825f;
            td.offsetY = 0.085f;
            td.offsetZ = 0.35f;
            
            td.offsetRotY = 90;
            td.offsetRotZ = 15;
        });
    }
}
