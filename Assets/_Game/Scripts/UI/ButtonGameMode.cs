using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonGameMode : MonoBehaviour
{
    private enum GameMode
    {
        None,
        VeryEasy,
        Easy,
        Medium,
        Hard,
        Custom,
    }
    [SerializeField] private GameMode _gameMode;
    private Button _button;
    void Awake()
    {
        _button = GetComponent<Button>();
    }
    void OnEnable()
    {
        _button.onClick.AddListener(OnClick);
    }
    void OnDisable()
    {
        _button.onClick.RemoveListener(OnClick);
    }
    private void OnClick()
    {
        switch (_gameMode)
        {
            case GameMode.VeryEasy:
                GameManager.SetBoardSize(3, 3);
                break;
            case GameMode.Easy:
                GameManager.SetBoardSize(4, 4);
                break;
            case GameMode.Medium:
                GameManager.SetBoardSize(5, 5);
                break;
            case GameMode.Hard:
                GameManager.SetBoardSize(6, 6);
                break;
            case GameMode.Custom:
                PopupCustomGame.Show();
                PopupSelectMode.Show();
                return;
        }
        ViewMainMenu.Hide();
        ViewGameplay.Show();
        UIManager.HideAllPopup();
        GameManager.Ins.NewGame();
    }
}