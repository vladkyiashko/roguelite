using UnityEngine;
using UnityEngine.EventSystems;

public class MovementJoystick : MonoBehaviour
{
    [SerializeField] private Transform Joystick;
    [SerializeField] private Transform JoystickBG;
    public Vector2 JoystickVec;
    private Vector2 JoystickTouchPos;
    private Vector2 JoystickOriginalPos;
    private float JoystickRadius;

    private void Start()
    {
        JoystickOriginalPos = JoystickBG.position;
        JoystickRadius = JoystickBG.GetComponent<RectTransform>().sizeDelta.y / 4;
    }

    public void PointerDown()
    {
        Joystick.position = Input.mousePosition;
        JoystickBG.position = Input.mousePosition;
        JoystickTouchPos = Input.mousePosition;
    }

    public void Drag(BaseEventData baseEventData)
    {
        PointerEventData pointerEventData = baseEventData as PointerEventData;
        Vector2 dragPos = pointerEventData.position;
        JoystickVec = (dragPos - JoystickTouchPos).normalized;

        float joystickDist = Vector2.Distance(dragPos, JoystickTouchPos);

        if (joystickDist < JoystickRadius)
        {
            Joystick.position = JoystickTouchPos + JoystickVec * joystickDist;
        }
        else
        {
            Joystick.position = JoystickTouchPos + JoystickVec * JoystickRadius;
        }
    }

    public void PointerUp()
    {
        JoystickVec = Vector2.zero;
        Joystick.position = JoystickOriginalPos;
        JoystickBG.position = JoystickOriginalPos;        
    }
}
