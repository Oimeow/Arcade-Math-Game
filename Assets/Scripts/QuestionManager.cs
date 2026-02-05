using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.IO;
using System.Threading;

public class QuestionManager : MonoBehaviour
{
    // =========================
    // UI
    // =========================
    [Header("UI References")]
    public TextMeshProUGUI feedbackText;
    public Button[] answerButtons; // size 5
    public Button nextButton;

    [Header("Button Colors")]
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    public Color defaultColor = Color.white;

    // =========================
    // GAME STATE
    // =========================
    [Header("Game State")]
    public int difficultyLevel = 1;
    public int streak = 0;

    private Question currentQuestion;
    private bool hasAnswered = false;
    private Dictionary<Button, string> buttonValues = new Dictionary<Button, string>();

    // =========================
    // QUESTION MODEL
    // =========================
    public enum AnswerType { Integer, String }

    [System.Serializable]
    public class Question
    {
        public string latex;
        public AnswerType answerType;

        public int intAnswer;
        public string stringAnswer;

        public Question(string tex, int a)
        {
            latex = tex;
            intAnswer = a;
            answerType = AnswerType.Integer;
        }

        public Question(string tex, string a)
        {
            latex = tex;
            stringAnswer = a;
            answerType = AnswerType.String;
        }

        public bool IsCorrect(string submitted)
        {
            if (answerType == AnswerType.Integer)
            {
                return submitted == intAnswer.ToString();
            }
            else
            {
                return submitted.Replace(" ", "") == stringAnswer.Replace(" ", "");
            }
        }

        public string CorrectAsString()
        {
            return answerType == AnswerType.Integer ? intAnswer.ToString() : stringAnswer;
        }
    }

    void Start()
    {

        feedbackText.text = "";
        nextButton.gameObject.SetActive(false);
        GenerateQuestion();
    }

    // =========================
    // QUESTION GENERATION
    // =========================
    void GenerateQuestion()
    {
        hasAnswered = false;
        feedbackText.text = "";
        nextButton.gameObject.SetActive(false);

        ResetButtonColors();

        currentQuestion = difficultyLevel switch
        {
            1 => GenerateArithmetic(),
            2 => GenerateAlgebra(),
            3 => GenerateQuadratic(),
            4 => GeneratePrecalc(),
            _ => GenerateArithmetic()
        };

        LatexDownloader.Instance.GetPng(currentQuestion.latex);  //  bro I finished this at 2:29 AM. 

        GenerateAnswerChoices();
    }

    // QUESTION TYPES
    Question GenerateArithmetic()
    {
        int a = Random.Range(2, 15);
        int b = Random.Range(2, 15);
        int correct = a * b;

        return new Question($"{a} \\times {b}", correct);
    }

    Question GenerateAlgebra()
    {
        int x = Random.Range(2, 10);
        int a = Random.Range(2, 6);
        int b = Random.Range(1, 10);
        int c = a * x + b;

        string tex = "\\text{Solve for } x\\text{: } " + a.ToString() + "x + " + b.ToString() + " = " + c.ToString();

        return new Question(tex, x);
    }

    Question GenerateQuadratic()
    {
        int x = Random.Range(2, 8);
        int c = x * x;

        return new Question("Solve: x = \\sqrt{" + c.ToString() + "}", x);
    }

    Question GeneratePrecalc()
    {
        int x = Random.Range(2, 6);
        int value = (int)Mathf.Pow(2, x);

        string tex = "Solve: 2^{x} = " + value.ToString();

        return new Question(tex, x);
    }

    // =========================
    // ANSWER GENERATION
    // =========================
    void GenerateAnswerChoices()
    {
        List<string> answers = new List<string>();
        string correct = currentQuestion.CorrectAsString();
        answers.Add(correct);

        while (answers.Count < answerButtons.Length)
        {
            string distractor = GenerateDistractor();
            if (!answers.Contains(distractor))
                answers.Add(distractor);
        }

        // Shuffle
        for (int i = 0; i < answers.Count; i++)
        {
            int j = Random.Range(i, answers.Count);
            (answers[i], answers[j]) = (answers[j], answers[i]);
        }

        buttonValues.Clear();
        for (int i = 0; i < answerButtons.Length; i++)
        {
            string value = answers[i];
            buttonValues[answerButtons[i]] = value;

            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = value;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(value));
        }
    }

    string GenerateDistractor()
    {
        if (currentQuestion.answerType == AnswerType.Integer)
        {
            int baseValue = currentQuestion.intAnswer;

            int[] commonMistakes =
            {
                baseValue + 1,
                baseValue - 1,
                baseValue * 2,
                baseValue / 2,
                -baseValue
            };

            return commonMistakes[Random.Range(0, commonMistakes.Length)].ToString();
        }
        else
        {
            string correct = currentQuestion.stringAnswer;

            if (correct.Contains("x"))
            {
                return Random.value > 0.5f
                    ? correct.Replace("x", "")
                    : correct.Replace("x", "x²");
            }

            return correct + "+C";
        }
    }

    // =========================
    // ANSWER HANDLING
    // =========================
    void OnAnswerSelected(string selected)
    {
        if (hasAnswered) return;
        hasAnswered = true;

        if (currentQuestion.IsCorrect(selected))
        {
            streak++;
            feedbackText.text = "Correct";
        }
        else
        {
            streak = 0;
            feedbackText.text = $"Wrong. Correct: {currentQuestion.CorrectAsString()}";
        }

        // Highlight buttons
        foreach (var btn in answerButtons)
        {
            string val = buttonValues[btn];
            ColorBlock colors = btn.colors;

            if (currentQuestion.IsCorrect(val))
                colors.normalColor = colors.selectedColor = colors.highlightedColor = colors.pressedColor = correctColor;
            else if (val == selected)
                colors.normalColor = colors.selectedColor = colors.highlightedColor = colors.pressedColor = wrongColor;
            else
                colors.normalColor = colors.selectedColor = colors.highlightedColor = colors.pressedColor = defaultColor;

            btn.colors = colors;
        }

        if (streak >= 3)
        {
            difficultyLevel++;
            streak = 0;
            feedbackText.text += "\nDifficulty Increased";
        }

        nextButton.gameObject.SetActive(true);
    }

    void ResetButtonColors()
    {
        foreach (var btn in answerButtons)
        {
            ColorBlock colors = btn.colors;
            colors.normalColor = defaultColor;
            colors.highlightedColor = defaultColor;
            colors.pressedColor = defaultColor;
            btn.colors = colors;
        }
    }

    // =========================
    // NEXT BUTTON
    // =========================
    public void OnNextPressed()
    {
        GenerateQuestion();
    }
}
