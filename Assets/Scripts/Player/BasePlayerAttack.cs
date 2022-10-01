using System;
using System.Collections;
using UnityEngine;

public class BasePlayerAttack : MonoBehaviour
{
    private readonly Vector3 LeftScale = new(-1, 1, 1);
    [SerializeField] private PlayerAttackTriggerGameEvent OnTriggerEnter;
    [SerializeField] private Transform Transform;    
    [SerializeField] private float AutoDestroyDelay = 1f;
    [SerializeField] private bool DestroyOnHit;
    [SerializeField] private Transform FlipXScaleFaceDir;
    [SerializeField] private Transform RotateToTranslateDir;
    [SerializeField] private TranslateType Translate;
    [SerializeField] private float SpeedMult;
    [SerializeField] private Vector3 SpawnOffset;
    private Coroutine AutoDestroyCoroutine;
    private WaitForSeconds _AutoDestroyDelayWaitForSeconds;
    private WaitForSeconds AutoDestroyDelayWaitForSeconds
    {
        get
        {
            _AutoDestroyDelayWaitForSeconds ??= new(AutoDestroyDelay);

            return _AutoDestroyDelayWaitForSeconds;
        }
    }

    public Transform GetTransform => Transform;
    public ILocalObjectPoolGeneric Pool { get; set; }
    public int Id { get; set; }
    public BasePlayerAttackBalance Balance { get; set; }
    public WaitForSeconds StunWaitForSeconds { get; set; }

    private void OnEnable()
    {
        AutoDestroyCoroutine = StartCoroutine(DestroyDelayed(AutoDestroyDelayWaitForSeconds));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryStopAutoDestroyCoroutine();

        OnTriggerEnter.Raise(new PlayerAttackTrigger(this, other.gameObject));

        if (DestroyOnHit)
        {
            Pool.Destroy(Transform);
        }
    }

    public void Init(InitData init)
    {
        Transform.position = init.OwnerPosition + SpawnOffset;

        if (FlipXScaleFaceDir != null)
        {
            FlipXScaleFaceDir.localScale = init.FaceDir == AbstractMove.FaceDir.Right ? Vector3.one : LeftScale;
        }        

        if (Translate != TranslateType.none)
        {
            Vector3 dir = default;
            switch (Translate)
            {
                case TranslateType.moveDir:
                    dir = init.MoveDir;
                    break;
                case TranslateType.nearestTargetPosition:
                    dir = (init.NearestTargetPosition - Transform.position).normalized;
                    break;
                case TranslateType.randomTargetPosition:
                    dir = (init.RandomTargetPosition - Transform.position).normalized;
                    break;
            }
            StartCoroutine(TranslateCor(dir));

            if (RotateToTranslateDir != null)
            {
                RotateToTranslateDir.rotation = Quaternion.LookRotation(Vector3.forward, Quaternion.Euler(0, 0, 90) * dir);
            }
        }
    }
    
    private IEnumerator TranslateCor(Vector3 dir)
    {
        while (true)
        {
            Transform.Translate(Time.deltaTime * SpeedMult * dir);
            yield return null;
        }
    }

    private void TryStopAutoDestroyCoroutine()
    {
        if (AutoDestroyCoroutine == null)
        {
            return;
        }

        StopCoroutine(AutoDestroyCoroutine);
        AutoDestroyCoroutine = null;
    }

    private IEnumerator DestroyDelayed(WaitForSeconds delay)
    {
        yield return delay;
        Pool.Destroy(Transform);
        AutoDestroyCoroutine = null;
    }

    public struct InitData
    {
        public AbstractMove.FaceDir FaceDir;
        public Vector3 MoveDir;
        public Vector3 NearestTargetPosition;
        public Vector3 RandomTargetPosition;
        public Vector3 OwnerPosition;
        public InitData(AbstractMove.FaceDir faceDir, Vector3 moveDir,
            Vector3 nearestTargetPosition, Vector3 randomTargetPosition, Vector3 ownerPosition)
        {
            FaceDir = faceDir;
            MoveDir = moveDir;
            NearestTargetPosition = nearestTargetPosition;
            RandomTargetPosition = randomTargetPosition;
            OwnerPosition = ownerPosition;
        }
    }

    [Serializable]
    public enum TranslateType
    {
        none,
        moveDir,
        nearestTargetPosition,
        randomTargetPosition
    }
}
