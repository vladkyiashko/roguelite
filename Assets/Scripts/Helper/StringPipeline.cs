using TMPro;
using UnityEngine;

public class StringPipeline : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Text;
    private string FormatString;

    public void Format(int value)
    {
        Format((object)value);
    }
    public void Format(float value)
    {
        Format((object)value);
    }
    public void Format(long value)
    {
        Format((object)value);
    }
    public void Format(double value)
    {
        Format((object)value);
    }
    public void Format(object value)
    {
        if (FormatString == default)
        {
            FormatString = Text.text;
        }

        Text.text = string.Format(FormatString, value);
    }    
}
