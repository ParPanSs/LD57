using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject coinImage;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private AudioClip changeGoldCount;
    public int Coin { get; private set; }
    public int Score { get; private set; }

    public void IncreaseScore(int value)
    {
        LeanTween.scale(scoreText.gameObject,
            scoreText.gameObject.transform.localScale + new Vector3(0.4f, 0.4f, 0.4f), 0.8f).setEasePunch();
        StartCoroutine(UpdateScoreCounter(Score, value));
    }

    public void IncreaseGold(int value)
    {
        GameManager.Instance.PlaySFX(changeGoldCount);
        var coin = Instantiate(coinPrefab, spawnPosition.position, Quaternion.identity);
        LeanTween.moveLocal(coin, coinImage.transform.position, .5f).setOnComplete(() =>
        {
            LeanTween.scale(coinImage.gameObject,
                coinImage.gameObject.transform.localScale + new Vector3(0.4f, 0.4f, 0.4f), 0.8f).setEasePunch();
            Destroy(coin);
            StartCoroutine(UpdateCoinCounter(Coin, value, true));
        });
    }
    public void DecreaseGold(int value)
    {
        //GameManager.Instance.PlaySFX(changeGoldCount);
        StartCoroutine(UpdateCoinCounter(Coin, -value, false));
    }

    private IEnumerator UpdateCoinCounter(int coinAmount, int value, bool addCoin)
    {
        Coin = coinAmount + value;

        int target = coinAmount + value;
        int step = addCoin ? 1 : -1;

        for (int i = coinAmount; addCoin ? i <= target : i >= target; i += step)
        {
            goldText.text = $"{i}";
            yield return new WaitForSeconds(.01f);
        }
    }

    private IEnumerator UpdateScoreCounter(int amount, int value)
    {
        for (int i = amount; i <= amount + value; i++)
        {
            scoreText.text = $"Score: {i}";
        }
        Score = amount + value;
        yield break;
    }
}
