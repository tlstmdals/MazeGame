using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float gravity = -9.81f;

    private CharacterController cc;
    private Vector3 velocity;
    private PlayerInput input;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        Vector3 move = transform.right * input.MoveInput.x + transform.forward * input.MoveInput.y;
        cc.Move(move * moveSpeed * Time.deltaTime);

        if (cc.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f; // 플레이어를 지면에 붙여 주기 위한 작은 음수 값
        }

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
}
