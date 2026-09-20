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
            0, 4, 1,
            1, 4, 2,
            2, 4, 3,
            3, 4, 0
        };
    }
}