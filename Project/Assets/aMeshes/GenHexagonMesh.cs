using UnityEditor;
using UnityEngine;

namespace Orazum.Heresy.Primitives
{
    public class GenHexagonMesh : MonoBehaviour
    {
        [SerializeField]
        private float radius = 1f;
        [SerializeField]
        private float height = 0.5f;

        Vector3[] vertices;
        Vector2[] uvs;
        int[] indices;
        //TODO: There are problems with how material is displayed. Normals?

        void Start()
        {
            if (height == 0)
            {
                Generate2DHex();
            }
            else
            { 
                Generate3DHex();
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            //  mesh.uv = uvs;

            AssetDatabase.CreateAsset(mesh, "Assets/_Game/Meshes/Hexagon.asset");
            AssetDatabase.SaveAssets();

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter)
            {
                meshFilter.mesh = mesh;
            }

        }

        void Generate2DHex()
        { 

        }

        void Generate3DHex()
        {
            Vector3[] vertices = new Vector3[14];
            Vector2[] uvs = new Vector2[14];
            for (int h = 0; h < 2; h++)
            {
                Vector3 centerPos = new Vector3(0, h * height, 0);
                vertices[h * 7] = centerPos;
                for (int i = 0; i < 6; i++)
                {
                    float angle = Mathf.PI / 2 - i * Mathf.PI / 3;
                    Vector3 pos = new Vector3(Mathf.Cos(angle), h * height, Mathf.Sin(angle));
                    pos *= radius;
                    vertices[i + 1 + h * 7] = pos;
                    // }
                }
                //uvs[0] = new Vector2(0.5f, 1);
                //uvs[1] = new Vector2(1, 0);
                //uvs[2] = new Vector2(0, 0);

                //uvs[3] = new Vector2(0.5f, 1);
                //uvs[4] = new Vector2(1, 0);
                //uvs[5] = new Vector2(0, 0);


                int[] hexTriangles =
                {  
//                      |           |           |          |           | 
                 7, 8, 9, 7,  9, 10,  7, 10, 11, 7, 11, 12,  7, 12, 13, 7, 13,  8, // upPart
                 0, 2, 1, 0,  3,  2,  0,  4,  3, 0,  5,  4,  0,  6,  5, 0,  1,  6, // botPart
                 8, 1, 2, 2,  9,  8,  9,  2,  3, 3, 10,  9, 10,  3,  4, 4, 11, 10, // 3 sides
                11, 4, 5, 5, 12, 11, 12,  5,  6, 6, 13, 12, 13,  6,  1, 1,  8, 13                                                              // 3 sides
            };
                //{ 3, 4, 5, 0, 2, 1, 0, 3, 2, 3, 5, 2, 3, 0, 1, 3, 1, 4, 2, 5, 1, 5, 4, 1 };
            }
        }
    }
}

