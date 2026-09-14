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

    public int score { get; private set; } = 0;

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

    private void Start()
    {
        NewGame();
    }

    public void SetBoardSize(int width, int height) => board.SetBoardSize(width, height);

    public void NewGame()
    {
        // reset score
        SetScore(0);
        hiscoreText.text = LoadHiscore().ToString();

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


    public void IncreaseScore(int points)
    {
        SetScore(score + points);
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();

        SaveHiscore();
    }

    private void SaveHiscore()
    {
        int hiscore = LoadHiscore();

        if (score > hiscore) {
            PlayerPrefs.SetInt("hiscore", score);
        }
    }

    private int LoadHiscore()
    {
        return PlayerPrefs.GetInt("hiscore", 0);
    }

}
