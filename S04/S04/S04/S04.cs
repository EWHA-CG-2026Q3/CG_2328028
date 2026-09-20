using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class S04_CustomDiamondMesh : MonoBehaviour
{
    void Start()
    {
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0f, 0f, 0f),       // 0
            new Vector3(1f, 0f, 0f),       // 1
            new Vector3(1f, 0f, 1f),       // 2
            new Vector3(0f, 0f, 1f),       // 3
            new Vector3(0.5f, 1f, 0.5f),   // 4 위
            new Vector3(0.5f, -1f, 0.5f)   // 5 아래
        };

        int[] triangles = new int[]
        {
            // 위쪽 4개 면
            0, 4, 1,
            1, 4, 2,
            2, 4, 3,
            3, 4, 0,

            // 아래쪽 4개 면
            0, 1, 5,
            1, 2, 5,
            2, 3, 5,
            3, 0, 5
        };

        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;

        GetComponent<MeshRenderer>().sharedMaterial =
            new Material(Shader.Find("Universal Render Pipeline/Lit"));
    }
}