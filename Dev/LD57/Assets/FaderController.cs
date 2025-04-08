using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FaderController : MonoBehaviour
{
    public static FaderController instance;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    [SerializeField] private Animator animator;

    private void SetFader()
    {
        animator.SetTrigger("Fader");
    }

    public void FadeOut(int value)
    {
        StartCoroutine(FadeOutCoroutine(value));
    }
    
    private IEnumerator FadeOutCoroutine(int value)
    {
        SetFader();
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(value);
    }
}
