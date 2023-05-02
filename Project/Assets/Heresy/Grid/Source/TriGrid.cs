using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public struct TriGridJob : IJob
{
    //Currently not used in calcs. Radius of outer circle
    public float triangleSize;
    public float colsGap;
    public float rowsGap;

    public float gridPos;
    public int2 dims;
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

    public void Execute()
    {
        
    }
}

// produces interesting result with hexagon mesh
// Quaternion tileRot = Quaternion.Euler(0, 90, 0); 
