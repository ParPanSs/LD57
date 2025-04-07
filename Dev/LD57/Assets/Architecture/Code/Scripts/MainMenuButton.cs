using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip clickClip;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        sfxSource.PlayOneShot(hoverClip);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        sfxSource.PlayOneShot(clickClip);
    }
}
