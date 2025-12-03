using UnityEngine;

/// <summary>
/// 플레이어 자식 카메라에서 시점 회전을 처리하는 스크립트입니다.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Tooltip("플레이어 루트 트랜스폼")]
    public Transform playerBody;
    [Tooltip("마우스 감도")]
    public float sensitivity = 100f;

    private float xRotation = 0f;
    private PlayerInput input;

    private void Awake()
    {
        // 부모 객체에 연결된 PlayerInput을 가져옵니다.
        input = GetComponentInParent<PlayerInput>();
    }

    private void Update()
    {
        // 마우스 입력 값을 받아옵니다.
        Vector2 look = input.LookInput;
        float mouseX = look.x * sensitivity * Time.deltaTime;
        float mouseY = look.y * sensitivity * Time.deltaTime;

        // 카메라의 상하 회전을 처리합니다.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 플레이어 본체의 좌우 회전을 처리합니다.
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
