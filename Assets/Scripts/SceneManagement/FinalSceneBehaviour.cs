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


    void Start()
    {
        restartButton.onClick.AddListener(RestartSimulation);
        StartCoroutine(InitializeFinalScene());
    }

    private IEnumerator InitializeFinalScene()
    {
        if(totalScore >= 13)
        {
            finalScoreText.gameObject.SetActive(true);
            successfulTextOverlay.gameObject.SetActive(true);
            yield return sceneDynamicEngine.CanvasGroupFadeIn(finalScoreText.GetComponent<CanvasGroup>());
            yield return sceneDynamicEngine.CanvasGroupFadeIn(successfulTextOverlay.GetComponent<CanvasGroup>());
        }
        else
        {
            finalScoreText.gameObject.SetActive(true);
            yield return sceneDynamicEngine.CanvasGroupFadeIn(finalScoreText.GetComponent<CanvasGroup>());            
        }

        yield return new WaitForSeconds(2f);
        if (totalScore >= 13)
        {
            concludingText13Overlay.gameObject.SetActive(true);
            yield return sceneDynamicEngine.CanvasGroupFadeIn(concludingText13Overlay.GetComponent<CanvasGroup>());
        }
        else if (totalScore >= 8)
        {
            concludingText8Overlay.gameObject.SetActive(true);
            yield return sceneDynamicEngine.CanvasGroupFadeIn(concludingText8Overlay.GetComponent<CanvasGroup>());
        }
        else
        {
            concludingText0Overlay.gameObject.SetActive(true);
            yield return sceneDynamicEngine.CanvasGroupFadeIn(concludingText0Overlay.GetComponent<CanvasGroup>());
        }
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
    }
    public void SetTotalScore(int score)
    {
        totalScore = score;
        finalScoreText.text = "Final Score: " + totalScore.ToString();
    }
}
