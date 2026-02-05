using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class QuestionImageLoader : MonoBehaviour
{
    public RawImage questionImage;
    LatexDownloader ld;

    void OnEnable()
    {
        ld = null;
        if (LatexDownloader.Instance)
        {
            ld = LatexDownloader.Instance;
        }
        else { 
            ld = GetComponent<LatexDownloader>();
        }

        ld.OnPngReady += HandlePngReady;
    }

    void OnDisable()
    {
        ld.OnPngReady -= HandlePngReady;
    }

    void HandlePngReady(string path)
    {
        if (!File.Exists(path))
            return;

        byte[] bytes = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(bytes);

        questionImage.texture = tex;
    }
}