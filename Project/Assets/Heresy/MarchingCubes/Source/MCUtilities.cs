using Unity.Mathematics;
using Unity.Collections;

namespace Orazum.MarchingCubes
{ 
    public static class MCUtilities
    {
        /// <summary>
        /// Interpolates the vertex's position.
        /// p - corner.
        /// v - density.
        /// isolevel - The density level where a surface will be created.
        /// Densities below this will be inside the surface (solid),
        /// and densities above this will be outside the surface (air)
        /// </summary>
        /// <returns>The interpolated vertex's position</returns>
        public static float3 VertexInterpolate(float3 p1, float3 p2, float v1, float v2, float isolevel)
        {
            return p1 + (isolevel - v1) * (p2 - p1) / (v2 - v1);
        }

        public static byte CalculateCubeIndex(MarchingCube<(float3 pos, byte value)> voxelCorners, byte isoLevel)
        { 
            byte cubeIndex = (byte)math.select(0, 1,   voxelCorners.Corner1.value < isoLevel);
            cubeIndex     |= (byte)math.select(0, 2,   voxelCorners.Corner2.value < isoLevel);
            cubeIndex     |= (byte)math.select(0, 4,   voxelCorners.Corner3.value < isoLevel);
            cubeIndex     |= (byte)math.select(0, 8,   voxelCorners.Corner4.value < isoLevel);
            cubeIndex     |= (byte)math.select(0, 16,  voxelCorners.Corner5.value < isoLevel);
            cubeIndex     |= (byte)math.select(0, 32,  voxelCorners.Corner6.value < isoLevel);
            cubeIndex     |= (byte)math.select(0, 64,  voxelCorners.Corner7.value < isoLevel);
            cubeIndex     |= (byte)math.select(0, 128, voxelCorners.Corner8.value < isoLevel);

            return cubeIndex;
        }

        public static MarchingCube<(float3, byte)> GetMarchingCube(
            ScalarField<(float3, byte)> scalarField, 
            int3 localPosition,
            float cellSize
        )
        { 
            MarchingCube<(float3, byte)> voxelCorners = new MarchingCube<(float3, byte)>();
            for (int i = 0; i < 8; i++)
            {
                int3 cubeCorner = localPosition + LookupTables.CubeCorners[i];
                if (scalarField.TryGetData(cubeCorner, out (float3 pos, byte value) data))
                {
                    voxelCorners[i] = (data.pos, data.value);
                }
            }
            return voxelCorners;
        }

        public static VertexList GenerateVertexList(
            MarchingCube<(float3 pos, byte value)> marchingCube, 
            int edgeIndex, 
            byte isoLevel
        )
        { 
            VertexList vertexList = new VertexList();

            for (int i = 0; i < 12; i++)
            {
                if ((edgeIndex & (1 << i)) == 0) { continue; }

                int edgeStartIndex = LookupTables.EdgeIndexTable[2 * i + 0];
                int edgeEndIndex = LookupTables.EdgeIndexTable[2 * i + 1];

                float3 corner1 = marchingCube[edgeStartIndex].pos;
                float3 corner2 = marchingCube[edgeEndIndex].pos;

                float density1 = marchingCube[edgeStartIndex].value / 255f;
                float density2 = marchingCube[edgeEndIndex].value / 255f;

                vertexList[i] = VertexInterpolate(corner1, corner2, density1, density2, isoLevel / 255f);
            }

            return vertexList;
        }
    }
}