using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;

using UnityEngine;
using UnityEngine.Rendering;

using Orazum.MarchingCubes;

//TODO: replace author of heresy commits

enum SizeMethod
{
    ByGrid,
    ByCell
}

/// <summary>
/// Can be served as an example. Generates metaball-like behaviour
/// </summary>
public class TestMarchingCubes : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    float isoLevel = 0.5f;

    [SerializeField]
    Transform[] transformsToUse;

    [SerializeField]
    int dims = 20;

    [SerializeField]
    float radius = 2;

    [SerializeField]
    SizeMethod sizeMethod = SizeMethod.ByGrid;

    [SerializeField]
    float cellSize;

    [SerializeField]
    float gridSize = 10;

    NativeArray<VertexData> vertices;
    NativeArray<short> indices;
    NativeArray<int> verticesCountRange;
    NativeArray<int> verticesCount;

    NativeArray<float3> transformPositions;
    NativeArray<Corner> scalarField;

    Mesh mesh;

    int cellsCount;
    int cubesCount;

    InitializeScalarField scalarFieldInit;
    GenerateMarchineCubes generate;
    FillVertexBuffer fillVertexBuffer;

    JobHandle scalarFieldHandle;
    JobHandle generateHandle;
    JobHandle verticesHandle;

    void Start()
    {
        cellsCount = dims * dims * dims;
        cubesCount = (dims - 1) * (dims - 1) * (dims - 1);

        // the algorithm will create at most 5 triangles per cell

        vertices = new(cubesCount * 15, Allocator.Persistent);
        indices = new(vertices.Length, Allocator.Persistent);

        verticesCountRange = new(cubesCount, Allocator.Persistent);
        verticesCount = new(1, Allocator.Persistent);

        transformPositions = new(transformsToUse.Length, Allocator.Persistent);

        scalarField = new NativeArray<Corner>(cellsCount, Allocator.Persistent);
        if (sizeMethod == SizeMethod.ByGrid)
        {
            cellSize = gridSize / dims;
        }
        else
        {
            gridSize = cellSize * dims;
        }

        mesh = GetComponent<MeshFilter>().mesh;
        mesh.MarkDynamic();

        scalarFieldInit = new()
        {
            scalarFieldOut = scalarField
            , dims = dims
            , cellSize = cellSize
            , gridSize = gridSize
            , posIn = transformPositions
        };

        generate = new()
        {
            scalarFieldIn = scalarField
            , verticesOut = vertices
            , vertexCountOut = verticesCountRange
            , gridDims = dims
        };

        for (int i = 0; i < vertices.Length; i++)
        {
            indices[i] = (short)i;
        }
        fillVertexBuffer = new()
        {
            vertices = vertices
            , verticesCountRangeIn = verticesCountRange
            , verticesCountOut = verticesCount
        };
    }

    void Update()
    {
        scalarFieldInit.gridPos = transform.position;
        for (int i = 0; i < transformsToUse.Length; i++)
        {
            transformPositions[i] = transformsToUse[i].position;
        }
        scalarFieldInit.distanceDenominator = radius;
        scalarFieldHandle = scalarFieldInit.ScheduleParallel(cellsCount, 32, default);

        generate.isoLevel = isoLevel;
        generateHandle = generate.ScheduleParallel(cellsCount, cubesCount, scalarFieldHandle);

        verticesHandle = fillVertexBuffer.Schedule(generateHandle);
        JobHandle.ScheduleBatchedJobs();
    }

    void LateUpdate()
    {
        scalarFieldHandle.Complete();
        generateHandle.Complete();
        verticesHandle.Complete();

        GenerateMeshImmediate();
    }

    public void GenerateMeshImmediate()
    {
        int indexCount = verticesCount[0];

        mesh.SetVertexBufferParams(verticesCount[0], VertexData.VertexBufferMemoryLayout);
        mesh.SetIndexBufferParams(indexCount, IndexFormat.UInt16);

        mesh.SetVertexBufferData(vertices, 0, 0, verticesCount[0], 0, MeshUpdateFlags.DontValidateIndices);
        mesh.SetIndexBufferData(indices, 0, 0, indexCount, MeshUpdateFlags.DontValidateIndices);

        mesh.subMeshCount = 1;
        SubMeshDescriptor subMesh = new SubMeshDescriptor(
            indexStart: 0,
            indexCount: indexCount
        );
        mesh.SetSubMesh(0, subMesh);

        // mesh.RecalculateNormals(); is not needed because the vertices are not shared/
        mesh.RecalculateBounds();
    }

    private bool areDisposed;
    private void OnDestroy()
    {
        if (!areDisposed)
        {
            scalarField.Dispose();
            vertices.Dispose();
            verticesCount.Dispose();
            indices.Dispose();
            verticesCountRange.Dispose();
            transformPositions.Dispose();
            areDisposed = true;
        }
    }
}

[BurstCompile]
struct InitializeScalarField : IJobFor
{
    [WriteOnly]
    public NativeArray<Corner> scalarFieldOut;

    [ReadOnly]
    public NativeArray<float3> posIn;

    public int3 dims;

    public float cellSize;
    public float gridSize;
    public float3 gridPos;
    public float distanceDenominator;

    public void Execute(int index)
    {
        int3 indexPos = IndexUtils.IndexToXyz(index, dims.x, dims.z);
        float3 pos = gridPos + indexPos * new float3(cellSize) - gridSize / 2;
        Corner corner = new();
        corner.pos = pos;

        float v = 0;
        for (int i = 0; i < posIn.Length; i++)
        {
            float d = math.distance(pos, posIn[i]);
            if (d > distanceDenominator)
            {
                continue;
            }

            v += (distanceDenominator - d) / distanceDenominator;
        }
        v = math.clamp(v, 0, 1);
        corner.value = v;
        scalarFieldOut[index] = corner;
    }   
}