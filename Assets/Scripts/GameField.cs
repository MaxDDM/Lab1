using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameField : MonoBehaviour
{
    [Header("Field Properties")]
    public float CellSize;
    public float Spacing;
    public int FieldSize;

    [SerializeField]
    private Cell cellPref;
    [SerializeField]
    private RectTransform rt;

    private List<List<Cell>> field;
    private void Start()
    {

        field = new List<List<Cell>>();
        for(int i = 0; i < FieldSize; i++)
        {
            field.Add(new List<Cell>(FieldSize));
            for(int j = 0; j < FieldSize; ++j)
            {
                field[i].Add(null);
            }
        }
        CreateCell();
        CreateCell();
    }

    private void CreateCell()
    {
        float fieldWidth = FieldSize * (CellSize + Spacing) + Spacing;
        rt.sizeDelta = new Vector2(fieldWidth, fieldWidth);

        float startX = -(fieldWidth / 2) + (CellSize / 2) + Spacing;
        float startY = -startX;

        var coords = GetEmptyPosition();

        var cell = Instantiate(cellPref, transform, false);
        CellView cellView = cell.GetComponent<CellView>();
        cellView.Init(ref cell);

        int value = Random.Range(0, 10) == 0 ? 2 : 1;
        float x = startX + (coords.Item1 * (CellSize + Spacing));
        float y = startY - (coords.Item2 * (CellSize + Spacing));

        cell.SetValue(coords.Item1, coords.Item2, value, x, y);
        field[coords.Item1][coords.Item2] = cell;
    }

    private (int, int) GetEmptyPosition()
    {
        var emptyCells = new List<(int, int)>();

        for (int x = 0; x < FieldSize; ++x)
        {
            for (int y = 0; y < FieldSize; ++y)
            {
                if (field[x][y] == null)
                {
                    emptyCells.Add((x, y));
                }
            }
        }
        return emptyCells[Random.Range(0, emptyCells.Count)];
    }
}
