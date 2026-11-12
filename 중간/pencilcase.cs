
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PencilHolderMesh : MonoBehaviour
{
    void OnEnable()
    {
        int sides = 8;
        float outerR = 0.12f;
        float innerR = 0.095f;
        float height = 0.25f;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // 반복문으로 팔각형의 정점 생성
        for (int i = 0; i < sides; i++)
        {
            float angle = 2f * Mathf.PI * i / sides;
            float x = Mathf.Cos(angle);
            float z = Mathf.Sin(angle);

            vertices.Add(new Vector3(x * outerR, 0f, z * outerR));
            vertices.Add(new Vector3(x * outerR, height, z * outerR));
            vertices.Add(new Vector3(x * innerR, 0.02f, z * innerR));
            vertices.Add(new Vector3(x * innerR, height, z * innerR));
        }

        // 바닥의 중심 정점
        int center = vertices.Count;
        vertices.Add(new Vector3(0f, 0.02f, 0f));

        // 반복문으로 삼각형 연결
        for (int i = 0; i < sides; i++)
        {
            int a = i * 4;
            int b = ((i + 1) % sides) * 4;

            // 바깥쪽 벽
            triangles.AddRange(new int[]
            {
                a, a + 1, b + 1,
                a, b + 1, b
            });

            // 안쪽 벽
            triangles.AddRange(new int[]
            {
                a + 2, b + 3, a + 3,
                a + 2, b + 2, b + 3
            });

            // 윗부분 테두리
            triangles.AddRange(new int[]
            {
                a + 1, a + 3, b + 3,
                a + 1, b + 3, b + 1
            });

            // 연필꽂이 내부 바닥
            triangles.AddRange(new int[]
            {
                center, b + 2, a + 2
            });
        }

        Mesh mesh = new Mesh();
        mesh.name = "OctagonalPencilHolder";
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
