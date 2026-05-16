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
    /// Generates transformation matrices from a predefined set of positions and rotations. <br />
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
}
