using System.Collections;
using UnityEngine;

public class DropPickup : MonoBehaviour
{
    [SerializeField] private Transform TargetTransform;
    [SerializeField] private IntGameEvent OnExpGemPickup;

    public void OnTrigger(Loot loot)
    {
        _ = StartCoroutine(FlyAnimation(loot));
    }

    private IEnumerator FlyAnimation(Loot loot)
    {
        Vector3 awayDir = (TargetTransform.position - loot.GetTransform.position).normalized;
        Vector3 awayPosition = loot.GetTransform.position - awayDir;
        yield return StartCoroutine(MoveTowards(0.16f, loot.GetTransform, awayPosition));
        yield return StartCoroutine(MoveTowards(0.33f, loot.GetTransform, Vector3.zero, TargetTransform));

        OnFlyComplete(loot);
    }

    private IEnumerator MoveTowards(float duration, Transform objectToMove,
            Vector3 toPosition, Transform toTransform = null)
    {
        float counter = 0;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            yield return MoveTowardsIteration(objectToMove,
                    toTransform != null ? toTransform.position : toPosition, duration, counter);
        }
    }

    private IEnumerator MoveTowardsIteration(Transform objectToMove, Vector3 toPosition, float duration, float counter)
    {
        Vector3 currentPos = objectToMove.position;

        float time = Vector3.Distance(currentPos, toPosition) / (duration - counter) * Time.deltaTime;

        objectToMove.position = Vector3.MoveTowards(currentPos, toPosition, time);

        yield return null;
    }

    private void OnFlyComplete(Loot loot)
    {
        loot.Pickup();
        OnExpGemPickup.Raise(loot.Value);
    }
}
