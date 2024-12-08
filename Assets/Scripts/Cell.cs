using Unity.VisualScripting;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public event System.Action<float, float> OnPositionChanged;
    public event System.Action<int> OnValueChanged;
    public int X { get; private set; }
    public int Y { get; private set; }

    public int Value { get; private set; }

    public int Points => Value == 0 ? 0 : (int)Mathf.Pow(2, Value);

    public void SetValue(int x, int y, int value, float X, float Y)
    {
        this.X = x;
        this.Y = y;
        Value = value;
        OnPositionChanged?.Invoke(X, Y);
        OnValueChanged?.Invoke(value);
    }
}
