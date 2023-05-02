using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public struct QuadGridJob : IJob
{
    public float quadSize;
    public float gapBetweenRows;
    public float gapBetweenColumns;
    public float3 gridPos;
    public int2 dims; // the columns are oscilating up and down.

    public NativeArray<float3> buffer;

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
        float3 deltaX = (quadSize + gapBetweenColumns) * math.right();
        float3 deltaY = (quadSize + gapBetweenRows) * math.forward();

        float3 initialDeltaX = -deltaX;
        float3 initialDeltaY = -deltaY;
        if (dims.y % 2 == 0)
        {
            initialDeltaX /= 2;
            initialDeltaY /= 2;
        }

        float3 startPos =
            -(dims.x / 2 - 1) * deltaX + initialDeltaX +
            -(dims.y / 2 - 1) * deltaY + initialDeltaY;
      
        float3 rowStart = startPos;
        float3 pos = startPos;

        for (int y = 0; y < dims.y; y++)
        {
            for (int x = 0; x < dims.x; x++)
            {
                buffer[XyToIndex(x, y, dims.x)] = pos + gridPos;
                pos += deltaX;
            }
            pos = rowStart;
            pos += deltaY;
            rowStart = pos;
        }
    }
}