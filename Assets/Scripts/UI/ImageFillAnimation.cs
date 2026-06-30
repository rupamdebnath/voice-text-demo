using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections; // Uses your existing DOTween setup

[RequireComponent(typeof(Image))]
public class ImageFillAnimation : MonoBehaviour
{
    private Image fillImage;

    void OnEnable()
    {
        GetComponent<Image>().fillAmount = 0f;
        StartCoroutine(AnimateFill());
    }

    IEnumerator AnimateFill()
    {
        yield return new WaitForSeconds(7f);
        fillImage = GetComponent<Image>();
        
        fillImage.fillAmount = 0f;

        fillImage.DOFillAmount(1f, 2f).SetEase(Ease.Linear);
        GetComponent<AudioSource>().Play(); 
    }
}
