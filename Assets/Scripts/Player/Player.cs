using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private MovementJoystick MovementJoystick;
    [SerializeField] private float PlayerSpeed;
    [SerializeField] private Rigidbody2D RB;

    private void FixedUpdate()
    {
        Vector2 moveDir = MovementJoystick.JoystickVec != Vector2.zero ? MovementJoystick.JoystickVec : GetKeyboardMoveVec();

        if (moveDir != Vector2.zero)
        {
            RB.velocity = new Vector2(moveDir.x * PlayerSpeed, moveDir.y * PlayerSpeed);
        }
        else
        {
            RB.velocity = Vector2.zero;
        }
    }

    private Vector2 GetKeyboardMoveVec()
    {
        Vector2 keyboardMoveVec = Vector2.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            keyboardMoveVec.y = +1f;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            keyboardMoveVec.y = -1f;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            keyboardMoveVec.x = -1f;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            keyboardMoveVec.x = +1f;
        }

        return keyboardMoveVec;
    }
}