using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class SceneDynamicEngine : MonoBehaviour
{
    [Header("Data Source")]
    public List<SceneData> allScenes = new List<SceneData>();
    private Dictionary<string, SceneData> sceneDatabase = new Dictionary<string, SceneData>();
    private SceneData currentActiveScene;
    private int totalScore = 0;

    [Header("UI Canvas Components")]
    public TextMeshProUGUI mainSubtitleText;
    public Button[] choiceButtons;
    public TextMeshProUGUI[] choiceTexts;
    public GameObject feedbackPanel;
    public TextMeshProUGUI feedbackTextMesh;
    public Button nextBtn;
    public GameObject choicePanel;
    [Header("Audio Output")]
    public AudioSource audioPlayer;

    private CanvasGroup choicePanelCanvasGroup;
    private CanvasGroup feedbackPanelCanvasGroup;

    void Start()
    {
        choicePanelCanvasGroup = choicePanel.GetComponent<CanvasGroup>();
        feedbackPanelCanvasGroup = feedbackPanel.GetComponent<CanvasGroup>();

        // 1. Lookup
        foreach (var scene in allScenes)
        {
            sceneDatabase[scene.sceneID] = scene;
        }

        // 2. Fire the global event tracker
        LogEvent("DemoStarted");

        // 3. Kickoff with the first operational round data packet
        LoadSceneStage("scene_02");
    }

    public void LoadSceneStage(string targetSceneID)
    {
        StartCoroutine(LoadSceneStageRoutine(targetSceneID));
    }

    private IEnumerator LoadSceneStageRoutine(string targetSceneID)
    {
        if(nextBtn.gameObject.activeInHierarchy)
        {
            nextBtn.gameObject.SetActive(false);
            yield return CanvasGroupFadeOut(feedbackPanelCanvasGroup);
        }
        if (!sceneDatabase.ContainsKey(targetSceneID))
        {
            LogEvent("DemoCompleted | FinalScore: " + totalScore);
            // Handle conclusion UI transition panel here
            yield break;
        }

        currentActiveScene = sceneDatabase[targetSceneID];
        LogEvent("SceneStarted | ID: " + targetSceneID);

        // Populate dynamic data inputs straight into your canvas fields
        mainSubtitleText.DOKill();
        mainSubtitleText.alpha = 1f;
        mainSubtitleText.text = currentActiveScene.patientText;
        feedbackPanel.SetActive(true);
        yield return CanvasGroupFadeIn(feedbackPanelCanvasGroup);

        // Audio
        if (currentActiveScene.audioFile != null)
        {
            audioPlayer.clip = currentActiveScene.audioFile;
            audioPlayer.Play();

            yield return new WaitUntil(() => audioPlayer == null || !audioPlayer.isPlaying);
        }

        yield return CanvasGroupFadeOut(feedbackPanelCanvasGroup);
        
        mainSubtitleText.text = currentActiveScene.systemOverlayText;
        Debug.Log("MainSub" + mainSubtitleText.text);
        yield return CanvasGroupFadeIn(feedbackPanelCanvasGroup);
        choicePanel.SetActive(true);
        yield return CanvasGroupFadeIn(choicePanelCanvasGroup);
        // Setup the choice option cards
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < currentActiveScene.options.Count)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceTexts[i].text = currentActiveScene.options[i].text;
                
                int index = i; // Prevent closure compilation issues
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => StartCoroutine(OnOptionPicked(index)));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false); // Disable unused layout items safely
            }
        }
    }

    IEnumerator OnOptionPicked(int optionIndex)
    {
        DialogueOption selectedOption = currentActiveScene.options[optionIndex];

        yield return CanvasGroupFadeOut(choicePanelCanvasGroup);
        // Track the choice logging actions
        LogEvent("OptionSelected | Choice: " + optionIndex);
        totalScore += selectedOption.score;
        LogEvent("ScoreAdded | Added: " + selectedOption.score + " | TotalScore: " + totalScore);

        mainSubtitleText.text = selectedOption.feedbackIntern;
        // Display the mandatory metric response card
        yield return CanvasGroupFadeIn(feedbackPanelCanvasGroup);

        //<<Patient Reaction>>
        if (selectedOption.patientReactionAudio != null)
        {
            audioPlayer.clip = selectedOption.patientReactionAudio;
            audioPlayer.Play();
            yield return new WaitUntil(() => audioPlayer == null || !audioPlayer.isPlaying);
        }
        yield return CanvasGroupFadeOut(feedbackPanelCanvasGroup);
        mainSubtitleText.text = selectedOption.feedbackSystem;
        yield return CanvasGroupFadeIn(feedbackPanelCanvasGroup);
        /*
        feedbackTextMesh.text = "<b>Feedback:</b> " + selectedOption.feedback + "\n\n" +
                              "<b>Patient:</b> " + selectedOption.patientReaction;
        
        LogEvent("FeedbackShown");*/

        // Prepare the Next Button trigger to transition inside this single engine
        Debug.Log("Next Button prepared to load scene: " + selectedOption.nextSceneID);
        nextBtn.gameObject.SetActive(true);
        nextBtn.onClick.RemoveAllListeners();
        nextBtn.onClick.AddListener(() => LoadSceneStage(selectedOption.nextSceneID));
    }

    // COnsole Logger
    void LogEvent(string eventName)
    {
        Debug.Log($"<color=#00F0FF>[AEVUS EVENT]</color> {eventName} | Time: {Time.time:F2}s");
    }

    IEnumerator CanvasGroupFadeIn(CanvasGroup canvasGroup)
    {        
        canvasGroup.DOKill();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        yield return canvasGroup.DOFade(1f, 1f).WaitForCompletion();
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    IEnumerator CanvasGroupFadeOut(CanvasGroup canvasGroup)
    {
        canvasGroup.DOKill();
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        yield return canvasGroup.DOFade(0f, 1f).WaitForCompletion();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void OnDestroy()
    {
        // Clean up any lingering DOTween tweens to prevent memory leaks
        DOTween.KillAll();
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].onClick.RemoveAllListeners();
        }
    }   
}
