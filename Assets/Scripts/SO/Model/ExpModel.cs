using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Model/ExpModel")]
public class ExpModel : ScriptableObject
{
    [NonSerialized] private int _CurrentLevel;
    public int CurrentLevel
    {
        get => _CurrentLevel;
        set
        {
            _CurrentLevel = value;
            LevelChanged.Raise(_CurrentLevel);
        }
    }

    [NonSerialized] private int _CurrentExp;
    public int CurrentExp
    {
        get => _CurrentExp;
        set
        {
            _CurrentExp = value;
            ExpChanged.Raise(_CurrentExp);
        }
    }

    [NonSerialized] private float _CurrentExpPercent;
    public float CurrentExpPercent
    {
        get => _CurrentExpPercent;
        set
        {
            _CurrentExpPercent = value;
            ExpPercentChanged.Raise(_CurrentExpPercent);
        }
    }

    [SerializeField] private IntGameEvent ExpChanged;
    [SerializeField] private FloatGameEvent ExpPercentChanged;
    [SerializeField] private IntGameEvent LevelChanged;
}
