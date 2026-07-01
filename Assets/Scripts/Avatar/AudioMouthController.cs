using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioMouthController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private int mouthBlendshapeIndex = 0; // Index of your "Mouth_Open" shape
    [SerializeField] private int eyeCloseIndex = 0; // How smoothly the mouth moves

    [Header("Audio Settings")]
    [Range(0.001f, 0.1f)] 
    [SerializeField] private float threshold = 0.02f;      // Frequency volume required to activate mouth
    [SerializeField] private float sensitivity = 500f;     // How wide the mouth opens based on the volume
    [SerializeField] private float smoothSpeed = 15f;      // How smoothly the mouth moves

    [Header("Blink Settings")]
    [SerializeField] private float blinkInterval = 3f;
    [SerializeField] private float blinkCloseTime = 0.08f;
    [SerializeField] private float blinkOpenTime = 0.12f;

    private AudioSource audioSource;
    private float[] spectrumData = new float[512];         // Must be a power of 2 (64, 128, 256, 512, etc.)
    private float targetBlendshapeValue = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (skinnedMeshRenderer == null)
        {
            skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        StartCoroutine(BlinkLoop());
    }

    void Update()
    {
        if (audioSource == null || !audioSource.isPlaying)
        {
            // If nothing is playing, smoothly close the mouth
            CloseMouthSmoothly();
            return;
        }

        // 1. Get raw FFT frequency sample data from the source channel
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.Blackman);

        // 2. Isolate human voice frequencies.
        // Array index 0 represents 0Hz. The last index represents half of your project sample rate (usually 22050Hz).
        // (44100Hz sample rate / 512 samples) = ~86Hz per array element index slot.
        // Elements index 1 to 5 cover roughly 86Hz to 430Hz (prime human speaking/vocal range).
        float vocalFrequencySum = 0f;
        for (int i = 1; i <= 5; i++)
        {
            vocalFrequencySum += spectrumData[i];
        }

        // 3. Check if the average frequency energy crosses the target noise threshold
        float averageVocalValue = vocalFrequencySum / 5f;

        if (averageVocalValue > threshold)
        {
            // Map the audio intensity to the 0-100 scale Unity blendshapes use
            targetBlendshapeValue = Mathf.Clamp(averageVocalValue * sensitivity, 0f, 100f);
        }
        else
        {
            targetBlendshapeValue = 0f;
        }

        // 4. Smoothly interpolate the blendshape so it doesn't jitter or stutter
        float currentWeight = skinnedMeshRenderer.GetBlendShapeWeight(mouthBlendshapeIndex);
        float newWeight = Mathf.Lerp(currentWeight, targetBlendshapeValue, Time.deltaTime * smoothSpeed);
        
        skinnedMeshRenderer.SetBlendShapeWeight(mouthBlendshapeIndex, newWeight);
    }

    private void CloseMouthSmoothly()
    {
        float currentWeight = skinnedMeshRenderer.GetBlendShapeWeight(mouthBlendshapeIndex);
        float newWeight = Mathf.Lerp(currentWeight, 0f, Time.deltaTime * smoothSpeed);
        skinnedMeshRenderer.SetBlendShapeWeight(mouthBlendshapeIndex, newWeight);
    }

    private IEnumerator BlinkLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(blinkInterval);
            yield return SetEyeWeight(100f, blinkCloseTime);
            yield return SetEyeWeight(0f, blinkOpenTime);
        }
    }

    private IEnumerator SetEyeWeight(float targetWeight, float duration)
    {
        float startWeight = skinnedMeshRenderer.GetBlendShapeWeight(eyeCloseIndex);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float newWeight = Mathf.Lerp(startWeight, targetWeight, t);
            skinnedMeshRenderer.SetBlendShapeWeight(eyeCloseIndex, newWeight);
            yield return null;
        }

        skinnedMeshRenderer.SetBlendShapeWeight(eyeCloseIndex, targetWeight);
    }
}
