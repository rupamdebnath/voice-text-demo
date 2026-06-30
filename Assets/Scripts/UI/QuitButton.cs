using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    
    public Button quitButton;
    void Start()
    {
        quitButton = GetComponent<Button>();
        quitButton.onClick.AddListener(QuitApplication);
    }

    public void QuitApplication()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            
            // 2. If we are NOT in the editor (a real build), execute this instead
        #else
            Application.Quit();
        #endif
    }

}
