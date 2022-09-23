using UnityEngine;

public class ExpController : MonoBehaviour
{
    [SerializeField] private LevelUpBalance LevelUpBalance;
    [SerializeField] private PlayerAttacksBalance PlayerAttacksBalance;
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

    public void OnLevelUp(int level)
    {
        if (level == 1)
        {
            return;
        }

        for (int i = 0; i < LevelUpBalance.UpgradesCount; i++)
        {
            LevelUpUpgradeType type = LevelUpBalance.GetTypeSelector.SelectRandomItem();

            if (type == LevelUpUpgradeType.attack)
            {
                int id = PlayerAttacksBalance.GetSelector.SelectRandomItem();
            }
        }
    }
}
