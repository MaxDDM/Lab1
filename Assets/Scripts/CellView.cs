using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
    private Cell cell;

    [SerializeField]
    private Image image;
    [SerializeField]
    private TextMeshProUGUI points;

    public void Init(ref Cell cell)
    {
        this.cell = cell;
        cell.OnValueChanged += UpdateValue;
        cell.OnPositionChanged += UpdatePosition;
    }

    private void UpdateValue(int value)
    {
        points.text = cell.Value == 0 ? "" : cell.Points.ToString();
        image.color = cell.Value < 1 ? Color.gray : Color.red;
        points.color = cell.Value < 1 ? image.color : Color.white;
    }

    private void UpdatePosition(float x, float y)
    {
        var position = new Vector2(x, y);
        cell.transform.localPosition = position;
    }
}