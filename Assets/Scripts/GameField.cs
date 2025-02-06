using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameField : MonoBehaviour
{
    public static GameField Instance;
    [Header("Field Properties")]
    public float CellSize;
    public float Spacing;
    public int FieldSize;
    [Space(10)]
    [SerializeField]
    private Cell cellPref;
    [SerializeField]
    private RectTransform rt;
    private Cell[,] field;
    private bool anyCellMoved;

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnInput(Vector2.left);
            Debug.Log("Ход влево");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            OnInput(Vector2.right);
            Debug.Log("Ход вправо");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnInput(Vector2.up);
            Debug.Log("Ход вверх");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            OnInput(Vector2.down);
            Debug.Log("Ход вниз");
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnInput(Vector2.left);
            Debug.Log("Ход влево");
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnInput(Vector2.right);
            Debug.Log("Ход вправо");
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnInput(Vector2.up);
            Debug.Log("Ход вверх");
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnInput(Vector2.down);
            Debug.Log("Ход вниз");
        }
#endif
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (File.Exists("output.data"))
        {
            using (FileStream file = File.Open("output.data", FileMode.Open))
            {
                object data = new BinaryFormatter().Deserialize(file);
                (int, int, int, bool, int)[,] cells = ((int, int, int, bool, int)[,])data;
                field = new Cell[FieldSize, FieldSize];
                float fieldWidth = FieldSize * (CellSize + Spacing) + Spacing;
                rt.sizeDelta = new Vector2(fieldWidth, fieldWidth);
                float X_0 = -(fieldWidth / 2) + (CellSize / 2) + Spacing;
                float Y_0 = -X_0;
                for (int x = 0; x < FieldSize; ++x)
                {
                    for (int y = 0; y < FieldSize; ++y)
                    {
                        var cell = Instantiate(cellPref, transform, false);
                        var position = new Vector2(X_0 + (x * (CellSize + Spacing)), Y_0 - (y * (CellSize + Spacing)));
                        cell.transform.localPosition = position;
                        cell.SetData(cells[x, y].Item1, cells[x, y].Item2, cells[x, y].Item3);
                        cell.isMerged = cells[x, y].Item4;
                        field[x, y] = cell;
                        if(x == 0 && y == 0)
                        {
                            GameController.Instance.SetScore(cells[x, y].Item5); 
                        }
                    }
                }
                CheckResult();
            }
        }
        SwipeController.SwipeEvent += OnSwipe;
    }

    private void OnDestroy()
    {
        (int, int, int, bool, int)[,] cells = new (int, int, int, bool, int)[FieldSize, FieldSize];
        for (int x = 0; x < FieldSize; ++x)
        {
            for(int y = 0; y < FieldSize; ++y)
            {
                cells[x, y] = (field[x, y].X, field[x, y].Y, field[x, y].Value, field[x, y].isMerged, GameController.Score);
            }
        }
        using FileStream file = File.OpenWrite("output.data");
        new BinaryFormatter().Serialize(file, cells);
    }

    private void OnSwipe(Vector2 direction)
    {
        if (direction == Vector2.up)
        {
            Debug.Log("Ход вверх");
        }
        if (direction == Vector2.down)
        {
            Debug.Log("Ход вниз");
        }
        if (direction == Vector2.left)
        {
            Debug.Log("Ход влево");
        }
        if (direction == Vector2.right)
        {
            Debug.Log("Ход вправо");
        }
        OnInput(direction);
    }

    private void OnInput(Vector2 direction)
    {
        if (!GameController.GameStarted)
        {
            return;
        }
        anyCellMoved = false;
        changeFlags();
        Move(direction);
        if(anyCellMoved)
        {
            CreateRandom();
            CheckResult();
        }
    }

    private void Move(Vector2 direction)
    {
        int XY_0 = direction.x > 0 || direction.y < 0 ? FieldSize - 1 : 0;
        int d = direction.x != 0 ? (int)direction.x : -(int)direction.y;
        for (int i = 0; i < FieldSize; ++i)
        {
            for (int k = XY_0; k >= 0 && k < FieldSize; k -= d)
            {
                var cell = direction.x != 0 ? field[k, i] : field[i, k];
                if (cell.IsEmpty)
                {
                    continue;
                }
                var cellToMerge = FindCellToMerge(cell, direction);
                if (cellToMerge != null)
                {
                    cell.Merge(cellToMerge);
                    anyCellMoved = true;

                    continue;
                }
                var emptyCell = FindEmpty(cell, direction);
                if (emptyCell != null)
                {
                    cell.MoveToOther(emptyCell);
                    anyCellMoved = true;
                }
            }
        }
    }

    private Cell FindEmpty(Cell cell, Vector2 direction)
    {
        Cell emptyCell = null;
        int X_0 = cell.X + (int)direction.x;
        int Y_0 = cell.Y - (int)direction.y;
        for (int x = X_0, y = Y_0; x >= 0 && x < FieldSize && y >= 0 && y < FieldSize; x += (int)direction.x, y -= (int)direction.y)
        {
            if (field[x, y].IsEmpty)
            {
                emptyCell = field[x, y];
            }
            else
            {
                break;
            }
        }
        return emptyCell;
    }

    private Cell FindCellToMerge(Cell cell, Vector2 direction)
    {
        int X_0 = cell.X + (int)direction.x;
        int Y_0 = cell.Y - (int)direction.y;
        for (int x = X_0, y = Y_0; x >= 0 && x < FieldSize && y >= 0 && y < FieldSize; x += (int)direction.x, y -= (int)direction.y)
        {
            if (field[x, y].IsEmpty)
            {
                continue;
            }
            if (field[x, y].Value == cell.Value && !field[x, y].isMerged)
            {
                return field[x, y];
            }
            break;
        }
        return null;
    }

    public void CreateField()
    {
        if (field == null)
        {
            field = new Cell[FieldSize, FieldSize];
            float Width = FieldSize * (CellSize + Spacing) + Spacing;
            rt.sizeDelta = new Vector2(Width, Width);
            float X_0 = -(Width / 2) + (CellSize / 2) + Spacing;
            float Y_0 = -X_0;
            for (int x = 0; x < FieldSize; x++)
            {
                for (int y = 0; y < FieldSize; y++)
                {
                    var cell = Instantiate(cellPref, transform, false);
                    var position = new Vector2(X_0 + (x * (CellSize + Spacing)), Y_0 - (y * (CellSize + Spacing)));
                    cell.transform.localPosition = position;
                    field[x, y] = cell;
                    cell.SetData(x, y, 0);
                }
            }
        }
        for (int x = 0; x < FieldSize; x++)
        {
            for (int y = 0; y < FieldSize; y++)
            {
                field[x, y].SetData(x, y, 0);
            }
        }
        CreateRandom();
        CreateRandom();
    }

    private void CheckResult()
    {
        bool lose = true;
        for (int x = 0; x < FieldSize; ++x)
        {
            for (int y = 0; y < FieldSize; ++y)
            {
                if (field[x, y].Value == 11)
                {
                    GameController.Instance.Win();
                    return;
                }
                if (lose && field[x, y].IsEmpty || FindCellToMerge(field[x, y], Vector2.left) || FindCellToMerge(field[x, y], Vector2.right) || FindCellToMerge(field[x, y], Vector2.up) || FindCellToMerge(field[x, y], Vector2.down))
                {
                    lose = false;
                }
            }
        }
        if (lose)
        {
            GameController.Instance.Lose();
        }
    }

    private void changeFlags()
    {
        for (int x = 0; x < FieldSize; ++x)
        {
            for (int y = 0; y < FieldSize; ++y)
            {
                field[x, y].isMerged = false;
            }
        }
    }

    private void CreateRandom()
    {
        var emptyCells = new List<Cell>();
        for (int x = 0; x < FieldSize; x++)
        {
            for (int y = 0; y < FieldSize; y++)
            {
                if (field[x, y].IsEmpty)
                {
                    emptyCells.Add(field[x, y]);
                }
            }
        }
        int value = Random.Range(0, 10) < 2 ? 2 : 1;
        var cell = emptyCells[Random.Range(0, emptyCells.Count)];
        cell.SetData(cell.X, cell.Y, value);
    }
}
