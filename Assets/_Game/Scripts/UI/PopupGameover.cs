using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class PopupGameover : BaseUI<PopupGameover>
{
    [SerializeField] private Image _title;
    [SerializeField] private Sprite _gameOverSprite;
    [SerializeField] private Sprite _newScoreSprite;
    [SerializeField] private TMP_Text _score;
    [SerializeField] private TMP_Text _bestScore;
    private CanvasGroup _canvasGroup;
    protected override void Awake()
    {
        base.Awake();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        int score = GameManager.Ins.Score;
        int bestScore = GameManager.GetBestScore();
        _score.text = $"{score}";
        _bestScore.text = $"{bestScore}";

        _title.sprite = score >= bestScore? _newScoreSprite : _gameOverSprite;
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