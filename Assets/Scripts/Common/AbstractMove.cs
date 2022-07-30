using UnityEngine;

public abstract class AbstractMove : MonoBehaviour
{
    [SerializeField] protected float Speed;
    [SerializeField] protected Rigidbody2D RB;
    [SerializeField] protected SpriteRenderer Sprite;
    protected Vector3 MoveDir;
    public FaceDir CurrentFaceDir { get; private set; }

    protected virtual void FixedUpdate()
    {
        if (MoveDir == Vector3.zero)
        {
            return;
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
}
