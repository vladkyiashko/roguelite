using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private MovementJoystick MovementJoystick;
    [SerializeField] private float PlayerSpeed;
    private void Update()
    {
        Vector3 moveDir = Vector3.zero;
        Vector2 keyboardMoveVec = GetKeyboardMoveVec();

        if (keyboardMoveVec != Vector2.zero)
        {
            moveDir = new Vector3(keyboardMoveVec.x, keyboardMoveVec.y).normalized;
        }
        else if (MovementJoystick.JoystickVec != Vector2.zero)
        {
            moveDir = new Vector3(MovementJoystick.JoystickVec.x, MovementJoystick.JoystickVec.y).normalized;
        }

        if (moveDir != Vector3.zero)
        {
            transform.position += moveDir * PlayerSpeed * Time.deltaTime;
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

    /*private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.LogError(gameObject + " OnTriggerEnter2D("+other.gameObject+")");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.LogError(gameObject + " OnTriggerExit2D("+other.gameObject+")");
    }*/
}