using UnityEngine;
using UnityEngine.Events;
using System;

public abstract class AbstractMove : MonoBehaviour
{
    [SerializeField] protected float Speed;
    [SerializeField] protected Rigidbody2D RB;
    [SerializeField] protected SpriteRenderer Sprite;
    [SerializeField] protected MoveEvents Events;
    protected Vector3 MoveDir;
    public FaceDir CurrentFaceDir { get; private set; }
    public bool IsMoving { get; set; }

    protected virtual void FixedUpdate()
    {
        if (MoveDir == Vector3.zero)
        {
            if (IsMoving)
            {
                Events.OnMoveStop.Invoke();
                IsMoving = false;
            }

            return;
        }

        if (!IsMoving)
        {
            Events.OnMoveStart.Invoke();
            IsMoving = true;
        }

        RB.MovePosition(transform.position + (MoveDir * Speed * Time.fixedDeltaTime));

        SetSpriteDir();
    }

    protected virtual void SetSpriteDir()
    {
        if (MoveDir.x > 0)
        {
            CurrentFaceDir = FaceDir.Right;
            Sprite.flipX = false;
        }
        else if (MoveDir.x < 0)
        {
            CurrentFaceDir = FaceDir.Left;
            Sprite.flipX = true;
        }
    }

    public enum FaceDir
    {
        Right,
        Left,
    }

    [Serializable]
    public struct MoveEvents
    {
        public UnityEvent OnMoveStart;
        public UnityEvent OnMoveStop;
    }
}
