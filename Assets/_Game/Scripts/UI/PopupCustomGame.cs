using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupCustomGame : BaseUI<PopupCustomGame>
{
    [SerializeField] private TMP_Text _widthText;
    [SerializeField] private TMP_Text _heightText;
    [SerializeField] private Button _widthAddBtn;
    [SerializeField] private Button _widthMinusBtn;
    [SerializeField] private Button _heightAddBtn;
    [SerializeField] private Button _heightMinusBtn;
    [SerializeField] private Button _backBtn;

    private int _width = 5;
    private int _height = 5;

    private int _maxWidth = 10;
    private int _maxHeight = 10;

    protected override void Awake()
    {
        base.Awake();
        _widthText.text = $"{_width}";
        _heightText.text = $"{_height}";
    }

    void OnEnable()
    {
        _widthAddBtn.onClick.AddListener(AddWidth);
        _widthMinusBtn.onClick.AddListener(MinusWidth);
        _heightAddBtn.onClick.AddListener(AddHeight);
        _heightMinusBtn.onClick.AddListener(MinusHeight);

        _backBtn.onClick.AddListener(OnClickBack);

        _widthText.text = $"{_width}";
        _heightText.text = $"{_height}";
    }


    void OnDisable()
    {
        _widthAddBtn.onClick.RemoveListener(AddWidth);
        _widthMinusBtn.onClick.RemoveListener(MinusWidth);
        _heightAddBtn.onClick.RemoveListener(AddHeight);
        _heightMinusBtn.onClick.RemoveListener(MinusHeight);

        _backBtn.onClick.RemoveListener(OnClickBack);
    }

    private void OnClickBack()
    {
        Hide();
        PopupSelectMode.Show();
    }
    private void UpdateText()
    {
        _widthText.text = $"{_width}";
        _heightText.text = $"{_height}";
        GameManager.SetBoardSize(_width, _height);
    }

    private void AddWidth()
    {
        _width = Mathf.Clamp(++_width, 1, _maxWidth);
        UpdateText();
    }
    private void MinusWidth()
    {
        _width = Mathf.Clamp(--_width, 1, _maxWidth);
        UpdateText();
    }
    private void AddHeight()
    {
        _height = Mathf.Clamp(++_height, 1, _maxHeight);
        UpdateText();
    }
    private void MinusHeight()
    {
        _height = Mathf.Clamp(--_height, 1, _maxHeight);
        UpdateText();
    }

}