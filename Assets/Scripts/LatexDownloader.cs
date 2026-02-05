using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LatexDownloader : MonoBehaviour
{
    public event Action<string> OnPngReady;

    public static LatexDownloader Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    Dictionary<char, string> latexBasicDict = new Dictionary<char, string>()
    {
        { '+', "&plus;" },
        { ' ', "&space;" }
    };

    public void GetPng(string latex, string color = "transparent", string downloadType = "png", int dpi = 300)
    {
        if (latex.Length == 0) {
            latex = @"null";  
        }

        string url = ParseUrl(latex, color, downloadType, dpi);

        Debug.Log($"'{latex}' produces URL:\n{url}");

        string filename = $"temp-latex-{DateTime.UtcNow.Ticks}.png";
        StartCoroutine(DownloadFile(url, filename));
    }

    string ParseUrl(string latexStr, string color = "transparent", string downloadType = "png", int dpi = 300)
    {
        string baseUrl = "https://latex.codecogs.com/";

        if (downloadType == "png")
        {
            baseUrl += $"png.image?\\dpi{{{dpi}}}\\bg{{{color}}}";

            string urlFood = "";

            foreach (char c in latexStr)
            {
                if (latexBasicDict.ContainsKey(c))
                    urlFood += latexBasicDict[c];
                else
                    urlFood += c.ToString().Trim();
            }

            return baseUrl + urlFood;
        }

        return null;
    }

    IEnumerator DownloadFile(string url, string filename)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                yield break;
            }

            byte[] data = request.downloadHandler.data;
            string path = Path.Combine(Application.persistentDataPath, filename);
            File.WriteAllBytes(path, data);

            Debug.Log($"Saved to '{path}'");

            // QuestionManager has a listener for this.
            OnPngReady?.Invoke(path);
        }
    }
}
