using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private MovementJoystick MovementJoystick;
    [SerializeField] private float PlayerSpeed;
    [SerializeField] private Rigidbody2D RB;

    private void FixedUpdate()
    {
        if (MovementJoystick.JoystickVec.y != 0)
        {
            RB.velocity = new Vector2(MovementJoystick.JoystickVec.x * PlayerSpeed, MovementJoystick.JoystickVec.y * PlayerSpeed);
        }
        else
        {
            RB.velocity = Vector2.zero;
        }
    }
}