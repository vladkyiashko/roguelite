using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider Healthbar;
    [SerializeField] private PlayerHealth PlayerHealth;

    private void Awake()
    {
        PlayerHealth.OnHealthPercentChanged += OnHealthPercentChanged; 
    }

    private void OnDestroy()
    {
        PlayerHealth.OnHealthPercentChanged -= OnHealthPercentChanged; 
    }

    private void OnHealthPercentChanged(float health)
    {
        Healthbar.value = health;
    }
}
