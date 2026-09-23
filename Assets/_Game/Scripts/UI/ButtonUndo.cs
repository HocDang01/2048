using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonUndo : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
    }

    private void Update()
    {
        button.interactable = GameManager.Ins != null && GameManager.Ins.CanUndo;
    }

    private void OnClick()
    {
        GameManager.Ins?.Undo();
    }
}
