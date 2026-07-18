using System.Collections;
using UnityEngine;

public class PatientAnimation : MonoBehaviour
{
    public Animator patientAnimator;

    // The layer index (0 is the default Base Layer)
    private int baseLayerIndex = 0;

    void Start()
    {
        // Start the loop that periodically attempts to play a random animation
        StartCoroutine(RandomAnimationTimer());
    }

    public void PlayAnyAnimationTrigger(string triggerName)
    {
        if(triggerName != null)
        {
            patientAnimator.SetTrigger(triggerName);
        }
    }

    public void PlayLooktoMedicineAnimation(string triggerName)
    {
        if(triggerName != null)
        {
            patientAnimator.SetTrigger(triggerName);
        }
    }
public bool IsAnimatorFree()
    {
        // 1. Check if the animator is blending between any states
        if (patientAnimator.IsInTransition(baseLayerIndex))
        {
            return false; 
        }

        // 2. Fetch the current playing state info
        AnimatorStateInfo stateInfo = patientAnimator.GetCurrentAnimatorStateInfo(baseLayerIndex);

        if (!stateInfo.IsName("Sitting Idle"))
        {
            return false; 
        }

        return true;
    }
    /// Loops infinitely, checking at random intervals if it can trigger a secondary action
    private IEnumerator RandomAnimationTimer()
    {
        while (true)
        {
            float randomWaitTime = Random.Range(5f, 55f);
            yield return new WaitForSeconds(randomWaitTime);

            // ONLY trigger if the animator is completely free and idling
            if (IsAnimatorFree())
            {
                if(randomWaitTime < 30f)
                {
                    PlayAnyAnimationTrigger("Sidelook");
                }
                else
                {
                    PlayAnyAnimationTrigger("Window");
                }
            }
        }
    }
}
