using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private Selectable selectable;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip clickClip;

    private void Awake()
    {
        selectable = GetComponent<Selectable>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectable != null && selectable.interactable)
            sfxSource.PlayOneShot(hoverClip);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (selectable != null && selectable.interactable)
            sfxSource.PlayOneShot(clickClip);
    }
}
