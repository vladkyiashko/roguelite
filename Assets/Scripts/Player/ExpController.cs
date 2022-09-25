using System.Collections.Generic;
using UnityEngine;

public class ExpController : MonoBehaviour
{
    [SerializeField] private LevelUpBalance LevelUpBalance;
    [SerializeField] private PlayerAttacksBalance PlayerAttacksBalance;
    [SerializeField] private ExpBalance ExpBalance;
    [SerializeField] private ExpModel ExpModel;
    [SerializeField] private PlayerAttackController PlayerAttackController;
    [SerializeField] private StringGameEvent[] OnLevelUpItemNames;
    [SerializeField] private SpriteGameEvent[] OnLevelUpItemIcons;
    [SerializeField] private StringGameEvent[] OnLevelUpItemDescrs;
    [SerializeField] private BoolGameEvent[] OnLevelUpItemIsNews;
    [SerializeField] private IntGameEvent[] OnLevelUpItemLevels;
    [SerializeField] private IntGameEvent OnGetAttackItemId;
    [SerializeField] private IntGameEvent OnGetDefenseItemId;
    private readonly Dictionary<int, ItemInfo> ItemInfoByListIndex = new();

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
        SetCurrentExpPercent();

        if (ExpModel.CurrentExp >= ExpBalance.ExpPerLevelIndexes[ExpModel.CurrentLevel - 1])
        {
            ExpModel.CurrentExp -= ExpBalance.ExpPerLevelIndexes[ExpModel.CurrentLevel - 1];
            ExpModel.CurrentLevel++;
        }
    }

    private void SetCurrentExpPercent()
    {
        ExpModel.CurrentExpPercent = (float)ExpModel.CurrentExp /
            ExpBalance.ExpPerLevelIndexes[ExpModel.CurrentLevel - 1];
    }

    public void OnLevelUp(int level)
    {
        if (level == 1)
        {
            return;
        }

        ItemInfoByListIndex.Clear();

        int upgradesCount = LevelUpBalance.UpgradesCount;

        int[] ids = new int[upgradesCount];

        for (int i = 0; i < upgradesCount; i++)
        {
            LevelUpUpgradeType type = LevelUpBalance.GetTypeSelector.SelectRandomItem();

            if (type == LevelUpUpgradeType.attack)
            {
                int id = PlayerAttacksBalance.GetSelector.SelectRandomItem();
                ids[i] = id;
                bool isNew = !PlayerAttackController.GetActiveAttackIds.Contains(id);
                int newItemLevel = isNew ? 1 : PlayerAttackController.GetItemLevelsByIds[id] + 1;
                float newValue = PlayerAttackController.GetItemValueByLevel(id, newItemLevel);
                ItemInfoByListIndex.Add(i, new ItemInfo(type, id));
                BroadcastLevelUpItem(PlayerAttacksBalance.Attacks[id], isNew, newItemLevel, newValue, i);
                PlayerAttacksBalance.SelectorRemoveId(new int[] { id });
            }
        }

        PlayerAttacksBalance.SelectorAddIds(ids);
    }    

    private void BroadcastLevelUpItem(PlayerAttacksBalance.PlayerItemsBalanceItem item, bool isNew, int newItemLevel, float newValue, int listIndex)
    {
        string descr = isNew ? item.Descr : string.Format(item.UpgDescrFormat, newValue);

        OnLevelUpItemNames[listIndex].Raise(item.Name);
        OnLevelUpItemIcons[listIndex].Raise(item.Sprite);
        OnLevelUpItemDescrs[listIndex].Raise(descr);
        OnLevelUpItemIsNews[listIndex].Raise(isNew);
        OnLevelUpItemLevels[listIndex].Raise(newItemLevel);
    }

    public void OnItemClick(int listIndex)
    {
        switch (ItemInfoByListIndex[listIndex].Type)
        {
            case LevelUpUpgradeType.attack:
                OnGetAttackItemId.Raise(ItemInfoByListIndex[listIndex].Id);
                break;
            case LevelUpUpgradeType.defense:
                OnGetDefenseItemId.Raise(ItemInfoByListIndex[listIndex].Id);
                break;
        }

        SetCurrentExpPercent();
    }

    private struct ItemInfo
    {
        public LevelUpUpgradeType Type;
        public int Id;

        public ItemInfo(LevelUpUpgradeType type, int id)
        {
            Type = type;
            Id = id;
        }
    }
}
