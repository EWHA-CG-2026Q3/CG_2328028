
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

// 빈 오브젝트(VirtualCamera)에 붙여,
// 이 오브젝트를 카메라로 삼아 장면을 캔버스에 와이어프레임으로 그림
public class S11_VirtualCamera_Finish : MonoBehaviour
{
    [Header("그릴 오브젝트")]
    public MeshFilter[] targets;

    [Header("캔버스에 담을 범위")]
    public float size = 2f;
    public float near = 0.3f;
    public float far = 20f;

    [Header("캔버스")]
    public RawImage canvasImage;
    public int width = 400;
    public int height = 300;
    public Color backgroundColor = Color.black;
    public Color lineColor = Color.white;

    Texture2D canvas;
    Color[] clearPixels;

    void OnEnable()
    {
        CreateCanvas();

        RenderPipelineManager.beginContextRendering += OnBeginRendering;
    }

    void OnDisable()
    {
        RenderPipelineManager.beginContextRendering -= OnBeginRendering;
    }

    void OnBeginRendering(
        ScriptableRenderContext context,
        System.Collections.Generic.List<Camera> cameras)
    {
        if (canvas == null ||
            canvas.width != width ||
            canvas.height != height)
        {
            CreateCanvas();
        }

        DrawSceneOnCanvas();
    }

    void CreateCanvas()
    {
        canvas = new Texture2D(
            width,
            height,
            TextureFormat.RGBA32,
            false
        );

        canvas.filterMode = FilterMode.Point;

        clearPixels = new Color[width * height];

        for (int i = 0; i < clearPixels.Length; i++)
        {
            clearPixels[i] = backgroundColor;
        }

        if (canvasImage != null)
        {
            canvasImage.texture = canvas;

            canvasImage.rectTransform.sizeDelta =
                new Vector2(width, height);
        }
    }

    // 뷰 행렬 V = R^-1 * T^-1
    Matrix4x4 BuildViewMatrix(Transform cam)
    {
        Matrix4x4 Tinv =
            Matrix4x4.Translate(-cam.position);

        Matrix4x4 Rinv =
            Matrix4x4.Rotate(
                Quaternion.Inverse(cam.rotation)
            );

        // S11 과제: 직접 구현한 행렬 곱셈 함수 사용
        return MultiplyMatrixMatrix(Rinv, Tinv);
    }

    // ==============================================
    // S11 과제 추가 부분
    // 열 단위 행렬 × 행렬 계산
    // ==============================================
    Matrix4x4 MultiplyMatrixMatrix(Matrix4x4 A, Matrix4x4 B)
    {
        Matrix4x4 result = Matrix4x4.zero;

        for (int i = 0; i < 4; i++)
        {
            Vector4 column = B.GetColumn(i);

            Vector4 newColumn = A * column;

            result.SetColumn(i, newColumn);
        }

        return result;
    }

    // 카메라가 본 장면을 캔버스에 그림
    void DrawSceneOnCanvas()
    {
        ClearCanvas();

        Matrix4x4 V = BuildViewMatrix(transform);

        foreach (MeshFilter mf in targets)
        {
            if (mf == null || mf.sharedMesh == null)
                continue;

            Matrix4x4 VM =
                V * mf.transform.localToWorldMatrix;

            Vector3[] verts = mf.sharedMesh.vertices;
            int[] tris = mf.sharedMesh.triangles;

            for (int i = 0; i < tris.Length; i += 3)
            {
                Vector3 a =
                    VM.MultiplyPoint(verts[tris[i]]);

                Vector3 b =
                    VM.MultiplyPoint(verts[tris[i + 1]]);

                Vector3 c =
                    VM.MultiplyPoint(verts[tris[i + 2]]);

                DrawEdge(a, b);
                DrawEdge(b, c);
                DrawEdge(c, a);
            }
        }

        canvas.Apply();
    }

    // 카메라 좌표를 캔버스 픽셀 위치로 변환
    bool ToPixel(Vector3 p, out Vector2 pixel)
    {
        pixel = Vector2.zero;

        if (p.z < near || p.z > far)
            return false;

        float aspect = (float)width / height;

        pixel.x =
            width * 0.5f
            + p.x / (size * aspect) * (width * 0.5f);

        pixel.y =
            height * 0.5f
            + p.y / size * (height * 0.5f);

        return true;
    }

    // 두 점을 잇는 선 그리기
    void DrawEdge(Vector3 a, Vector3 b)
    {
        if (!ToPixel(a, out Vector2 pa))
            return;

        if (!ToPixel(b, out Vector2 pb))
            return;

        DrawLine(
            Mathf.RoundToInt(pa.x),
            Mathf.RoundToInt(pa.y),
            Mathf.RoundToInt(pb.x),
            Mathf.RoundToInt(pb.y)
        );
    }

    // 브레젠험 선 그리기 알고리즘
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

    // 캔버스 초기화
    void ClearCanvas()
    {
        canvas.SetPixels(clearPixels);
    }

    // Scene 뷰에 VirtualCamera 범위 표시
    void OnDrawGizmos()
    {
        Vector3 o = transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(o, o + transform.right);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(o, o + transform.up);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(o, o + transform.forward);

        float aspect = (float)width / height;

        float hx = size * aspect;
        float hy = size;

        Vector3 center = new Vector3(
            0,
            0,
            (near + far) * 0.5f
        );

        Vector3 extent = new Vector3(
            hx * 2,
            hy * 2,
            far - near
        );

        Gizmos.color = Color.yellow;

        Gizmos.matrix = Matrix4x4.TRS(
            transform.position,
            transform.rotation,
            Vector3.one
        );

        Gizmos.DrawWireCube(center, extent);

        Gizmos.matrix = Matrix4x4.identity;
    }
}
