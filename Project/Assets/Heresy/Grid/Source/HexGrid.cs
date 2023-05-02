using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public struct HexGridJob : IJob
{
    public float3 gridPos;
    public float gapSize;
    public float hexagonSize; // Radius of outer circle
    public int2 dims; // the columns are oscilating up and down.

    public NativeArray<float3> buffer;

    private const float TAU = 6.28318530717959f;
    private static int XyToIndex(int2 xy, int columnCount)
    {
            return xy.y * columnCount + xy.x;
    }
    private static int XyToIndex(int x, int y, int columnCount)
    {
            return y * columnCount + x;
    }

    /// <link="https://www.redblobgames.com/grids/hexagons/#coordinates"></link>
    public void Execute()
    {
        // radius of inner circleOfHexagon can help understand
        float cos30 = math.cos(TAU / 12);

        float innerRadius = cos30 * hexagonSize;
        float3 deltaX = (innerRadius + gapSize / 2) * math.right();

        float columnDeltaY = cos30 * (innerRadius * 2 + gapSize);
        float3 columnUp = columnDeltaY * math.forward();
        float3 columnDown = -columnUp;

        float3 rowDeltaY = columnUp * 2;

        float3 initial =
            -(dims.x / 2) * deltaX +
            -(dims.y / 2) * rowDeltaY;

        float3 rowStart = initial;
        float3 pos = initial;

        bool shouldUp = true;

        for (int y = 0; y < dims.y; y++)
        {
            for (int x = 0; x < dims.x; x++)
            {
                int index = XyToIndex(x, y, dims.x);
                buffer[index] = pos + gridPos;
                pos += deltaX;
                pos += shouldUp ? columnUp : columnDown;
                shouldUp = !shouldUp;
            }
            pos = rowStart;
            pos += rowDeltaY;
            rowStart = pos;

            shouldUp = true;
        }
    }
}

// produces interesting result with hexagon mesh
    // Quaternion tileRot = Quaternion.Euler(0, 90, 0); 
