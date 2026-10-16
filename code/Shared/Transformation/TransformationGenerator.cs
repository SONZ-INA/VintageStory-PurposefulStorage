namespace PurposefulStorage;

/// <summary>
/// Helper class for generating transformation matrices used to render items inside containers. <br />
/// Supports both grid-based placement (shelves/segments) and fully manual (explicit) placement.
/// </summary>
public static class TransformationGenerator {
    /// <summary>
    /// Generates transformation matrices using the container's grid structure (sections & their items). <br />
    /// The provided accessor defines how each slot is positioned, and optional item layouts adjust placement per segment, depending on the item inside.
    /// </summary>
    public static float[][] GenerateLayout(BEBasePSContainer be, Action<TransformationData> accessor) {
        float[][] tfMatrices = new float[be.SlotCount][];

        TransformationData td = new() {
            preRotate = be.Block.GetRotationAngle()
        };

        int index = 0;

        for (int section = 0; section < be.SectionSegmentCounts.Length; section++) {
            int segmentsInSection = be.SectionSegmentCounts[section];

            for (int segment = 0; segment < segmentsInSection; segment++) {
                for (int item = 0; item < be.ItemsPerSegment; item++) {

                    if (index >= be.SlotCount) break;

                    var slot = be.inv[index];

                    if (slot.Empty) {
                        tfMatrices[index] = new Matrixf().Values;
                        index++;
                        continue;
                    }

                    td.Reset();
                    td.index = index;
                    td.section = section;
                    td.segment = segment;
                    td.item = item;

                    accessor(td);

                    tfMatrices[index] = td.BuildMatrix();
                    index++;
                }
            }
        }

        return tfMatrices;
    }

    /// <summary>
    /// Like <see cref="GenerateLayout"/>, but td.item is the index of the physical item within the slot's stack.<br />
    /// The number of physical items scales with how full the stack is, capped at <paramref name="limit"/>.
    /// Returns one array of matrices per slot (empty array for empty slots).
    /// </summary>
    public static float[][][] GenerateStackLayout(BEBasePSContainer be, int limit, Action<TransformationData> accessor) {
        float[][][] tfMatrices = new float[be.SlotCount][][];

        TransformationData td = new() {
            preRotate = be.Block.GetRotationAngle()
        };

        int index = 0;

        for (int section = 0; section < be.SectionSegmentCounts.Length; section++) {
            int segmentsInSection = be.SectionSegmentCounts[section];

            for (int segment = 0; segment < segmentsInSection; segment++) {
                for (int slotInSegment = 0; slotInSegment < be.ItemsPerSegment; slotInSegment++) {

                    if (index >= be.SlotCount) break;

                    var slot = be.inv[index];
                    int visible = GetVisibleItemCount(slot, limit);

                    if (visible == 0) {
                        tfMatrices[index] = [];
                        index++;
                        continue;
                    }

                    float[][] stack = new float[visible][];

                    for (int stackItem = 0; stackItem < visible; stackItem++) {
                        td.Reset();
                        td.index = index;
                        td.section = section;
                        td.segment = segment;
                        td.item = slotInSegment;
                        td.stackItem = stackItem;

                        accessor(td);

                        stack[stackItem] = td.BuildMatrix();
                    }

                    tfMatrices[index] = stack;
                    index++;
                }
            }
        }

        return tfMatrices;
    }

    /// <summary>
    /// Generates transformation matrices from a predefined set of positions and rotations.<br />
    /// Each slot is placed exactly according to the given matrix, with an optional modifier for small adjustments.
    /// </summary>
    public static float[][] GenerateExplicit(ExplicitTransform transform, Action<TransformationData>? modifier = null) {
        int count = transform.Length;
        float[][] tfMatrices = new float[count][];
        TransformationData td = new();

        for (int i = 0; i < count; i++) {
            td.Reset();
            td.index = i;

            td.x = GetValue(transform.X, i);
            td.y = GetValue(transform.Y, i);
            td.z = GetValue(transform.Z, i);

            td.offsetRotX = GetValue(transform.RX, i);
            td.offsetRotY = GetValue(transform.RY, i);
            td.offsetRotZ = GetValue(transform.RZ, i);

            modifier?.Invoke(td);

            tfMatrices[i] = td.BuildMatrix();
        }

        return tfMatrices;
    }

    /// <summary>
    /// How many physical items to show. Scales proportionally with fill level, at least 1 if non-empty, never above limit.
    /// </summary>
    public static int GetVisibleItemCount(ItemSlot slot, int limit) {
        if (slot.Empty || limit <= 0) return 0;

        int capacity = slot.StackSize + slot.GetRemainingSlotSpace(slot.Itemstack);
        if (capacity <= 0) return 1;

        int visible = (int)Math.Ceiling(slot.StackSize * (double)limit / capacity);
        return Math.Clamp(visible, 1, limit);
    }
}
