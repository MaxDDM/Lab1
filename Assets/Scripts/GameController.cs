using System.IO;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public static int Score { get; set; }
    public static bool GameStarted { get; private set; }
    [SerializeField]
    private TextMeshProUGUI gameResult;
    [SerializeField]
    private TextMeshProUGUI scoreText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (!File.Exists("output.data"))
        {
            StartGame();
        } else
        {
            GameStarted = true;
        }
    }

    public void StartGame()
    {
        gameResult.text = "";
        SetScore(0);
        GameStarted = true;
        GameField.Instance.CreateField();
    }

    public void Win()
    {
        GameStarted = false;
        gameResult.text = "Победа";
    }

    public void Lose()
    {
        GameStarted = false;
        gameResult.text = "Поражение";
    }

    public void AddScore(int score)
    {
        SetScore(Score + score);
    }

    public void SetScore(int score)
    {
        Score = score;
        scoreText.text = Score.ToString();
    }
}
