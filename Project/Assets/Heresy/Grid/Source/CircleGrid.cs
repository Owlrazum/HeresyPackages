using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public struct CircleGridJob : IJob
{
    public int outerLayerCount;
    public float totalRadius;
    public float layersCount;
    public float3 gridPos;

    public NativeArray<float3> buffer;
    public NativeArray<int> outPosCount;

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
        float radiusDelta = totalRadius / layersCount;
        float angleDeltaOuter = TAU / outerLayerCount;
        float arcLengthOuter = totalRadius * angleDeltaOuter;

        float layerRadius = totalRadius;
        outPosCount[0] = 0;

        for (int layer = 0; layer < layersCount; layer++)
        {
            float currentAngle = TAU / 4;
            float layerPerimeter = layerRadius * TAU;
            float arcLengthInLayer = layerPerimeter / arcLengthOuter;
            int numberOfTilesInLayer = UnityEngine.Mathf.RoundToInt(arcLengthInLayer);
            float angleDelta = TAU / numberOfTilesInLayer;

            for (int index = 0; index < numberOfTilesInLayer; index++)
            {
                float3 pos = new float3(
                    math.cos(currentAngle) * layerRadius, 0, 
                    math.sin(currentAngle) * layerRadius);

                buffer[outPosCount[0]++] = pos + gridPos;
                currentAngle -= angleDelta;
            }

            layerRadius -= radiusDelta;
        }
    }
}