using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class S03_CustomPolygonMesh_Pentagon : MonoBehaviour
{
    void Start()
    {
        // TODO 1: 원하는 다각형의 정점 좌표를 채우세요 (최소 4개)
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0f, 1f, 0f),        // 0 위
            new Vector3(1f, 0.3f, 0f),      // 1 오른쪽 위
            new Vector3(0.6f, -0.8f, 0f),   // 2 오른쪽 아래
            new Vector3(-0.6f, -0.8f, 0f),  // 3 왼쪽 아래
            new Vector3(-1f, 0.3f, 0f)      // 4 왼쪽 위
        };
    }
}