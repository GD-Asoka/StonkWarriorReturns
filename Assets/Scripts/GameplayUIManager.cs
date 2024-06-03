using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayUIManager : MonoBehaviour
{
    private TMP_Text _scoreText;
    private TMP_Text _newsHeadlineText;
    private TMP_Text _newsFlavourText;

    private void Awake()
    {
        ScoreTag score = GetComponentInChildren<ScoreTag>();
        if (score != null)
        {
            score.gameObject.TryGetComponent<TMP_Text>(out _scoreText);
        }
        NewsHeadlineTag newsHeadline = GetComponentInChildren<NewsHeadlineTag>();
        if (newsHeadline != null)
        {
            newsHeadline.gameObject.TryGetComponent<TMP_Text>(out _newsHeadlineText);
        }
        NewsFlavourTag newsFlavour = GetComponentInChildren<NewsFlavourTag>();
        if (newsFlavour != null)
        {
            newsFlavour.gameObject.TryGetComponent<TMP_Text>(out _newsFlavourText);
        }
    }

    private void OnEnable()
    {
        StockPriceManager.NewsCall += UpdateNews;
    }

    private void OnDisable()
    {
        StockPriceManager.NewsCall -= UpdateNews;        
    }

    private void Update()
    {
        if (_scoreText != null)
        {
            _scoreText.text = $"Total Value: {PlayerScript.INSTANCE.buyoutValue}";
        }
    }

    public void UpdateNews(NewsHeadlineScriptableObject headline)
    {
        _newsHeadlineText.text = headline.newsHeadline;
        _newsFlavourText.text = headline.flavourText;
    }
}
