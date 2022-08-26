using UnityEngine;

public class ExpController : MonoBehaviour
{
    [SerializeField] private ExpBalance ExpBalance;
    [SerializeField] private ExpModel ExpModel;

    private void Start()
    {
        InitLevel();
    }

    private void InitLevel()
    {
        ExpModel.CurrentLevel = 1;
    }

    public void OnExpGemPickup(int value)
    {
        ExpModel.CurrentExp += value;
        ExpModel.CurrentExpPercent = (float)ExpModel.CurrentExp /
            ExpBalance.ExpPerLevelIndexes[ExpModel.CurrentLevel - 1];
    }
}
