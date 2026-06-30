using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class DialogueOption
{
    [TextArea(2,3)] public string text;
    public int score;
    public string feedbackIntern;
    public string feedbackSystem;
    [TextArea(2,3)] public string patientReaction;

    public AudioClip patientReactionAudio;
    public AudioClip systemfeedbackAudio;
    public string nextSceneID;
}

[System.Serializable]
public class SceneData
{
    public string sceneID;
    [TextArea(2,3)] public string patientText;
    public AudioClip audioFile;
    public string systemOverlayText;
    public List<DialogueOption> options = new List<DialogueOption>();
}
