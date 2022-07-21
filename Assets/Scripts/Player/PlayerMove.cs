using UnityEngine;

public class PlayerMove : AbstractMove
{
    [SerializeField] private MovementJoystick MovementJoystick;

    protected override void FixedUpdate()
    {
        MoveDir = MovementJoystick.JoystickVec != Vector2.zero ? MovementJoystick.JoystickVec : GetKeyboardMoveVec();

        base.FixedUpdate();
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
