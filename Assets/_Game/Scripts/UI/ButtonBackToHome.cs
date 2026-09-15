using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonBackToHome : MonoBehaviour
{
    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
    }
    void OnEnable()
    {
        _button.onClick.AddListener(BackToHome);
    }

    void OnDisable()
    {
        _button.onClick.RemoveListener(BackToHome);
    }

    private void BackToHome()
    {
        ViewMainMenu.Show();
        ViewGameplay.Hide();
        UIManager.HideAllPopup();
    }
}