using UnityEngine;

// 동차좌표와 4x4 행렬을 이용해 shear 변환을 적용
[ExecuteAlways]
[RequireComponent(typeof(DiamondMesh))]
public class S09_TRS_Raw_Finish : MonoBehaviour
{
    // 본인 학번 끝자리로 계산한 k값을 Inspector에서 입력
    // k = (학번 끝자리 + 1) / 5
    [SerializeField] float k = 1f;

    DiamondMesh diamondMesh;

    void OnEnable()
    {
        diamondMesh = GetComponent<DiamondMesh>();
    }

    void Update()
    {
        if (diamondMesh == null || diamondMesh.BaseVertices == null)
            return;

        Vector3[] verts = ApplyShearRaw(diamondMesh.BaseVertices, k);
        diamondMesh.SetVertices(verts);
    }

    // Inspector에서 k값을 바꿀 때
    // 꼭대기 정점 (0.5, 1, 0.5)의 변환 결과를 Console에 출력
    void OnValidate()
    {
        Vector3 topVertex = new Vector3(0.5f, 1f, 0.5f);

        Vector4 h = ToHomogeneous(topVertex);
        float[,] H = ShearMatrixRaw(k);

        h = MultiplyMatrixVectorRaw(H, h);

        Debug.Log(
            $"k = {k}, 꼭대기 정점 (0.5, 1, 0.5) → {FromHomogeneous(h)}"
        );
    }


    // =========================
    // Shear 행렬
    // =========================

    float[,] ShearMatrixRaw(float k)
    {
        return new float[,]
        {
            { 1f, k,  0f, 0f },
            { 0f, 1f, 0f, 0f },
            { 0f, 0f, 1f, 0f },
            { 0f, 0f, 0f, 1f }
        };
    }


    // =========================
    // 동차좌표
    // =========================

    Vector4 ToHomogeneous(Vector3 v)
    {
        return new Vector4(v.x, v.y, v.z, 1f);
    }

    Vector3 FromHomogeneous(Vector4 h)
    {
        return new Vector3(h.x, h.y, h.z);
    }


    // =========================
    // 4x4 행렬 x 벡터
    // =========================

    Vector4 MultiplyMatrixVectorRaw(float[,] M, Vector4 v)
    {
        float[] input =
        {
            v.x, v.y, v.z, v.w
        };

        float[] result = new float[4];

        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                result[row] += M[row, col] * input[col];
            }
        }

        return new Vector4(
            result[0],
            result[1],
            result[2],
            result[3]
        );
    }


    // =========================
    // 모든 정점에 Shear 적용
    // =========================

    Vector3[] ApplyShearRaw(Vector3[] baseVertices, float k)
    {
        float[,] H = ShearMatrixRaw(k);

        Vector3[] verts =
            new Vector3[baseVertices.Length];

        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector4 h =
                ToHomogeneous(baseVertices[i]);

            h =
                MultiplyMatrixVectorRaw(H, h);

            verts[i] =
                FromHomogeneous(h);
        }

        return verts;
    }
}