using UnityEngine;
using UnityEngine.UI;

public class StressChecker : MonoBehaviour
{
    public Image lowIndicator;
    public Image mediumIndicator;
    public Image highIndicator;

    public void SetStressLevel(int optionIndex)
    {
        if(optionIndex == 0)
        {
            lowIndicator.gameObject.SetActive(true);
            mediumIndicator.gameObject.SetActive(false);
            highIndicator.gameObject.SetActive(false);
        }
        else if(optionIndex == 1)
        {
            lowIndicator.gameObject.SetActive(false);
            mediumIndicator.gameObject.SetActive(true);
            highIndicator.gameObject.SetActive(false);
        }
        else if(optionIndex == 2)
        {
            lowIndicator.gameObject.SetActive(false);
            mediumIndicator.gameObject.SetActive(false);
            highIndicator.gameObject.SetActive(true);
        }
    }
}
