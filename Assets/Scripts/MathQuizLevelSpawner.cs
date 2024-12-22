using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Include this to use SceneManager

public class MathQuizLevelSpawner : MonoBehaviour
{
    public GameObject firstPlaceHolder; // Assign a player prefab in the Inspector
    public GameObject secondPlaceHolder; // Assign an enemy prefab in the Inspector
    public TextMeshProUGUI firstPlaceHolderText;
    public TextMeshProUGUI secondPlaceHolderText;
    public Image operatorPlaceHolder;
    public GameObject[] prefabs; // Prefabs for spawning
    public Sprite[] operators; // Operators images (e.g., +, -)
    public Button[] answers; // Buttons for answers
    public GameObject[] questionnumbers; // Array to hold question number UI elements
   
    // Math question parameters
    public string theme = "Addition";
    public int age = 8;
    public int difficulty = 2;
    public string lastOperator = "+";
    public bool lastCorrect = true;

    // Counter to track number of questions answered
    private int questionCount = 0;

    private void Start()
    {
        SpawnLevel();
    }

    private async void SpawnLevel()
    {
        // Wait for the math question to be generated
        string[] mathQuestion = await WordGeneration.instance.StartMathGeneration(theme, age, difficulty);
        if (popup != null)
            popup.SetActive(false);
        else
            return;

        if (mathQuestion != null && mathQuestion.Length >= 4)
        {
            // Parse the math question
            if (!int.TryParse(mathQuestion[0], out int firstNumber) ||
                !int.TryParse(mathQuestion[2], out int secondNumber) ||
                string.IsNullOrEmpty(mathQuestion[1]))
            {
                Debug.LogError("Invalid math question format.");
                return;
            }

            string operatorSymbol = mathQuestion[1];
             explanation = mathQuestion[3];

            // Spawn objects for the first number
            UpdatePlaceholder(firstPlaceHolder, firstNumber);
            firstPlaceHolderText.text = firstNumber.ToString();

            // Spawn objects for the second number
            UpdatePlaceholder(secondPlaceHolder, secondNumber);
            secondPlaceHolderText.text = secondNumber.ToString();

            // Update the operator image
            SetOperatorImage(operatorSymbol);

            // Update question number display
            UpdateQuestionNumberDisplay(questionCount);

            // Determine the correct answer
            int correctAnswer = CalculateAnswer(firstNumber, secondNumber, operatorSymbol);

            // Assign answers to buttons
            AssignAnswers(correctAnswer);
        }
        else
        {
            Debug.Log("mathQuestion.Length:" + mathQuestion.Length);
            Debug.LogError("Failed to generate a valid math question.");
        }
    }

    private void UpdatePlaceholder(GameObject placeHolder, int count)
    {
        if (placeHolder == null || prefabs == null || prefabs.Length == 0)
        {
            Debug.LogError("Placeholders or prefabs are not properly assigned.");
            return;
        }

        foreach (Transform child in placeHolder.transform)
        {
            Destroy(child.gameObject); // Clear previous objects
        }

        for (int i = 0; i < count; i++)
        {
            Instantiate(prefabs[0], placeHolder.transform);
        }
    }

    public void SetOperatorImage(string operatorSymbol)
    {
        switch (operatorSymbol)
        {
            case "'+'":
                operatorPlaceHolder.sprite = operators[0];
                break;
            case "'-'":
                operatorPlaceHolder.sprite = operators[1];
                break;
            default:
                Debug.LogError($"Unsupported operator: '{operatorSymbol}'");
                break;
        }
    }

    private int CalculateAnswer(int firstNumber, int secondNumber, string operatorSymbol)
    {
        return operatorSymbol switch
        {
            "'+'" => firstNumber + secondNumber,
            "'-'" => firstNumber - secondNumber,
            _ => throw new System.Exception("Unsupported operator."),
        };
    }

    private void AssignAnswers(int correctAnswer)
    {
        HashSet<int> usedAnswers = new() { correctAnswer }; // To avoid duplicate answers
        int correctIndex = Random.Range(0, answers.Length);

        for (int i = 0; i < answers.Length; i++)
        {
            TextMeshProUGUI buttonText = answers[i].GetComponentInChildren<TextMeshProUGUI>();
            answers[i].onClick.RemoveAllListeners(); // Clear previous listeners

            if (i == correctIndex)
            {
                // Assign correct answer
                buttonText.text = correctAnswer.ToString();
                answers[i].onClick.AddListener(() => HandleCorrectAnswer());
            }
            else
            {
                // Assign random incorrect answer
                int incorrectAnswer;
                do
                {
                    incorrectAnswer = Random.Range(correctAnswer - 10, correctAnswer + 10);
                } while (usedAnswers.Contains(incorrectAnswer)); // Ensure unique answers

                usedAnswers.Add(incorrectAnswer);
                buttonText.text = incorrectAnswer.ToString();
                answers[i].onClick.AddListener(() => HandleIncorrectAnswer());
            }
        }
    }

    private void HandleCorrectAnswer()
    {
        Debug.Log("Correct Answer!");
        // Increment question count
        questionCount++;
        ShowPopUp(true);
        // Reset and spawn next question
        ResetAndSpawnNextQuestion();

        // Check if 5 questions have been answered
        if (questionCount >= 5)
        {
            // Load the ThemeChoice scene
            SceneManager.LoadScene("ThemeChoice");
        }
    }

    private void HandleIncorrectAnswer()
    {
        Debug.Log("Incorrect Answer!");
        // Increment question count
        questionCount++;
        ShowPopUp(false);
        // Reset and spawn next question
        ResetAndSpawnNextQuestion();

        // Check if 5 questions have been answered
        if (questionCount == 5)
        {
            // Load the ThemeChoice scene
            SceneManager.LoadScene("ThemeChoice");
        }
    }

    private void ResetAndSpawnNextQuestion()
    {
        // Clear current placeholders
        foreach (Transform child in firstPlaceHolder.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in secondPlaceHolder.transform)
        {
            Destroy(child.gameObject);
        }

        // Spawn a new question
        SpawnLevel();
    }

    private void UpdateQuestionNumberDisplay(int questionIndex)
    {
        // Display question number
        if (questionnumbers.Length > questionIndex)
        {
            questionnumbers[questionIndex].SetActive(true); // Show current question number
        }
    }
    public GameObject popup;
    public GameObject checktrue;
    public GameObject checkfalse;
    public TextMeshProUGUI decription;
    string explanation;
    public void ShowPopUp(bool answer)
    {
        popup.SetActive(true);
            checktrue.SetActive(answer);
           checkfalse.SetActive(!answer);
        decription.text=explanation;
    }
}
