using System.Linq;

namespace PurposefulStorage;

public readonly record struct ExplicitTransform(
    float?[] X, float?[] Y, float?[] Z,
    float?[] RX, float?[] RY, float?[] RZ
) {
    public int Length => new[] {
        X?.Length ?? 0,
        Y?.Length ?? 0,
        Z?.Length ?? 0,
        RX?.Length ?? 0,
        RY?.Length ?? 0,
        RZ?.Length ?? 0
    }.Max();
}
