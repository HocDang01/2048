using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class PopupGameover : BaseUI<PopupGameover>
{
    [SerializeField] private Button _homeBtn;
    [SerializeField] private Button _replayBtn;

    private CanvasGroup _canvasGroup;
    protected override void Awake()
    {
        base.Awake();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        _homeBtn.onClick.AddListener(BackToMainMenu);
        _replayBtn.onClick.AddListener(Replay);
    }
    void OnDisable()
    {
        _homeBtn.onClick.RemoveListener(BackToMainMenu);
        _replayBtn.onClick.RemoveListener(Replay);
    }

    private void Replay()
    {
        GameManager.Ins.NewGame();
        Hide();
    }

    private void BackToMainMenu()
    {
        ViewGameplay.Hide();
        ViewMainMenu.Show();
        Hide();
    }

    protected override void OnShow()
    {
        base.OnShow();
        _canvasGroup.alpha = 0f;
        StartCoroutine(Fade(_canvasGroup, 1f, 0.5f));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }
}