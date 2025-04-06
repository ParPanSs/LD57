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

    public int Coin { get; set; }
    public int Score { get; set; }
    
    public void IncreaseScore(int value)
    {
        LeanTween.scale(scoreText.gameObject,
            scoreText.gameObject.transform.localScale + new Vector3(0.4f, 0.4f, 0.4f), 0.8f).setEasePunch();
        StartCoroutine(UpdateScoreCounter(Score, value));
    }

    public void IncreaseGold(int value)
    {
        var coin = Instantiate(coinPrefab, spawnPosition.position, Quaternion.identity);
        LeanTween.moveLocal(coin, coinImage.transform.position, .5f).setOnComplete(() =>
        {
            LeanTween.scale(coinImage.gameObject,
                coinImage.gameObject.transform.localScale + new Vector3(0.4f, 0.4f, 0.4f), 0.8f).setEasePunch();
            Destroy(coin);
            StartCoroutine(UpdateCoinCounter(Coin, value));
        });
    }

    private IEnumerator UpdateCoinCounter(int coinAmount, int value)
    {
        for (int i = coinAmount; i <= coinAmount + value; i++)
        {
            goldText.text = $"{i}";
            yield return new WaitForSeconds(.01f);
        }
        Coin = coinAmount + value;
    }
    private IEnumerator UpdateScoreCounter(int amount, int value)
    {
        for (int i = amount; i <= amount + value; i++)
        {
            scoreText.text = $"Score: {i}";
            //yield return new WaitForSeconds(.00000000001f);
        }
        Score = amount + value;
        yield break;
    }
}
