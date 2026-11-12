
using UnityEngine;

public class ClockHandRotation : MonoBehaviour
{
    public float degreesPerSecond = 6f;

    private Quaternion initialRotation;
    private float elapsedTime = 0f;

    void Start()
    {
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        float angle = -degreesPerSecond * elapsedTime * Mathf.Deg2Rad;

        float c = Mathf.Cos(angle);
        float s = Mathf.Sin(angle);

        // Z축 회전행렬 직접 구성
        Matrix4x4 rotation = Matrix4x4.identity;
        rotation.m00 = c;
        rotation.m01 = -s;
        rotation.m10 = s;
        rotation.m11 = c;

        transform.localRotation = initialRotation * rotation.rotation;
    }
}
