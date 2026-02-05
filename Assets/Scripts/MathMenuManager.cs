using UnityEngine;
using UnityEngine.SceneManagement;

public class MathMenuManager : MonoBehaviour
{
    public string mathGameSceneName;

    public void PlayGame() {
        SceneManager.LoadScene(mathGameSceneName);
    }
}
