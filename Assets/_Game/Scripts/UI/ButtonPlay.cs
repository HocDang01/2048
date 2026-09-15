using UnityEngine;
using UnityEngine.UI;

public class ButtonPlay : MonoBehaviour
{
    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
    }
    void OnEnable()
    {
        _button.onClick.AddListener(Play);
    }

    void OnDisable()
    {
        _button.onClick.RemoveListener(Play);
    }

    private void Play()
    {
        ViewMainMenu.Hide();
        ViewGameplay.Show();
        UIManager.HideAllPopup();
        GameManager.Ins.NewGame();
    }
}