
using UnityEngine;
using UnityEngine.UI;

// S12: 빈 오브젝트 VirtualCamera가 본 장면을 원근투영으로 캔버스에 그림
// S11_VirtualCamera_Finish.cs에서 바뀐 곳
//   - size 대신 fieldOfView (세로 각도)
//   - BuildProjectionMatrix 추가
//   - ToPixel: size로 나누던 두 줄을 "P 곱하기와 w로 나누기"로 바꿈
//   - DrawLine: 캔버스 밖 픽셀은 칠하지 않고 건너뜀
//   - OnDrawGizmos: 직육면체 상자 대신 Frustum
public class S12_VirtualCamera_Finish : MonoBehaviour
{
    [Header("그릴 오브젝트")]
    public MeshFilter[] targets;

    [Header("카메라가 담는 범위")]
    [Range(1f, 179f)] public float fieldOfView = 90f;
    public float near = 0.3f;
    public float far = 20f;

    [Header("캔버스")]
    public RawImage canvasImage;
    public int width = 400;
    public int height = 300;
    public Color lineColor = Color.white;
    public Color clearColor = new Color(0f, 0f, 0f, 0f);

    Texture2D canvas;
    Color[] clearPixels;
    Matrix4x4 P;

    void Start()
    {
        canvas = new Texture2D(width, height, TextureFormat.RGBA32, false);
        canvas.filterMode = FilterMode.Point;
        canvas.wrapMode = TextureWrapMode.Clamp;

        clearPixels = new Color[width * height];

        for (int i = 0; i < clearPixels.Length; i++)
        {
            clearPixels[i] = clearColor;
        }

        if (canvasImage != null)
        {
            canvasImage.texture = canvas;
        }
    }

    void LateUpdate()
    {
        DrawSceneOnCanvas();
    }

    // V = R^-1 * T^-1
    Matrix4x4 BuildViewMatrix(Transform cam)
    {
        Matrix4x4 Tinv = Matrix4x4.Translate(-cam.position);
        Matrix4x4 Rinv = Matrix4x4.Rotate(Quaternion.Inverse(cam.rotation));

        return Rinv * Tinv;
    }

    // 원근 투영 행렬 P
    Matrix4x4 BuildProjectionMatrix()
    {
        // 원래 값으로 복구 완료
        float aspect = (float)width / height;

        float t = Mathf.Tan(fieldOfView * 0.5f * Mathf.Deg2Rad);

        Matrix4x4 m = Matrix4x4.zero;

        m.m00 = 1f / (t * aspect);
        m.m11 = 1f / t;
        m.m32 = 1f;

        return m;
    }

    void DrawSceneOnCanvas()
    {
        if (canvas == null) return;

        ClearCanvas();

        Matrix4x4 V = BuildViewMatrix(transform);
        P = BuildProjectionMatrix();

        foreach (MeshFilter mf in targets)
        {
            if (mf == null || mf.sharedMesh == null)
                continue;

            Matrix4x4 VM = V * mf.transform.localToWorldMatrix;

            Vector3[] verts = mf.sharedMesh.vertices;
            int[] tris = mf.sharedMesh.triangles;

            for (int i = 0; i < tris.Length; i += 3)
            {
                Vector3 a = VM.MultiplyPoint(verts[tris[i]]);
                Vector3 b = VM.MultiplyPoint(verts[tris[i + 1]]);
                Vector3 c = VM.MultiplyPoint(verts[tris[i + 2]]);

                DrawEdge(a, b);
                DrawEdge(b, c);
                DrawEdge(c, a);
            }
        }

        canvas.Apply();
    }

    void ClearCanvas()
    {
        canvas.SetPixels(clearPixels);
    }

    void DrawEdge(Vector3 a, Vector3 b)
    {
        if (!ToPixel(a, out Vector2 pa)) return;
        if (!ToPixel(b, out Vector2 pb)) return;

        DrawLine(
            Mathf.RoundToInt(pa.x),
            Mathf.RoundToInt(pa.y),
            Mathf.RoundToInt(pb.x),
            Mathf.RoundToInt(pb.y)
        );
    }

    bool ToPixel(Vector3 p, out Vector2 pixel)
    {
        pixel = Vector2.zero;

        if (p.z < near || p.z > far)
            return false;

        Vector4 clip = P * new Vector4(p.x, p.y, p.z, 1f);

        Vector2 ndc = new Vector2(
            clip.x / clip.w,
            clip.y / clip.w
        );

        pixel.x = width * 0.5f + ndc.x * (width * 0.5f);
        pixel.y = height * 0.5f + ndc.y * (height * 0.5f);

        return true;
    }

    void DrawLine(int x0, int y0, int x1, int y1)
    {
        int dx = Mathf.Abs(x1 - x0);
        int sx = x0 < x1 ? 1 : -1;

        int dy = -Mathf.Abs(y1 - y0);
        int sy = y0 < y1 ? 1 : -1;

        int err = dx + dy;

        while (true)
        {
            if (x0 >= 0 && x0 < width &&
                y0 >= 0 && y0 < height)
            {
                canvas.SetPixel(x0, y0, lineColor);
            }

            if (x0 == x1 && y0 == y1)
                break;

            int e2 = 2 * err;

            if (e2 >= dy)
            {
                err += dy;
                x0 += sx;
            }

            if (e2 <= dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    void OnDrawGizmos()
    {
        float aspect = (float)width / height;

        Gizmos.matrix = Matrix4x4.TRS(
            transform.position,
            transform.rotation,
            Vector3.one
        );

        Gizmos.color = Color.yellow;

        Gizmos.DrawFrustum(
            Vector3.zero,
            fieldOfView,
            far,
            near,
            aspect
        );

        Gizmos.color = Color.blue;

        Gizmos.DrawLine(
            Vector3.zero,
            Vector3.forward * 2f
        );

        Gizmos.matrix = Matrix4x4.identity;
    }
}
