using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class ImprovedQuestionManager : MonoBehaviour
{
    // =========================
    // UI
    // =========================
    [Header("UI References")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI difficultyText;
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
    public int totalCorrect = 0;
    public int totalAttempted = 0;

    private Question currentQuestion;
    private bool hasAnswered = false;
    private Dictionary<Button, string> buttonValues = new Dictionary<Button, string>();

    // =========================
    // QUESTION MODEL
    // =========================
    public enum AnswerType { Integer, Decimal, String }

    [System.Serializable]
    public class Question
    {
        public string prompt;
        public AnswerType answerType;
        public int intAnswer;
        public float decimalAnswer;
        public string stringAnswer;
        public List<string> contextualDistractors; // Specific wrong answers for this question

        public Question(string p, int a, List<string> distractors = null)
        {
            prompt = p;
            intAnswer = a;
            answerType = AnswerType.Integer;
            contextualDistractors = distractors ?? new List<string>();
        }

        public Question(string p, float a, List<string> distractors = null)
        {
            prompt = p;
            decimalAnswer = a;
            answerType = AnswerType.Decimal;
            contextualDistractors = distractors ?? new List<string>();
        }

        public Question(string p, string a, List<string> distractors = null)
        {
            prompt = p;
            stringAnswer = a;
            answerType = AnswerType.String;
            contextualDistractors = distractors ?? new List<string>();
        }

        public bool IsCorrect(string submitted)
        {
            if (answerType == AnswerType.Integer)
            {
                return submitted == intAnswer.ToString();
            }
            else if (answerType == AnswerType.Decimal)
            {
                return submitted == decimalAnswer.ToString("F2");
            }
            else
            {
                return submitted.Replace(" ", "").ToLower() == stringAnswer.Replace(" ", "").ToLower();
            }
        }

        public string CorrectAsString()
        {
            return answerType switch
            {
                AnswerType.Integer => intAnswer.ToString(),
                AnswerType.Decimal => decimalAnswer.ToString("F2"),
                _ => stringAnswer
            };
        }
    }

    // =========================
    // UNITY LIFECYCLE
    // =========================
    void Start()
    {
        feedbackText.text = "";
        nextButton.gameObject.SetActive(false);
        UpdateDifficultyDisplay();
        GenerateQuestion();
    }

    void UpdateDifficultyDisplay()
    {
        if (difficultyText != null)
        {
            string levelName = GetDifficultyName(difficultyLevel);
            difficultyText.text = $"Level {difficultyLevel}: {levelName} | Streak: {streak}";
        }
    }

    string GetDifficultyName(int level)
    {
        return level switch
        {
            1 => "Basic Arithmetic",
            2 => "Advanced Arithmetic",
            3 => "Pre-Algebra",
            4 => "Algebra I",
            5 => "Algebra II",
            6 => "Quadratics",
            7 => "Functions",
            8 => "Precalculus",
            9 => "Calculus I",
            _ => "Advanced Calculus"
        };
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
            1 => GenerateLevel1(),
            2 => GenerateLevel2(),
            3 => GenerateLevel3(),
            4 => GenerateLevel4(),
            5 => GenerateLevel5(),
            6 => GenerateLevel6(),
            7 => GenerateLevel7(),
            8 => GenerateLevel8(),
            9 => GenerateLevel9(),
            _ => GenerateLevel10()
        };

        questionText.text = currentQuestion.prompt;
        GenerateAnswerChoices();
        UpdateDifficultyDisplay();
    }

    // =========================
    // LEVEL 1: BASIC ARITHMETIC
    // =========================
    Question GenerateLevel1()
    {
        int type = Random.Range(0, 4);

        switch (type)
        {
            case 0: // Addition
                {
                    int a = Random.Range(5, 50);
                    int b = Random.Range(5, 50);
                    int answer = a + b;
                    var distractors = new List<string>
                    {
                        (a - b).ToString(),
                        (a + b - 1).ToString(),
                        (a + b + 1).ToString(),
                        (a * 2).ToString()
                    };
                    return new Question($"{a} + {b}", answer, distractors);
                }
            case 1: // Subtraction
                {
                    int a = Random.Range(20, 99);
                    int b = Random.Range(5, a);
                    int answer = a - b;
                    var distractors = new List<string>
                    {
                        (a + b).ToString(),
                        (answer - 1).ToString(),
                        (answer + 1).ToString(),
                        (b - a).ToString()
                    };
                    return new Question($"{a} − {b}", answer, distractors);
                }
            case 2: // Multiplication
                {
                    int a = Random.Range(2, 15);
                    int b = Random.Range(2, 15);
                    int answer = a * b;
                    var distractors = new List<string>
                    {
                        (a + b).ToString(),
                        (answer - a).ToString(),
                        (answer + a).ToString(),
                        (a * b + b).ToString()
                    };
                    return new Question($"{a} × {b}", answer, distractors);
                }
            default: // Division
                {
                    int b = Random.Range(2, 12);
                    int answer = Random.Range(2, 12);
                    int a = b * answer;
                    var distractors = new List<string>
                    {
                        (answer + 1).ToString(),
                        (answer - 1).ToString(),
                        a.ToString(),
                        b.ToString()
                    };
                    return new Question($"{a} ÷ {b}", answer, distractors);
                }
        }
    }

    // =========================
    // LEVEL 2: ADVANCED ARITHMETIC
    // =========================
    Question GenerateLevel2()
    {
        int type = Random.Range(0, 4);

        switch (type)
        {
            case 0: // Two-step operations
                {
                    int a = Random.Range(2, 10);
                    int b = Random.Range(2, 10);
                    int c = Random.Range(1, 10);
                    int answer = a * b + c;
                    var distractors = new List<string>
                    {
                        (a * (b + c)).ToString(),
                        (a * b - c).ToString(),
                        (a + b + c).ToString(),
                        (a * b).ToString()
                    };
                    return new Question($"{a} × {b} + {c}", answer, distractors);
                }
            case 1: // Order of operations
                {
                    int a = Random.Range(2, 8);
                    int b = Random.Range(2, 8);
                    int c = Random.Range(1, 10);
                    int answer = a + b * c;
                    var distractors = new List<string>
                    {
                        ((a + b) * c).ToString(),
                        (a * b + c).ToString(),
                        (a + b + c).ToString(),
                        (b * c).ToString()
                    };
                    return new Question($"{a} + {b} × {c}", answer, distractors);
                }
            case 2: // Squares
                {
                    int a = Random.Range(3, 15);
                    int answer = a * a;
                    var distractors = new List<string>
                    {
                        (a * 2).ToString(),
                        (answer - a).ToString(),
                        (answer + a).ToString(),
                        ((a - 1) * (a - 1)).ToString()
                    };
                    return new Question($"{a}²", answer, distractors);
                }
            default: // Negative numbers
                {
                    int a = Random.Range(5, 20);
                    int b = Random.Range(5, 20);
                    int answer = a - b;
                    var distractors = new List<string>
                    {
                        (b - a).ToString(),
                        Mathf.Abs(answer).ToString(),
                        (a + b).ToString(),
                        "0"
                    };
                    return new Question($"{a} − {b}", answer, distractors);
                }
        }
    }

    // =========================
    // LEVEL 3: PRE-ALGEBRA
    // =========================
    Question GenerateLevel3()
    {
        int type = Random.Range(0, 4);

        switch (type)
        {
            case 0: // Simple equations
                {
                    int x = Random.Range(2, 20);
                    int b = Random.Range(1, 15);
                    int result = x + b;
                    var distractors = new List<string>
                    {
                        (result - x).ToString(),
                        result.ToString(),
                        (x - b).ToString(),
                        b.ToString()
                    };
                    return new Question($"Solve: x + {b} = {result}", x, distractors);
                }
            case 1: // Two-step equations
                {
                    int x = Random.Range(2, 15);
                    int a = Random.Range(2, 8);
                    int b = Random.Range(1, 10);
                    int result = a * x + b;
                    var distractors = new List<string>
                    {
                        ((result - b)).ToString(),
                        (result / a).ToString(),
                        ((result + b) / a).ToString(),
                        a.ToString()
                    };
                    return new Question($"Solve: {a}x + {b} = {result}", x, distractors);
                }
            case 2: // Fractions
                {
                    int num = Random.Range(1, 5);
                    int denom = Random.Range(2, 6);
                    int x = Random.Range(2, 12);
                    int result = (num * x) / denom;
                    if (result * denom != num * x) return GenerateLevel3(); // Ensure integer answer

                    var distractors = new List<string>
                    {
                        (result + 1).ToString(),
                        (result - 1).ToString(),
                        (x).ToString(),
                        (num * x).ToString()
                    };
                    return new Question($"Solve: ({num}/{denom})x = {result}", x, distractors);
                }
            default: // Distributive property
                {
                    int a = Random.Range(2, 6);
                    int b = Random.Range(1, 10);
                    int c = Random.Range(1, 10);
                    int answer = a * b + a * c;
                    var distractors = new List<string>
                    {
                        (a * (b - c)).ToString(),
                        (a * b * c).ToString(),
                        (a + b + c).ToString(),
                        (b + c).ToString()
                    };
                    return new Question($"{a}({b} + {c})", answer, distractors);
                }
        }
    }

    // =========================
    // LEVEL 4: ALGEBRA I
    // =========================
    Question GenerateLevel4()
    {
        int type = Random.Range(0, 4);

        switch (type)
        {
            case 0: // Multi-step equations
                {
                    int x = Random.Range(2, 12);
                    int a = Random.Range(2, 6);
                    int b = Random.Range(1, 8);
                    int c = Random.Range(1, 8);
                    int result = a * x + b;
                    var distractors = new List<string>
                    {
                        ((result - b - c) / a).ToString(),
                        ((result + c) / a).ToString(),
                        (result - b).ToString(),
                        ((result - c) / a).ToString()
                    };
                    return new Question($"Solve: {a}x + {b} = {result} − {c}", x - (c / a), distractors);
                }
            case 1: // Variables on both sides
                {
                    int x = Random.Range(2, 10);
                    int a = Random.Range(3, 8);
                    int b = Random.Range(1, a);
                    int c = Random.Range(1, 10);
                    int d = b * x + c;
                    // ax = bx + c, so (a-b)x = c, x = c/(a-b)
                    var distractors = new List<string>
                    {
                        (c / a).ToString(),
                        (c / b).ToString(),
                        (a - b).ToString(),
                        ((a + b) / c).ToString()
                    };
                    return new Question($"Solve: {a}x = {b}x + {c}", (c / (a - b)), distractors);
                }
            case 2: // Systems (substitution)
                {
                    int x = Random.Range(2, 8);
                    int y = Random.Range(2, 8);
                    int a = Random.Range(1, 5);
                    int b = Random.Range(1, 5);
                    var distractors = new List<string>
                    {
                        y.ToString(),
                        (x + y).ToString(),
                        (x - 1).ToString(),
                        (x + 1).ToString()
                    };
                    return new Question($"If y = {y} and x + y = {x + y}, find x", x, distractors);
                }
            default: // Inequalities
                {
                    int x = Random.Range(3, 15);
                    int a = Random.Range(2, 6);
                    int result = a * x;
                    var distractors = new List<string>
                    {
                        (x - 1).ToString(),
                        (x + 1).ToString(),
                        (result / (a + 1)).ToString(),
                        (result / (a - 1)).ToString()
                    };
                    return new Question($"Solve: {a}x > {result - a}", x, distractors);
                }
        }
    }

    // =========================
    // LEVEL 5: ALGEBRA II
    // =========================
    Question GenerateLevel5()
    {
        int type = Random.Range(0, 3);

        switch (type)
        {
            case 0: // Exponent rules
                {
                    int a = Random.Range(2, 6);
                    int b = Random.Range(2, 5);
                    int c = Random.Range(2, 5);
                    int answer = b + c;
                    var distractors = new List<string>
                    {
                        (b * c).ToString(),
                        (b - c).ToString(),
                        $"{a}^{b * c}",
                        b.ToString()
                    };
                    return new Question($"Simplify: {a}^{b} × {a}^{c} = {a}^?", answer, distractors);
                }
            case 1: // Factoring
                {
                    int a = Random.Range(2, 8);
                    int b = Random.Range(2, 8);
                    int sum = a + b;
                    int product = a * b;
                    var distractors = new List<string>
                    {
                        $"{sum},{product}",
                        $"{a},{a}",
                        $"{b},{b}",
                        $"{product},{sum}"
                    };
                    return new Question($"Factor x² + {sum}x + {product}: (x+?)(x+?)", $"{a},{b}", distractors);
                }
            default: // Rational expressions
                {
                    int a = Random.Range(2, 8);
                    int b = Random.Range(2, 8);
                    int answer = a * b;
                    var distractors = new List<string>
                    {
                        (a + b).ToString(),
                        a.ToString(),
                        b.ToString(),
                        "1"
                    };
                    return new Question($"Simplify: ({a}x / {b}) × {b}", $"{a}x", distractors);
                }
        }
    }

    // =========================
    // LEVEL 6: QUADRATICS
    // =========================
    Question GenerateLevel6()
    {
        int type = Random.Range(0, 3);

        switch (type)
        {
            case 0: // Perfect squares
                {
                    int x = Random.Range(2, 12);
                    int c = x * x;
                    var distractors = new List<string>
                    {
                        (x + 1).ToString(),
                        (x - 1).ToString(),
                        (c / 2).ToString(),
                        (x * 2).ToString()
                    };
                    return new Question($"Solve: x² = {c}", x, distractors);
                }
            case 1: // Completing the square
                {
                    int h = Random.Range(1, 8);
                    int k = Random.Range(1, 10);
                    var distractors = new List<string>
                    {
                        k.ToString(),
                        (h + k).ToString(),
                        $"{k},{h}",
                        $"{h*h},{k}"
                    };
                    return new Question($"Vertex of (x−{h})² + {k}", $"{h},{k}", distractors);
                }
            default: // Quadratic formula roots
                {
                    int root1 = Random.Range(1, 6);
                    int root2 = Random.Range(1, 6);
                    int b = -(root1 + root2);
                    int c = root1 * root2;
                    var distractors = new List<string>
                    {
                        $"{root1 + 1},{root2}",
                        $"{root1},{root2 + 1}",
                        $"{b},{c}",
                        $"{-root1},{-root2}"
                    };
                    return new Question($"Roots of x² {b:+0;-#}x {c:+0;-#}", $"{root1},{root2}", distractors);
                }
        }
    }

    // =========================
    // LEVEL 7: FUNCTIONS
    // =========================
    Question GenerateLevel7()
    {
        int type = Random.Range(0, 3);

        switch (type)
        {
            case 0: // Function evaluation
                {
                    int a = Random.Range(2, 8);
                    int b = Random.Range(1, 10);
                    int x = Random.Range(1, 6);
                    int answer = a * x + b;
                    var distractors = new List<string>
                    {
                        (a * x).ToString(),
                        (a + b).ToString(),
                        (x + b).ToString(),
                        (a * (x + b)).ToString()
                    };
                    return new Question($"If f(x) = {a}x + {b}, find f({x})", answer, distractors);
                }
            case 1: // Composition
                {
                    int a = Random.Range(2, 6);
                    int x = Random.Range(1, 5);
                    int answer = a * (x + 1);
                    var distractors = new List<string>
                    {
                        (a * x + 1).ToString(),
                        (a + x + 1).ToString(),
                        (a * x).ToString(),
                        ((a + 1) * x).ToString()
                    };
                    return new Question($"If f(x) = {a}x and g(x) = x+1, find f(g({x}))", answer, distractors);
                }
            default: // Inverse
                {
                    int a = Random.Range(2, 8);
                    int b = Random.Range(1, 10);
                    var distractors = new List<string>
                    {
                        $"x/{a}+{b}",
                        $"({a}x-{b})",
                        $"{a}x+{b}",
                        $"x-{b}"
                    };
                    return new Question($"Inverse of f(x) = {a}x + {b}", $"(x-{b})/{a}", distractors);
                }
        }
    }

    // =========================
    // LEVEL 8: PRECALCULUS
    // =========================
    Question GenerateLevel8()
    {
        int type = Random.Range(0, 3);

        switch (type)
        {
            case 0: // Logarithms
                {
                    int x = Random.Range(2, 5);
                    int value = (int)Mathf.Pow(2, x);
                    var distractors = new List<string>
                    {
                        (x - 1).ToString(),
                        (x + 1).ToString(),
                        value.ToString(),
                        (value / 2).ToString()
                    };
                    return new Question($"Solve: 2^x = {value}", x, distractors);
                }
            case 1: // Trig basics
                {
                    var answers = new Dictionary<string, string>
                    {
                        { "sin(0°)", "0" },
                        { "sin(90°)", "1" },
                        { "cos(0°)", "1" },
                        { "cos(90°)", "0" },
                        { "tan(45°)", "1" }
                    };
                    var pair = answers.ElementAt(Random.Range(0, answers.Count));
                    var distractors = new List<string> { "0", "1", "-1", "0.5", "√2" };
                    distractors.Remove(pair.Value);
                    return new Question(pair.Key, pair.Value, distractors);
                }
            default: // Sequences
                {
                    int a = Random.Range(2, 8);
                    int d = Random.Range(2, 6);
                    int n = Random.Range(3, 7);
                    int answer = a + (n - 1) * d;
                    var distractors = new List<string>
                    {
                        (a + n * d).ToString(),
                        (a * n).ToString(),
                        (answer - d).ToString(),
                        (a + d * d).ToString()
                    };
                    return new Question($"Arithmetic seq: a_1={a}, d={d}. Find a_{n}", answer, distractors);
                }
        }
    }

    // =========================
    // LEVEL 9: CALCULUS I
    // =========================
    Question GenerateLevel9()
    {
        int type = Random.Range(0, 3);

        switch (type)
        {
            case 0: // Power rule
                {
                    int a = Random.Range(2, 8);
                    int n = Random.Range(2, 6);
                    var distractors = new List<string>
                    {
                        $"{a}x^{n}",
                        $"{a * n}x",
                        $"{n}x^{n - 1}",
                        $"{a}x^{n + 1}"
                    };
                    return new Question($"d/dx ({a}x^{n})", $"{a * n}x^{n - 1}", distractors);
                }
            case 1: // Product rule concept
                {
                    int a = Random.Range(2, 6);
                    var distractors = new List<string>
                    {
                        $"{a}x",
                        $"{2 * a}",
                        $"{a}x²",
                        "0"
                    };
                    return new Question($"d/dx ({a}x²)", $"{2 * a}x", distractors);
                }
            default: // Basic integration
                {
                    int a = Random.Range(2, 8);
                    var distractors = new List<string>
                    {
                        $"{a}x+C",
                        $"{a}x³+C",
                        $"x²+C",
                        $"{a / 2}x²"
                    };
                    return new Question($"∫ {a}x dx", $"{a}x²/2+C", distractors);
                }
        }
    }

    // =========================
    // LEVEL 10: ADVANCED CALCULUS
    // =========================
    Question GenerateLevel10()
    {
        int type = Random.Range(0, 3);

        switch (type)
        {
            case 0: // Chain rule
                {
                    int a = Random.Range(2, 6);
                    int b = Random.Range(2, 6);
                    var distractors = new List<string>
                    {
                        $"{a * b}x",
                        $"{a}({b}x+1)",
                        $"{a * b}({b}x+1)",
                        $"{a}x^{b}"
                    };
                    return new Question($"d/dx ({a}({b}x+1)²)", $"{2 * a * b}({b}x+1)", distractors);
                }
            case 1: // Limits
                {
                    int a = Random.Range(2, 8);
                    var distractors = new List<string>
                    {
                        "∞",
                        "0",
                        "undefined",
                        (-a).ToString()
                    };
                    return new Question($"lim(x→∞) {a}/x", "0", distractors);
                }
            default: // Definite integral
                {
                    int a = Random.Range(2, 6);
                    int upper = Random.Range(2, 6);
                    int answer = a * upper;
                    var distractors = new List<string>
                    {
                        (a * upper * upper / 2).ToString(),
                        (a * upper + upper).ToString(),
                        a.ToString(),
                        (2 * a * upper).ToString()
                    };
                    return new Question($"∫ from 0 to {upper} {a} dx", answer, distractors);
                }
        }
    }

    // =========================
    // ANSWER GENERATION
    // =========================
    void GenerateAnswerChoices()
    {
        List<string> answers = new List<string>();
        string correct = currentQuestion.CorrectAsString();
        answers.Add(correct);

        // Add contextual distractors first
        foreach (var distractor in currentQuestion.contextualDistractors)
        {
            if (!answers.Contains(distractor) && answers.Count < answerButtons.Length)
                answers.Add(distractor);
        }

        // Fill remaining slots with generic distractors if needed
        while (answers.Count < answerButtons.Length)
        {
            string distractor = GenerateGenericDistractor();
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
            answerButtons[i].interactable = true;
        }
    }

    string GenerateGenericDistractor()
    {
        // Fallback generic distractors
        if (currentQuestion.answerType == AnswerType.Integer)
        {
            int baseValue = currentQuestion.intAnswer;
            int[] variations =
            {
                baseValue + Random.Range(1, 5),
                baseValue - Random.Range(1, 5),
                baseValue * 2,
                Mathf.Max(1, baseValue / 2),
                -baseValue
            };
            return variations[Random.Range(0, variations.Length)].ToString();
        }
        else if (currentQuestion.answerType == AnswerType.Decimal)
        {
            float baseValue = currentQuestion.decimalAnswer;
            return (baseValue + Random.Range(-2f, 2f)).ToString("F2");
        }
        else
        {
            string[] genericMath = { "x", "2x", "x²", "1", "0", "-x" };
            return genericMath[Random.Range(0, genericMath.Length)];
        }
    }

    // =========================
    // ANSWER HANDLING
    // =========================
    void OnAnswerSelected(string selected)
    {
        if (hasAnswered) return;
        hasAnswered = true;
        totalAttempted++;

        bool isCorrect = currentQuestion.IsCorrect(selected);

        if (isCorrect)
        {
            streak++;
            totalCorrect++;
            feedbackText.text = "Correct!";
            feedbackText.color = correctColor;
        }
        else
        {
            streak = 0;
            feedbackText.text = $"Wrong. Correct answer: {currentQuestion.CorrectAsString()}";
            feedbackText.color = wrongColor;
        }

        // Highlight buttons
        foreach (var btn in answerButtons)
        {
            string val = buttonValues[btn];
            ColorBlock colors = btn.colors;

            if (currentQuestion.IsCorrect(val))
            {
                // Always highlight the correct answer in green
                colors.normalColor = colors.selectedColor =
                    colors.highlightedColor = colors.pressedColor = correctColor;
            }
            else if (val == selected && !isCorrect)
            {
                // Only highlight selected answer in red if it was wrong
                colors.normalColor = colors.selectedColor =
                    colors.highlightedColor = colors.pressedColor = wrongColor;
            }
            else
            {
                // Keep other buttons at default color
                colors.normalColor = colors.selectedColor =
                    colors.highlightedColor = colors.pressedColor = defaultColor;
            }

            btn.colors = colors;
            btn.interactable = false;
        }

        // Check for level up
        if (streak >= 3 && difficultyLevel < 10)
        {
            difficultyLevel++;
            streak = 0;
            feedbackText.text += $"\nDifficulty Increased! Now at {GetDifficultyName(difficultyLevel)}";
        }

        nextButton.gameObject.SetActive(true);
    }

    void ResetButtonColors()
    {
        foreach (var btn in answerButtons)
        {
            ColorBlock colors = btn.colors;
            colors.normalColor = defaultColor;
            colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
            colors.pressedColor = new Color(0.8f, 0.8f, 0.8f);
            btn.colors = colors;
            btn.interactable = true;
        }
    }

    // =========================
    // NEXT BUTTON
    // =========================
    public void OnNextPressed()
    {
        GenerateQuestion();
    }

    // =========================
    // PUBLIC UTILITIES
    // =========================
    public float GetAccuracy()
    {
        return totalAttempted > 0 ? (float)totalCorrect / totalAttempted : 0f;
    }

    public void ResetProgress()
    {
        difficultyLevel = 1;
        streak = 0;
        totalCorrect = 0;
        totalAttempted = 0;
        GenerateQuestion();
    }
}