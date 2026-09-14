using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Ins {get; private set;}

    void Awake()
    {
        Ins = this;
    }
    void Start()
    {
        ViewMainMenu.Show();
        ViewGameplay.Hide();
        HideAllPopup();
    }

    public static void HideAllPopup()
    {
        PopupCustomGame.Hide();
        PopupGameover.Hide();
        PopupSelectMode.Hide();
    }
}