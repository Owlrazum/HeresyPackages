using UnityEngine; 
using Unity.Mathematics;
using Orazum.MarchingCubes;
using System.Collections.Generic;

class Cube
{
    public GameObject gb;
    public MeshRenderer rend;
    public bool marked;
}

public class TestIndexUtils : MonoBehaviour
{
    public int dims = 10;
    public int gap = 1;

    public Material idle;
    public Material reduced;
    public Material selected;

    public int3 indexToSelect;
    public int reduceAmount;

    Mesh cubeMesh;

    List<Cube> list;

    void Awake()
    {
        GameObject dummy = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubeMesh = dummy.GetComponent<MeshFilter>().sharedMesh;
        Destroy(dummy);

        CreateGrid();
        Select();
        ReduceGrid();
        SelectReduced();
    }

    void CreateGrid()
    {
        list = new(dims * dims * dims);
        for (int y = 0; y < dims; y++)
        {
            for (int z = 0; z < dims; z++)
            {
                for (int x = 0; x < dims; x++)
                {
                    int3 pos = new int3(x, y, z);
                    list.Add(CreateCube(pos, gap));
                }
            }
        }
    }
    Cube CreateCube(int3 pos, int gap)
    {
        GameObject gb = new("Cube");
        var rend = gb.AddComponent<MeshRenderer>();
        var filter = gb.AddComponent<MeshFilter>();

        gb.transform.position = new Vector3(
            pos.x + gap * pos.x, pos.y + gap * pos.y, pos.z + gap * pos.z);
        rend.sharedMaterial = idle;
        filter.sharedMesh = cubeMesh;

        Cube cube = new();
        cube.gb = gb;
        cube.rend = rend;
        cube.marked = false;

        return cube;
    }
    void Select()
    {
        list[IndexUtils.XyzToIndex(indexToSelect, dims, dims)].rend.sharedMaterial = selected;
    }
    void ReduceGrid()
    {
        for (int y = 0; y < dims - reduceAmount; y++)
        { 
            for (int z = 0; z < dims - reduceAmount; z++)
            {
                for (int x = dims - 1; x >= dims - reduceAmount; x--)
                {
                    Reduce(x, y, z);
                }
            }
        }

        for (int y = 0; y < dims - reduceAmount; y++)
        {
            for (int x = 0; x < dims; x++)
            {
                for (int z = dims - 1; z >= dims - reduceAmount; z--)
                {
                    Reduce(x, y, z);
                }
            }
        }

        for (int z = 0; z < dims; z++)
        {
            for (int x = 0; x < dims; x++)
            {
                for (int y = dims - 1; y >= dims - reduceAmount; y--)
                {
                    Reduce(x, y, z);
                }
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].marked)
            {
                list.RemoveAt(i);
                i--;
            }
        }
    }
    void Reduce(int x, int y, int z)
    {
        int index = IndexUtils.XyzToIndex(x, y, z, dims, dims);
        list[index].rend.sharedMaterial = reduced;
        list[index].marked = true;
    }
    void SelectReduced()
    {
        int index = IndexUtils.XyzToIndex(indexToSelect, dims, dims);
        int reducedIndex = IndexUtils.ReduceIndex3D(index, dims, reduceAmount);
        Debug.Log(index + " " + reducedIndex);
        list[reducedIndex].rend.sharedMaterial = selected;
    }
}