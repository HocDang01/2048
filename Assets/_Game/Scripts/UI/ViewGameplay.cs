using System;
using UnityEngine;
using UnityEngine.UI;

public class ViewGameplay : BaseUI<ViewGameplay>
{
    [SerializeField] private Button _homeBtn;

    void OnEnable()
    {
        _homeBtn.onClick.AddListener(BackToMainMenu);
    }

    void OnDisable()
    {
        _homeBtn.onClick.RemoveListener(BackToMainMenu);
    }
    private void BackToMainMenu()
    {
        ViewMainMenu.Show();
        ViewGameplay.Hide();
    }
}