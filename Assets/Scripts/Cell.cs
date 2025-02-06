using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public static Color[] CellColors = new Color[8] { Color.gray, Color.red, Color.blue, Color.green, Color.black, Color.magenta, Color.cyan, Color.yellow };
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Value { get; private set; }
    public int Score => IsEmpty ? 0 : (int)Mathf.Pow(2, Value);
    public bool isMerged { get; set; }
    public bool IsEmpty => Value == 0;

    [SerializeField]
    private Image image;
    [SerializeField]
    private TextMeshProUGUI score;

    public void IncreaseValue()
    {
        ++Value;
        isMerged = true;
        GameController.Instance.AddScore(Score);
        UpdateCell();
    }

    public void SetData(int x, int y, int value)
    {
        X = x;
        Y = y;
        Value = value;
        UpdateCell();
    }

    public void MoveToOther(Cell other)
    {
        other.SetData(other.X, other.Y, Value);
        SetData(X, Y, 0);

        UpdateCell();
    }

    public void UpdateCell()
    {
        image.color = CellColors[Value % 8];
        score.text = IsEmpty ? string.Empty : Score.ToString();
        score.color = Value < 1 ? Color.black : Color.white;
    }

    public void Merge(Cell other)
    {
        other.IncreaseValue();
        SetData(X, Y, 0);
        UpdateCell();
    }
}
