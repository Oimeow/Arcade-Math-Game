using TMPro;
using UnityEngine;

public class ModeManager : MonoBehaviour
{
    public bool isExpertMode = false;

    public TextMeshProUGUI modeText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(this);  // persistence across scenes.
    }

    // Update is called once per frame
    void Update()
    {
     
        
    }

    public void LoadMode(bool playExpertMode)
    {
        this.isExpertMode = playExpertMode;
        modeText.text = playExpertMode ? "Expert" : "Normal";
    }
}
