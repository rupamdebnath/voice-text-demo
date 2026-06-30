using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinalSceneBehaviour : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI successfulTextOverlay;
    public TextMeshProUGUI concludingText13Overlay;
    public TextMeshProUGUI concludingText8Overlay;
    public TextMeshProUGUI concludingText0Overlay;

    public Button restartButton;

    private int totalScore;
    public SceneDynamicEngine sceneDynamicEngine;
    public StartScene startScene;
    public LightAnimator lightAnimator;


    void OnEnable()
    {
        SetTotalScore(sceneDynamicEngine.GetTotalScore());
        restartButton.onClick.AddListener(RestartSimulation);
        StartCoroutine(InitializeFinalScene());
    }

    private IEnumerator InitializeFinalScene()
    {
        successfulTextOverlay.gameObject.SetActive(false);
        finalScoreText.gameObject.SetActive(false);
        concludingText13Overlay.gameObject.SetActive(false);
        concludingText8Overlay.gameObject.SetActive(false);
        concludingText0Overlay.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        if(totalScore >= 13)
        {
            Debug.Log("Grat");
            finalScoreText.gameObject.SetActive(true);
            successfulTextOverlay.gameObject.SetActive(true);
            yield return sceneDynamicEngine.CanvasGroupFadeIn(finalScoreText.GetComponent<CanvasGroup>());
            yield return sceneDynamicEngine.CanvasGroupFadeIn(successfulTextOverlay.GetComponent<CanvasGroup>());
            yield return new WaitForSeconds(5f);
            yield return sceneDynamicEngine.CanvasGroupFadeOut(successfulTextOverlay.GetComponent<CanvasGroup>());
            successfulTextOverlay.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("else");
            finalScoreText.gameObject.SetActive(true);
            yield return sceneDynamicEngine.CanvasGroupFadeIn(finalScoreText.GetComponent<CanvasGroup>());            
        }
        
        yield return new WaitForSeconds(2f);
        lightAnimator.gameObject.SetActive(true);
        if (totalScore >= 13)
        {
            concludingText13Overlay.gameObject.SetActive(true);
            yield return sceneDynamicEngine.CanvasGroupFadeIn(concludingText13Overlay.GetComponent<CanvasGroup>());
        }
        else if (totalScore >= 8 && totalScore < 13)
        {
            concludingText8Overlay.gameObject.SetActive(true);
            yield return sceneDynamicEngine.CanvasGroupFadeIn(concludingText8Overlay.GetComponent<CanvasGroup>());
        }
        else
        {
            concludingText0Overlay.gameObject.SetActive(true);
            yield return sceneDynamicEngine.CanvasGroupFadeIn(concludingText0Overlay.GetComponent<CanvasGroup>());
        }
        yield return new WaitForSeconds(5f);
        restartButton.gameObject.SetActive(true);
    }

    private void RestartSimulation()
    {
        // Reset the total score
        if (sceneDynamicEngine != null)
        {
            sceneDynamicEngine.SetTotalScore(0);
            sceneDynamicEngine.gameObject.SetActive(false);
        }
        startScene.gameObject.SetActive(true);
        startScene.startScreenPanel.SetActive(true);
        this.transform.parent.gameObject.SetActive(false);
        lightAnimator.gameObject.SetActive(false);
    }
    public void SetTotalScore(int score)
    {
        totalScore = sceneDynamicEngine.GetTotalScore();
        finalScoreText.text = "Final Score: " + totalScore.ToString();
    }
}
