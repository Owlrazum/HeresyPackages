using System;
using Unity.Collections;
using Unity.Mathematics;

namespace Orazum.MarchingCubes
{
    /// <summary>
    /// A 3-dimensional field of data
    /// </summary>
    public struct ScalarField<T> : IDisposable where T : struct
    {
        private NativeArray<T> _data;

        public int Width { get; }
        public int Height { get; }
        public int Depth { get; }

        public int Length => Width * Height * Depth;

        public ScalarField(int widthArg, int heightArg, int depthArg, Allocator allocator)
        {
#if UNITY_EDITOR
            if (widthArg < 0 || heightArg < 0 || depthArg < 0)
            {
                throw new ArgumentException("The dimensions of this scalarField must all be positive!");
            }
#endif

            _data = new NativeArray<T>(widthArg * heightArg * depthArg, allocator);

            Width = widthArg;
            Height = heightArg;
            Depth = depthArg;
        }

        public void Dispose()
        {
            _data.Dispose();
        }

        public void SetData(T data, int index)
        {
            _data[index] = data;
        }

        public bool TryGetData(int3 localPosition, out T data)
        {
            int index = XyzToIndex(localPosition, Width, Depth);
            if (index >= 0 && index < _data.Length)
            {
                data = GetData(index);
                return true;
            }
            data = default;
            return false;
        }

        public T GetData(int index)
        {
            return _data[index];
        }

        public int GetHeightIndex(int localPos)
        {
            return XyzToY(localPos, Width, Depth);
        }

        public int GetPlaneIndex(int localPos)
        {
            int x = XyzToX(localPos, Width);
            int y = XyzToZ(localPos, Width, Depth);
            return XyToIndex(x, y, Width);
        }

        public int GetPlaneIndex(int2 xy)
        {
            return XyToIndex(xy, Width);
        }


        public static int XyToIndex(int2 xy, int columnCount)
        {
            return xy.y * columnCount + xy.x;
        }

        public static int XyToIndex(int x, int y, int columnCount)
        {
            return y * columnCount + x;
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
    }
}