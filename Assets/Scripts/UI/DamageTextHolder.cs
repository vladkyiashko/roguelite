using TMPro;
using UnityEngine;

public class DamageTextHolder : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Text;
    [SerializeField] private Transform Transform;
    public Transform GetTransform => Transform;
    public TextMeshProUGUI GetText => Text;
}
