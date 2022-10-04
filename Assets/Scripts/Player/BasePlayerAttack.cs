using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BasePlayerAttack : MonoBehaviour
{
    private readonly Vector3 LeftScale = new(-1, 1, 1);
    [SerializeField] private PlayerAttackTriggerGameEvent OnTriggerEnter;
    [SerializeField] private Transform Transform;    
    [SerializeField] private float AutoDestroyDelay = 1f;
    [SerializeField] private int DestroyOnHitCount;
    [SerializeField] private Transform FlipXScaleFaceDir;
    [SerializeField] private Transform RotateToTranslateDir;
    [SerializeField] private TranslateType Translate;
    [SerializeField] private OffsetData[] ThrowUpOffsets;
    [SerializeField] private ThrowRandomData ThrowRandomValue;
    [SerializeField] private PathData OrbitData;
    [SerializeField] private float SpeedMult;
    [SerializeField] private Vector3[] SpawnOffsets;
    private int HitCount;
    private List<Tween> ActiveTweens = new();
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
        OnTriggerEnter.Raise(new PlayerAttackTrigger(this, other.gameObject));

        ProcessDestroyOnHitCount();
    }

    private void ProcessDestroyOnHitCount()
    {
        if (DestroyOnHitCount <= 0)
        {
            return;
        }

        HitCount++;

        if (HitCount >= DestroyOnHitCount)
        {
            TryStopAutoDestroyCoroutine();
            Pool.Destroy(Transform);
        }
    }

    public void Init(InitData init)
    {
        Transform.position = init.OwnerTransform.position + GetSpawnOffset();

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
                case TranslateType.throwUp:
                    ThrowUp();
                    break;
                case TranslateType.throwRandom:
                    ThrowRandom(init.OwnerTransform.position);
                    break;
                case TranslateType.orbit:
                    Orbit(init.OwnerTransform);
                    break;
            }

            if (dir != default)
            {
                StartCoroutine(TranslateCor(dir));
            }

            if (RotateToTranslateDir != null)
            {
                RotateToTranslateDir.rotation = Quaternion.LookRotation(Vector3.forward, Quaternion.Euler(0, 0, 90) * dir);
            }
        }
    }
    
    private Vector3 GetSpawnOffset()
    {
        if (SpawnOffsets.Length == 0)
        {
            return Vector3.zero;
        }

        if (SpawnOffsets.Length == 1)
        {
            return SpawnOffsets[0];
        }

        float x = UnityEngine.Random.Range(SpawnOffsets[0].x, SpawnOffsets[1].x);
        float y = UnityEngine.Random.Range(SpawnOffsets[0].y, SpawnOffsets[1].y);
        return new Vector3(x, y);
    }

    private void ThrowUp()
    {
        float startLocalY = Transform.localPosition.y;
        Transform.DOLocalMoveY(startLocalY + ThrowUpOffsets[0].Offset, ThrowUpOffsets[0].Duration).SetEase(Ease.OutSine).OnComplete(
            () => Transform.DOLocalMoveY(startLocalY + ThrowUpOffsets[1].Offset, ThrowUpOffsets[1].Duration).SetEase(Ease.InSine)
        );
    }

    private void ThrowRandom(Vector3 ownerPosition)
    {
        float targetXOffset = UnityEngine.Random.Range(ThrowRandomValue.TargetOffsets[0].x, ThrowRandomValue.TargetOffsets[1].x);
        float targetYOffet = UnityEngine.Random.Range(ThrowRandomValue.TargetOffsets[0].y, ThrowRandomValue.TargetOffsets[1].y);
        Vector3 target = ownerPosition + new Vector3(targetXOffset, targetYOffet);
        Transform.DOLocalMove(target, ThrowRandomValue.Duration).OnComplete(() => ThrowRandomValue.OnMoveComplete.Invoke());
    }

    private void Orbit(Transform ownerTransform)
    {
        Transform.SetParent(ownerTransform);

        Vector3[] waypoints = new Vector3[OrbitData.Waypoints.Length];
        for (int i = 0; i < OrbitData.Waypoints.Length; i++)
        {
            waypoints[i] = OrbitData.Waypoints[i];
        }
        Tween pathTween = Transform.DOLocalPath(waypoints, OrbitData.Duration, PathType.CatmullRom).SetEase(Ease.Linear).SetLoops(-1);
        ActiveTweens.Add(pathTween);
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
        for (int i = 0; i < ActiveTweens.Count; i++)
        {
            ActiveTweens[i].Kill();
        }
        ActiveTweens.Clear();
        Pool.Destroy(Transform);
        AutoDestroyCoroutine = null;
    }

    public struct InitData
    {
        public AbstractMove.FaceDir FaceDir;
        public Vector3 MoveDir;
        public Vector3 NearestTargetPosition;
        public Vector3 RandomTargetPosition;
        public Transform OwnerTransform;
        public InitData(AbstractMove.FaceDir faceDir, Vector3 moveDir,
            Vector3 nearestTargetPosition, Vector3 randomTargetPosition, Transform ownerTransform)
        {
            FaceDir = faceDir;
            MoveDir = moveDir;
            NearestTargetPosition = nearestTargetPosition;
            RandomTargetPosition = randomTargetPosition;
            OwnerTransform = ownerTransform;
        }
    }

    [Serializable]
    public struct OffsetData
    {
        public float Offset;
        public float Duration;
    }

    [Serializable]
    public struct ThrowRandomData
    {
        public Vector3[] TargetOffsets;
        public float Duration;
        public UnityEvent OnMoveComplete;
    }

    [Serializable]
    public struct PathData
    {
        public Vector3[] Waypoints;
        public float Duration;
    }

    [Serializable]
    public enum TranslateType
    {
        none,
        moveDir,
        nearestTargetPosition,
        randomTargetPosition,
        throwUp,
        throwRandom,
        orbit
    }
}
