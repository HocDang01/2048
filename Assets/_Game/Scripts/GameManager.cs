using System.Collections;
using TMPro;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Ins { get; private set; }

    [SerializeField] private TileBoard board;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hiscoreText;

    public static int WIDTH;
    public static int HEIGHT;

    public int Score { get; private set; } = 0;
    public bool CanUndo => board != null && board.CanUndo;

    private void Awake()
    {
        if (Ins != null) {
            DestroyImmediate(gameObject);
        } else {
            Ins = this;
        }
    }

    private void OnDestroy()
    {
        if (Ins == this) {
            Ins = null;
        }
    }

    public static void SetBoardSize(int width, int height)
    {
        WIDTH = width;
        HEIGHT = height;
    }

    public void NewGame()
    {
        // reset score
        SetScore(0);
        hiscoreText.text = GetBestScore().ToString();

        // update board state
        board.BuildBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }

    public void GameOver()
    {
        board.enabled = false;
        PopupGameover.Show();
    }

    public void Undo()
    {
        if (board == null || !board.Undo()) return;

        board.enabled = true;
        PopupGameover.Hide();
    }


    public void IncreaseScore(int points)
    {
        SetScore(Score + points);
    }

    public void RestoreScore(int score)
    {
        SetScore(score);
    }

    private void SetScore(int score)
    {
        this.Score = score;
        scoreText.text = score.ToString();

        SaveBestScore();
    }

    private void SaveBestScore()
    {
        int hiscore = GetBestScore();

        if (Score > hiscore) {
            PlayerPrefs.SetInt(GetBestScoreString(), Score);
            hiscoreText.text = $"{Score}";
        }
    }

    public static int GetBestScore()
    {
        return PlayerPrefs.GetInt(GetBestScoreString(), 0);
    }

    public static string GetBestScoreString()
    {
        return $"{Consts.BESTSCORE}_{WIDTH}_{HEIGHT}";
    }

}
