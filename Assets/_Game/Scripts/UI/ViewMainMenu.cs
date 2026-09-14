using System;
using UnityEngine;
using UnityEngine.UI;

public class ViewMainMenu : BaseUI<ViewMainMenu>
{
    [SerializeField] private Button _playBtn;

    protected override void Awake()
    {
        base.Awake();
        _playBtn.onClick.AddListener(OnClickPlay);
    }

    private void OnClickPlay()
    {
        PopupSelectMode.Show();
    }
}