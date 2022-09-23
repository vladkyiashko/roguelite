using System;
using DataStructures.RandomSelector;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Balance/LevelUpBalance")]
public class LevelUpBalance : ScriptableObject
{
    public int UpgradesCount = 3;
    [SerializeField] private UpgradeTypeWeight[] UpgradeTypeWeights;

    private DynamicRandomSelector<LevelUpUpgradeType> TypeSelector;
    public DynamicRandomSelector<LevelUpUpgradeType> GetTypeSelector
    {
        get
        {
            if (TypeSelector == null)
            {
                InitTypeSelector();
            }

            return TypeSelector;
        }
    }

    private void InitTypeSelector()
    {
        TypeSelector = new DynamicRandomSelector<LevelUpUpgradeType>();
        for (int i = 0; i < UpgradeTypeWeights.Length; i++)
        {
            TypeSelector.Add(UpgradeTypeWeights[i].UpUpgradeType, UpgradeTypeWeights[i].Weight);
        }
        TypeSelector.Build();
    }

    [Serializable]
    public struct UpgradeTypeWeight
    {
        public LevelUpUpgradeType UpUpgradeType;
        public int Weight;
    }
}

public enum LevelUpUpgradeType
{
    attack,
    defense
}
