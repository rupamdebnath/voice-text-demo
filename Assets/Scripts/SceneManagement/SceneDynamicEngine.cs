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
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    public float smoothSpeed = 15f;

    [Header("UI Canvas Components")]
    public TextMeshProUGUI patientAudioText;
    public Button[] choiceButtons;
    public TextMeshProUGUI[] choiceTexts;
    public GameObject patientPanel;
    public GameObject feedbackPanel;
    public TextMeshProUGUI feedbackTextMesh;
    public TextMeshProUGUI goalTextMesh;
    public Button nextBtn;
    public Button feedbackButton;
    public GameObject choicePanel;
    [Header("Audio Output")]
    public AudioSource audioPlayer;
    [Header("Final Scene")]
    public GameObject finalScenePanel;
    public StressChecker stressChecker;
    public LightAnimator lightAnimator;
    private CanvasGroup choicePanelCanvasGroup;
    private CanvasGroup feedbackPanelCanvasGroup;
    private CanvasGroup patientPanelCanvasGroup;

    void OnEnable()
    {
        choicePanelCanvasGroup = choicePanel.GetComponent<CanvasGroup>();
        feedbackPanelCanvasGroup = feedbackPanel.GetComponent<CanvasGroup>();
        patientPanelCanvasGroup = patientPanel.GetComponent<CanvasGroup>();
        feedbackButton.onClick.AddListener(() => StartCoroutine(ClickFeedBackButtonRoutine()));
        sceneDatabase.Clear();

        // 1. Lookup
        foreach (var scene in allScenes)
        {
            sceneDatabase[scene.sceneID] = scene;
        }

        if (audioPlayer != null)
        {
            audioPlayer.Stop();
            audioPlayer.clip = null;
        }

        //patientPanel.SetActive(true);
        Debug.Log("Active" + patientPanel.activeSelf);
        patientAudioText.DOKill();
        patientAudioText.alpha = 1f;
        /*
        patientPanelCanvasGroup.alpha = 1f;
        patientPanelCanvasGroup.interactable = true;
        patientPanelCanvasGroup.blocksRaycasts = false;
        */

        choicePanelCanvasGroup.alpha = 0f;
        choicePanelCanvasGroup.interactable = false;
        choicePanelCanvasGroup.blocksRaycasts = false;
        nextBtn.gameObject.SetActive(false);
        choicePanel.SetActive(false);
        patientPanel.SetActive(false);

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
        if (feedbackPanelCanvasGroup.alpha > 0f)
        {
            yield return CanvasGroupFadeOut(feedbackPanelCanvasGroup);
            feedbackPanel.SetActive(false);
            nextBtn.gameObject.SetActive(false);
        }
        if(nextBtn.gameObject.activeInHierarchy)
        {
            nextBtn.gameObject.SetActive(false);
            yield return CanvasGroupFadeOut(feedbackPanelCanvasGroup);
        }
        if (!sceneDatabase.ContainsKey(targetSceneID))
        {
            LogEvent("DemoCompleted | FinalScore: " + totalScore);
            // Handle conclusion UI transition panel
            finalScenePanel.transform.parent.gameObject.SetActive(true);
            stressChecker.gameObject.SetActive(false);

            yield return CanvasGroupFadeIn(finalScenePanel.GetComponent<CanvasGroup>());
            yield break;
        }

        stressChecker.SetStressLevel(0);
        stressChecker.gameObject.SetActive(true);

        currentActiveScene = sceneDatabase[targetSceneID];
        LogEvent("SceneStarted | ID: " + targetSceneID);

        // Populate dynamic data inputs straight into your canvas fields
        patientAudioText.DOKill();
        patientAudioText.alpha = 1f;
        patientAudioText.text = currentActiveScene.patientText;
        goalTextMesh.text = currentActiveScene.goalText;
        patientPanel.SetActive(true);
        yield return CanvasGroupFadeIn(patientPanelCanvasGroup);

        // Audio
        if (currentActiveScene.patientAudio != null)
        {
            audioPlayer.clip = currentActiveScene.patientAudio;
            audioPlayer.Play();

            yield return new WaitUntil(() => audioPlayer == null || !audioPlayer.isPlaying);
        }

        yield return CanvasGroupFadeOut(patientPanelCanvasGroup);
        
        feedbackTextMesh.text = currentActiveScene.systemOverlayText;
        feedbackPanel.SetActive(true);
        yield return CanvasGroupFadeIn(feedbackPanelCanvasGroup);
        yield return PlayAnyAudio(currentActiveScene.systemOverlayAudio);

        choicePanel.SetActive(true);
        lightAnimator.gameObject.SetActive(true);
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
        lightAnimator.gameObject.SetActive(false);

        //DialogueOption
        DialogueOption selectedOption = currentActiveScene.options[optionIndex];
        yield return CanvasGroupFadeOut(feedbackPanelCanvasGroup);
        //yield return CanvasGroupFadeOut(choicePanelCanvasGroup);
        feedbackButton.gameObject.SetActive(true);

        feedbackTextMesh.text = selectedOption.feedbackIntern;
        // Display the mandatory metric response card
        yield return CanvasGroupFadeIn(feedbackPanelCanvasGroup);

        //User Audio Nurse
        yield return PlayAnyAudio(selectedOption.userAnswerAudio);
        yield return new WaitUntil(() => feedbackButton.gameObject.activeInHierarchy == false);

        // Track the choice logging actions
        LogEvent("OptionSelected | Choice: " + optionIndex);
        totalScore += selectedOption.score;
        LogEvent("ScoreAdded | Added: " + selectedOption.score + " | TotalScore: " + totalScore);

        //System Feedback Audio
        yield return PlayAnyAudio(selectedOption.systemfeedbackAudio);

        yield return CanvasGroupFadeOut(feedbackPanelCanvasGroup);
        patientAudioText.text = selectedOption.patientReaction;
        yield return CanvasGroupFadeIn(patientPanelCanvasGroup);

        //<<Patient Reaction>> And Stress level
        stressChecker.SetStressLevel(optionIndex);
        yield return PatientReactionBlendShapes(selectedOption);

        if (selectedOption.patientReactionAudio != null)
        {
            audioPlayer.clip = selectedOption.patientReactionAudio;
            audioPlayer.Play();
            yield return new WaitUntil(() => audioPlayer == null || !audioPlayer.isPlaying);
        }

        PatientReactionBlendShapesReset();
        stressChecker.SetStressLevel(0);

        yield return CanvasGroupFadeOut(patientPanelCanvasGroup);
        feedbackTextMesh.text = selectedOption.feedbackSystem;
        yield return CanvasGroupFadeIn(feedbackPanelCanvasGroup);

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

    public IEnumerator CanvasGroupFadeIn(CanvasGroup canvasGroup)
    {        
        canvasGroup.DOKill();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        yield return canvasGroup.DOFade(1f, 1f).WaitForCompletion();
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public IEnumerator CanvasGroupFadeOut(CanvasGroup canvasGroup)
    {
        canvasGroup.DOKill();
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        yield return canvasGroup.DOFade(0f, 1f).WaitForCompletion();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void SetTotalScore(int score)
    {
        totalScore = score;
    }
    public int GetTotalScore()
    {
        return totalScore;
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

    IEnumerator PatientReactionBlendShapes(DialogueOption selectedOption)
    {
        Debug.Log("inside");
        int currentIndex = 0;
        List<int> indexes = new List<int> { 1, 2, 3, 5, 13, 14, 15, 16 };
        foreach(int targetIndex in indexes)
        {
            float startweight = skinnedMeshRenderer.GetBlendShapeWeight(targetIndex);
            float targetWeight = selectedOption.targetValues[currentIndex];
            float progress = 0f;

            while (progress < 1f)
            {
                progress += Time.deltaTime * smoothSpeed;
                float newWeight = Mathf.Lerp(startweight, targetWeight, progress);
                skinnedMeshRenderer.SetBlendShapeWeight(targetIndex, newWeight);
                yield return null;
            }

            skinnedMeshRenderer.SetBlendShapeWeight(targetIndex, targetWeight);
            currentIndex++;
        }
    }

    void PatientReactionBlendShapesReset()
    {
        List<int> indexes = new List<int> { 1, 2, 3, 5, 13, 14, 15, 16 };
        foreach(int targetIndex in indexes)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(targetIndex, 0f);
        }
    }

    IEnumerator PlayAnyAudio(AudioClip audioClip)
    {
        if (audioClip != null)
        {
            audioPlayer.GetComponent<AudioMouthController>().enabled = false;
            audioPlayer.clip = audioClip;
            audioPlayer.Play();
            yield return new WaitUntil(() => audioPlayer == null || !audioPlayer.isPlaying);
            audioPlayer.GetComponent<AudioMouthController>().enabled = true;
        }
    }

    IEnumerator ClickFeedBackButtonRoutine()
    {
        feedbackButton.gameObject.SetActive(false);
        yield return CanvasGroupFadeOut(choicePanelCanvasGroup);
    }
}
