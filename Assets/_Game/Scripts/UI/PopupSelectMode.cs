using UnityEngine;
using UnityEngine.UI;

public class PopupSelectMode : BaseUI<PopupSelectMode>
{
    [SerializeField] private Button _backBtn;

    void OnEnable()
    {
        _backBtn.onClick.AddListener(Hide);
    }
    void OnDisable()
    {
        _backBtn.onClick.RemoveListener(Hide);
    }
}