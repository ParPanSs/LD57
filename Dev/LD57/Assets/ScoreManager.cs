using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private GameObject coinImage;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Transform spawnPosition;
    
    private void Awake()
    {
        Instance = this;
        if (Instance == null)
        {
            Destroy(gameObject);
        }
    }

    public void IncreaseScore(int value)
    {
        
    }

    public void IncreaseGold(int value)
    {
        var text = goldText.text.Split(' ', ':');
        int.TryParse(text[1], out int coinAmount);
        var coin = Instantiate(coinPrefab, spawnPosition.position, Quaternion.identity);
        LeanTween.moveLocal(coin, coinImage.transform.position, .5f).setOnComplete(() =>
        {
            LeanTween.scale(coinImage.gameObject,
                coinImage.gameObject.transform.localScale + new Vector3(0.4f, 0.4f, 0.4f), 0.8f).setEasePunch();
            Destroy(coin);
            StartCoroutine(UpdateCounter(coinAmount, value));
        });
    }

    private IEnumerator UpdateCounter(int coinAmount, int value)
    {
        for (int i = coinAmount; i <= coinAmount + value; i++)
        {
            goldText.text = $"Gold: {i}";
            yield return new WaitForSeconds(.01f);
        }
    }
}
