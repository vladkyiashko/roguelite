using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BasePlayerAttack : MonoBehaviour
{
    private const int WallLayer = 11;
    [SerializeField] private PlayerAttackTriggerGameEvent OnTriggerEnter;
    [SerializeField] private Transform Transform;    
    [SerializeField] private float AutoDestroyDelay = 1f;
    [SerializeField] private FlipData FlipValue;
    [SerializeField] private Transform RotateToTranslateDir;
    [SerializeField] private TranslateType Translate;
    [SerializeField] private ThrowUpData ThrowUpValue;
    [SerializeField] private ThrowRandomData ThrowRandomValue;
    [SerializeField] private PathData OrbitData;
    [SerializeField] private Vector3[] SpawnOffsets;
    [SerializeField] private AreaData AreaStatData;
    [SerializeField] private PreDestroy PreDestroyData;
    private int HitCount;
    private List<Collider2D> IgnoreColliders = new();
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
    private WaitForSeconds _HitboxDelayWaitForSeconds;
    private WaitForSeconds HitboxDelayWaitForSeconds
    {
        get
        {
            _HitboxDelayWaitForSeconds ??= new(Balance.HitboxDelay);

            return _HitboxDelayWaitForSeconds;
        }
    }
    private WaitForSeconds _ProjectileIntervalWaitForSeconds;
    public WaitForSeconds ProjectileIntervalWaitForSeconds
    {
        get
        {
            if (Balance.ProjectileInterval == 0)
            {
                return null;
            }

            _ProjectileIntervalWaitForSeconds ??= new(Balance.ProjectileInterval);

            return _ProjectileIntervalWaitForSeconds;
        }
    }
    public Transform GetTransform => Transform;
    public ILocalObjectPoolGeneric Pool { get; set; }
    public int Id { get; set; }
    public BasePlayerAttackBalance Balance { get; set; }
    public WaitForSeconds StunWaitForSeconds { get; set; }

    private void OnDestroy()
    {
        IgnoreColliders.Clear();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (Balance.BlockedByWalls && other.gameObject.layer == WallLayer)
        {
            Destroy();
            return;
        }

        if (IgnoreColliders.Contains(other))
        {
            return;
        }

        IgnoreColliders.Add(other);
        StartCoroutine(RemoveIgnoreColliderDelayed(other));

        OnTriggerEnter.Raise(new PlayerAttackTrigger(this, other.gameObject));

        ProcessPierce();
    }

    private IEnumerator RemoveIgnoreColliderDelayed(Collider2D other)
    {
        yield return HitboxDelayWaitForSeconds;
        IgnoreColliders.Remove(other);
    }

    private void ProcessPierce()
    {
        if (Balance.Pierce <= 0)
        {
            return;
        }

        HitCount++;

        if (HitCount >= Balance.Pierce)
        {
            HitCount = 0;
            Destroy();
        }
    }

    private void Destroy()
    {
        TryStopAutoDestroyCoroutine();
        Pool.Destroy(Transform);
    }

    public virtual void Init(InitData init)
    {
        Transform.position = init.OwnerTransform.position + GetSpawnOffset(init.Iteration, init.FaceDir);

        if (FlipValue.FlipScale != null)
        {
            Vector3 localScale = Vector3.one;

            if (!FlipValue.AlterIterFlipXScale || init.Iteration % 2 == 0)
            {
                localScale = init.FaceDir == AbstractMove.FaceDir.Right ?
                    localScale : new Vector3(-localScale.x, localScale.y, localScale.z);
            }
            else
            {
                localScale = init.FaceDir == AbstractMove.FaceDir.Left ?
                    localScale : new Vector3(-localScale.x, localScale.y, localScale.z);
            }

            if (FlipValue.AlterIterFlipYScale && init.Iteration % 2 == 1)
            {
                localScale = new Vector3(localScale.x, -localScale.y, localScale.z);
            }

            FlipValue.FlipScale.localScale = localScale;
        }

        InitArea();

        if (Translate != TranslateType.none)
        {
            Vector3 dir = default;
            switch (Translate)
            {
                case TranslateType.moveDir:
                    dir = init.MoveDir;
                    break;
                case TranslateType.nearestTargetDir:
                    dir = (init.NearestTargetPosition - Transform.position).normalized;
                    break;
                case TranslateType.randomTargetDir:
                    dir = (init.RandomTargetPosition - Transform.position).normalized;
                    break;
                case TranslateType.throwUp:
                    ThrowUp(init.Iteration, Balance.Speed, init.FaceDir);
                    break;
                case TranslateType.firstNearestTargetPositionOtherThrowRandom:
                    FirstNearestTargetPositionOtherThrowRandom(init.Iteration, init.NearestTargetPosition, init.OwnerTransform.position, Balance.Speed);
                    break;
                case TranslateType.orbit:
                    Orbit(init.OwnerTransform, Balance.Speed, init.Iteration, init.Amount);
                    break;
            }

            if (dir != default)
            {
                StartCoroutine(TranslateCor(dir, Balance.Speed));
            }

            if (RotateToTranslateDir != null)
            {
                RotateToTranslateDir.rotation = Quaternion.LookRotation(Vector3.forward, Quaternion.Euler(0, 0, 90) * dir);
            }
        }

        AutoDestroyCoroutine = StartCoroutine(DestroyDelayed(
            AutoDestroyDelay > 0f ? AutoDestroyDelayWaitForSeconds : new WaitForSeconds(Balance.Duration)));

        if (PreDestroyData.PreDestroyTime > 0)
        {
            StartCoroutine(PreDestroyDelayed());
        }
    }
    
    private IEnumerator PreDestroyDelayed()
    {
        yield return new WaitForSeconds(AutoDestroyDelay > 0f ?
            AutoDestroyDelay - PreDestroyData.PreDestroyTime : Balance.Duration - PreDestroyData.PreDestroyTime);
        PreDestroyData.OnPreDestroy.Invoke();
    }

    protected virtual void InitArea()
    {
        if (AreaStatData.ScaleTransform == null || AreaStatData.ScaleType == AreaScaleType.none)
        {
            return;
        }

        AreaStatData.ScaleTransform.localScale = new Vector3(
            AreaStatData.ScaleType == AreaScaleType.x || AreaStatData.ScaleType == AreaScaleType.xy ? Balance.Area : AreaStatData.ScaleTransform.localScale.x,
            AreaStatData.ScaleType == AreaScaleType.y || AreaStatData.ScaleType == AreaScaleType.xy ? Balance.Area : AreaStatData.ScaleTransform.localScale.y,
            AreaStatData.ScaleTransform.localScale.z);
    }

    private Vector3 GetSpawnOffset(int iteration, AbstractMove.FaceDir faceDir)
    {
        if (SpawnOffsets.Length == 0)
        {
            return Vector3.zero;
        }

        Vector3 result;

        if (SpawnOffsets.Length == 1)
        {
            result = SpawnOffsets[0];
        }
        else
        {
            float x = UnityEngine.Random.Range(SpawnOffsets[0].x, SpawnOffsets[1].x);
            float y = UnityEngine.Random.Range(SpawnOffsets[0].y, SpawnOffsets[1].y);
            result = new Vector3(x, y);
        }

        if (FlipValue.FlipScale != null)
        {
            if ((!FlipValue.AlterIterFlipXScale && faceDir == AbstractMove.FaceDir.Left)
                || (FlipValue.AlterIterFlipXScale && faceDir == AbstractMove.FaceDir.Right && iteration % 2 == 1)
                || (FlipValue.AlterIterFlipXScale && faceDir == AbstractMove.FaceDir.Left && iteration % 2 == 0))
            {
                result = new Vector3(-result.x, result.y, result.z);
            }
        }

        return result;
    }

    private void ThrowUp(int iteration, float speed, AbstractMove.FaceDir faceDir)
    {
        int dir = faceDir == AbstractMove.FaceDir.Right ? 1 : -1;
        int index = iteration % 10;
        Transform.DOLocalJump(Transform.localPosition + new Vector3(ThrowUpValue.Widths[index] * dir, -12f, 0f),
            ThrowUpValue.Heights[index] * (1f + speed / 10f), 1, ThrowUpValue.Durations[index]);
    }

    private void FirstNearestTargetPositionOtherThrowRandom(int iteration, Vector3 nearestPosition, Vector3 ownerPosition, float speed)
    {
        if (iteration == 0 && (nearestPosition - Transform.position).sqrMagnitude <= 60f)
        {
            Transform.DOMove(nearestPosition, ThrowRandomValue.Duration * (1f + speed / 5f)).OnComplete(() => ThrowRandomValue.OnMoveComplete.Invoke());
        }
        else
        {
            float targetXOffset = UnityEngine.Random.Range(ThrowRandomValue.TargetOffsets[0].x, ThrowRandomValue.TargetOffsets[1].x);
            float targetYOffet = UnityEngine.Random.Range(ThrowRandomValue.TargetOffsets[0].y, ThrowRandomValue.TargetOffsets[1].y);
            Vector3 target = ownerPosition + new Vector3(targetXOffset, targetYOffet);
            Transform.DOLocalMove(target, ThrowRandomValue.Duration * (1f + speed / 5f)).OnComplete(() => ThrowRandomValue.OnMoveComplete.Invoke());
        }        
    }

    private void Orbit(Transform ownerTransform, float speed, int iteration, int amount)
    {
        Transform.SetParent(ownerTransform);

        Vector3[] waypoints = new Vector3[OrbitData.Waypoints.Length];
        SetWaypoints(waypoints, 0, OrbitData.Waypoints.Length, 0);
        Tween pathTween = Transform.DOLocalPath(waypoints,
            OrbitData.Duration * (1f + speed / 5f), PathType.CatmullRom).SetEase(Ease.Linear).SetLoops(-1);

        if (iteration > 0)
        {
            StartCoroutine(SetOrbitOffset(pathTween, speed, iteration, amount, OrbitData.Waypoints.Length / 2));
        }
        else
        {
            ActiveTweens.Add(pathTween);
        }        
    }

    private IEnumerator SetOrbitOffset(Tween pathTween, float speed, int iteration, int amount, int waypointsLength)
    {
        yield return null;

        Vector3 pathPos = pathTween.PathGetPoint((float)iteration / (float)amount / 2f);
        Transform.localPosition = pathPos;
        pathTween.Kill();

        yield return null;

        Vector3[] waypoints = new Vector3[OrbitData.Waypoints.Length];
        int skipCount = waypointsLength / amount * iteration;

        SetWaypoints(waypoints, skipCount, OrbitData.Waypoints.Length, 0);
        SetWaypoints(waypoints, 0, skipCount, OrbitData.Waypoints.Length - skipCount);

        Tween offsetPathTween = Transform.DOLocalPath(waypoints,
            OrbitData.Duration * (1f + speed / 5f), PathType.CatmullRom).SetEase(Ease.Linear).SetLoops(-1);
        ActiveTweens.Add(offsetPathTween);
    }

    private void SetWaypoints(Vector3[] waypoints, int start, int stop, int waypointsIndex)
    {
        for (int i = start; i < stop; i++)
        {
            waypoints[waypointsIndex] = OrbitData.Waypoints[i] * Balance.Area;
            waypointsIndex++;
        }
    }

    private IEnumerator TranslateCor(Vector3 dir, float speed)
    {
        while (true)
        {
            Transform.Translate(Time.deltaTime * speed * 7f * dir);
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
        public int Iteration;
        public int Amount;
        public InitData(AbstractMove.FaceDir faceDir, Vector3 moveDir,
            Vector3 nearestTargetPosition, Vector3 randomTargetPosition, Transform ownerTransform, int iteration, int amount)
        {
            FaceDir = faceDir;
            MoveDir = moveDir;
            NearestTargetPosition = nearestTargetPosition;
            RandomTargetPosition = randomTargetPosition;
            OwnerTransform = ownerTransform;
            Iteration = iteration;
            Amount = amount;
        }
    }

    [Serializable]
    public struct ThrowUpData
    {
        public float[] Widths;
        public float[] Heights;
        public float[] Durations;
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
    public struct AreaData
    {
        public Transform ScaleTransform;
        public AreaScaleType ScaleType;

    }
    [Serializable]
    public enum AreaScaleType
    {
        none,
        x,
        y,
        xy
    }

    [Serializable]
    public struct FlipData
    {
        public Transform FlipScale;
        public bool AlterIterFlipXScale;
        public bool AlterIterFlipYScale;
    }

    [Serializable]
    public struct PreDestroy
    {
        public float PreDestroyTime;
        public UnityEvent OnPreDestroy;
    }

    [Serializable]
    public enum TranslateType
    {
        none,
        moveDir,
        nearestTargetDir,
        randomTargetDir,
        throwUp,
        firstNearestTargetPositionOtherThrowRandom,
        orbit
    }
}
