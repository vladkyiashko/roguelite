using UnityEngine;
using UnityEngine.Events;
using System;

public abstract class AbstractMove : MonoBehaviour
{
    [SerializeField] protected BaseUnitBalance Balance;
    [SerializeField] protected Rigidbody2D RB;
    [SerializeField] protected SpriteRenderer Sprite;
    [SerializeField] protected AbstractHealth Health;
    [SerializeField] protected MoveEvents Events;
    protected Vector3 MoveDir;
    private Vector3 LastMoveDir = Vector3.right;
    public Vector3 GetLastMoveDir => LastMoveDir;
    public FaceDir CurrentFaceDir { get; private set; }
    private bool IsMoving;

    protected virtual void FixedUpdate()
    {
        if (Health.GetCurrentHealth == 0)
        {
            return;
        }

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

        RB.MovePosition(transform.position + (MoveDir * Balance.Speed * Time.fixedDeltaTime));

        SetSpriteDir();

        LastMoveDir = MoveDir;
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
