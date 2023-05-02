using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;

public class GridTester : MonoBehaviour
{
    [SerializeField]
    bool testHex;
    [SerializeField]
    bool testQuad;
    [SerializeField]
    bool testCircle;

    [SerializeField]
    int2 hexDims = new int2(7, 21);
    [SerializeField]
    int2 quadDims = new int2(10, 10);
    [SerializeField]
    int2 circleDims = new int2(7, 3);

    [SerializeField]
    GameObject prefab;

    void Start()
    {
        JobHandle hexHandle = new();
        NativeArray<float3> hexBuffer = new();

        JobHandle quadHandle = new();
        NativeArray<float3> quadBuffer = new();
        JobHandle circleHandle = new();
        NativeArray<float3> circleBuffer = new();
        NativeArray<int> outPosCount = new();

        if (testHex)
        {
            hexBuffer = new(hexDims.x * hexDims.y, Allocator.TempJob);
            HexGridJob job = new()
            {
                gridPos = float3.zero
            ,
                gapSize = 0
            ,
                hexagonSize = 5
            ,
                dims = hexDims
            ,
                buffer = hexBuffer
            };

            hexHandle = job.Schedule();
        }

        if (testQuad)
        {
            quadBuffer = new(quadDims.x * quadDims.y, Allocator.TempJob);
            QuadGridJob job = new()
            {
                gridPos = float3.zero
            ,
                quadSize = 10
            ,
                dims = quadDims
            ,
                buffer = quadBuffer
            };

            quadHandle = job.Schedule();
        }

        if (testCircle)
        { 
            circleBuffer = new(circleDims.x * circleDims.y, Allocator.TempJob);
            outPosCount = new(1, Allocator.TempJob);
            CircleGridJob job = new()
            {
                gridPos = float3.zero
                         ,
                outerLayerCount = circleDims.x
                         ,
                totalRadius = 20
                         ,
                layersCount = circleDims.y
                         ,
                buffer = circleBuffer
                         ,
                outPosCount = outPosCount
            };

            circleHandle = job.Schedule();
        }

        if (testHex)
        {
            hexHandle.Complete();

            GameObject parent = new GameObject();

            foreach (float3 pos in hexBuffer)
            {
                Instantiate(prefab, pos, Quaternion.identity, parent.transform);
            }

            hexBuffer.Dispose();
        }

        if (testQuad)
        {
            quadHandle.Complete();

            GameObject parent = new GameObject();

            foreach (float3 pos in quadBuffer)
            {
                Instantiate(prefab, pos, Quaternion.identity, parent.transform);
            }

            quadBuffer.Dispose();
        }

        if (testCircle)
        {
            circleHandle.Complete();

            GameObject parent = new GameObject();

            for (int i = 0; i < outPosCount[0]; i++)
            {
                Instantiate(prefab, circleBuffer[i], Quaternion.identity, parent.transform);
            }

            circleBuffer.Dispose();
            outPosCount.Dispose();
        }
    }
}