using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    public Button button;

    private const float WhiteVeilDefaultAlpha = 171f / 255f;

    [Header("UI References")]
    public Image whiteVeilOverlay;
    public GameObject startScreenPanel;
    public List<RawImage> uiElementsToFade = new List<RawImage>();
    
    [Header("Fade Settings")]
    public float fadeDuration = 2f;
    
    private bool isFading = false;
    private float currentFadeTime = 0.0f;

    [Header("Engine")]
    public SceneDynamicEngine sceneDynamicEngine;

    private void Start()
    {
        ResetStartSceneVisualState();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnEnable()
    {
        ResetStartSceneVisualState();
    }

    private void ResetStartSceneVisualState()
    {
        Color overlayColor = whiteVeilOverlay.color;
        overlayColor.a = WhiteVeilDefaultAlpha;
        whiteVeilOverlay.color = overlayColor;
        //Reset default state of UI elements to fully visible
        foreach (RawImage uiElement in uiElementsToFade)
        {
            Color uiColor = uiElement.color;
            uiColor.a = 1.0f;
            uiElement.color = uiColor;
        }

        // fading turned off
        isFading = false;
        currentFadeTime = 0.0f;
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
            currentColor.a = Mathf.Lerp(WhiteVeilDefaultAlpha, 0.0f, progress);
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
        sceneDynamicEngine.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
