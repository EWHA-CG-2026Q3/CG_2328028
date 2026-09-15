using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class S03_CustomPolygonMesh_Pentagon : MonoBehaviour
{
    void Start()
    {
        // TODO 1: 원하는 다각형의 정점 좌표
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0f, 1f, 0f),        // 0
            new Vector3(1f, 0.3f, 0f),      // 1
            new Vector3(0.6f, -0.8f, 0f),   // 2
            new Vector3(-0.6f, -0.8f, 0f),  // 3
            new Vector3(-1f, 0.3f, 0f)      // 4
        };

        // TODO 2: 정점 3개씩 묶어 삼각형 구성
        int[] triangles = new int[]
        {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4
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