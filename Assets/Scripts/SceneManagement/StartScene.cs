using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    public Button button;

    [Header("UI References")]
    public Image whiteVeilOverlay;
    public GameObject startScreenPanel;
    public List<RawImage> uiElementsToFade = new List<RawImage>();
    
    [Header("Fade Settings")]
    public float fadeDuration = 2f;
    
    private bool isFading = false;
    private float currentFadeTime = 0.0f;

    private void Start()
    {
        // fading turned off
        isFading = false;
        
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        Debug.Log("Button clicked! Starting the 1-second background fade transition...");
        
        // Trigger the fade system
        isFading = true;
        currentFadeTime = 0.0f;
    }

    void Update()
    {
        if (isFading)
        {
            currentFadeTime += Time.deltaTime;
            float progress = currentFadeTime / fadeDuration;

            Color currentColor = whiteVeilOverlay.color;
            
            // Fades out alpha value down to 0
            currentColor.a = Mathf.Lerp(0.67f, 0.0f, progress);
            foreach (RawImage uiElement in uiElementsToFade)
            {
                Color uiColor = uiElement.color;
                uiColor.a = Mathf.Lerp(1.0f, 0.0f, progress);
                uiElement.color = uiColor;
            }
            whiteVeilOverlay.color = currentColor;

            if (progress >= 1.0f)
            {
                FinishFadeSequence();
            }
        }
    }

    void FinishFadeSequence()
    {
        isFading = false;
        
        Color finalColor = whiteVeilOverlay.color;
        finalColor.a = 0.0f;
        whiteVeilOverlay.color = finalColor;
        startScreenPanel.SetActive(false);
        
        Debug.Log("Lobby screen cleared. Scene transition complete.");
    }
}
