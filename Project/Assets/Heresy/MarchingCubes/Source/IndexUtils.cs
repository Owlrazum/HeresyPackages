using System;
using Unity.Collections;
using Unity.Mathematics;

namespace Orazum.MarchingCubes
{
    /// <summary>
    /// Blender style coordinate system. 
    /// Construct row x axis, construct column z axis, construct height y axis
    /// </summary>
    static class IndexUtils
    {
 
        public static int2 IndexToXy(int index, int columnCount)
        {
            return new int2(index % columnCount, index / columnCount);
        }

        public static int XyToIndex(int2 xy, int columnCount)
        {
            return xy.y * columnCount + xy.x;
        }

        public static int XyToIndex(int x, int y, int columnCount)
        {
            return y * columnCount + x;
        }

        public static int3 IndexToXyz(int index, int columnCount, int rowCount)
        {
            int3 position = new int3(
                index % columnCount,
                index / (columnCount * rowCount),
                index / columnCount % rowCount
            );
            return position;
        }

        public static int XyzToIndex(int3 xyz, int columnCount, int rowCount)
        {
            return xyz.y * columnCount * rowCount + xyz.z * columnCount + xyz.x;
        }

        public static int XyzToIndex(int x, int y, int z, int columnCount, int rowCount)
        {
            return y * columnCount * rowCount + z * columnCount + x;
        }

        public static int XyzToX(int xyz, int columnCount)
        {
            return xyz % columnCount;
        }

        public static int XyzToY(int xyz, int columnCount, int rowCount)
        {
            return xyz / (columnCount * rowCount);
        }

        public static int XyzToZ(int xyz, int columnCount, int rowCount)
        {
            return xyz / columnCount % rowCount;
        }

        /// <summary>
        /// Imagine an index pointing to element in 3d array. 
        /// The function returns the index of the same element, as if
        /// reduceCount of last elements per row and plane were removed:
        /// reduceCount elements per row, reduceCount rows per plane. 
        /// The index should not point to element that was removed.
        /// </summary>
        public static int ReduceIndex3D(int index, int3 dims, int reduceCount)
        {
            int3 pos = IndexUtils.IndexToXyz(index, dims.x, dims.z);
            return IndexUtils.XyzToIndex(pos, dims.x - reduceCount, dims.z - reduceCount);
        }
    }

    namespace Test
    {
        public static class IndexUtilsTest
        { 

        }
    }
}