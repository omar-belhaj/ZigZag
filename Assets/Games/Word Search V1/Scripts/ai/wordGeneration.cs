using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;



public class WordGeneration : MonoBehaviour
{
    public static WordGeneration instance;
    private void Awake()
    {
        instance = this;
    }
    private const string apiKey = "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";
    public async Task<string[]> StartMathGeneration(string theme, int age, int difficulty)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("API Key is missing.");
            return null;
        }

        Debug.Log("API Key loaded successfully!");
        int randomValue = UnityEngine.Random.Range(1, 3); // Generates 1 or 2
        string prompt;
        if (randomValue == 1)
        {
            prompt = $"Based on the following information, create a math question suitable for a child of age {age}. " +
                           $"The question should randomly use one of the operators '+' " +
                           $"Return the result as an array in the format: [first number, operator, second number, friendly explanation for a {age}-year-old].";
        }
        else
        {
            prompt = $"Based on the following information, create a math question suitable for a child of age {age}. " +
              $"The question should randomly use one of the operators  '-'. " +
              $"Return the result as an array in the format: [first number, operator, second number, friendly explanation for a {age}-year-old].";
        }



        return await GenerateMathQuestion(prompt);
    }

    private async Task<string[]> GenerateMathQuestion(string prompt)
    {
        var requestBody = new
        {
            model = "gpt-4",
            messages = new[] {
                new { role = "system", content = "You are a math tutor for children." },
                new { role = "user", content = prompt }
            },
            max_tokens = 100,
            temperature = 0.7
        };

        string jsonBody = JsonConvert.SerializeObject(requestBody);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm("https://api.openai.com/v1/chat/completions", jsonBody))
        {
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var response = JsonUtility.FromJson<OpenAIResponse>(request.downloadHandler.text);
                    if (response != null && response.choices.Length > 0)
                    {
                        string resultContent = response.choices[0].message.content.Trim();
                        return ParseMathResponse(resultContent);
                    }
                    else
                    {
                        Debug.LogError("No valid response from API.");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error parsing response: " + ex.Message);
                    return null;
                }
            }
            else
            {
                Debug.LogError("API Request Error: " + request.error);
                return null;
            }
        }
    }

    private string[] ParseMathResponse(string responseContent)
    {
        Debug.Log($"Raw API Response: {responseContent}");

        try
        {
            // Sanitize response and parse into array
            responseContent = responseContent
                .Replace("[", string.Empty)
                .Replace("]", string.Empty)
                .Replace("\"", string.Empty);

            string[] questionArray = responseContent.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < questionArray.Length; i++)
            {
                questionArray[i] = questionArray[i].Trim();
                Debug.Log($"Parsed Element {i}: {questionArray[i]}");
            }

            /*// Ensure response contains the required 4 elements
            if (questionArray.Length != 4)
            {
                Debug.LogError("Unexpected response format. Expected 4 elements but received: " + questionArray.Length);
                return null;
            }*/

            return questionArray;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing the response: {ex.Message}");
            return null;
        }
    }
    public async Task<List<MotInfo>> StartWordGeneration(string theme, int difficulte, int nombreMots, int age)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("Clé API non trouvée.");
            return null;
        }

        Debug.Log("Clé API chargée avec succès !");
        return await GenererMots(theme, difficulte, nombreMots, age);
    }

    private async Task<List<MotInfo>> GenererMots(string theme, int difficulte, int nombreMots, int age)
    {
        // Liste pour stocker les résultats
        var motsInfos = new List<MotInfo>();

        // Étape 1 : Générer les mots avec l'API ChatGPT
        string prompt = $"Génère une liste de {nombreMots} mots en MAJUSCULES, non numérotée, d'une difficulté de {difficulte} sur 10, destinée à des enfants de {age} ans, sur le thème '{theme}'. Chaque mot doit être suivi de sa définition séparée par un deux-points ':'. La somme totale des lettres des mots ne doit pas dépasser 29, et les mots doivent être simples, adaptés à leur âge, et compréhensibles. Exemple de format attendu :\r\nCHAT : Animal domestique qui miaule\r\nSOLEIL : Étoile qui éclaire la Terre";
        var chatRequestBody = new
        {
            model = "gpt-4",
            messages = new[]
            {
            new { role = "system", content = "Tu es un assistant pédagogique spécialisé dans la création de listes de mots adaptées aux enfants." },
            new { role = "user", content = prompt }
        },
            max_tokens = 300,
            temperature = 0.7
        };

        string chatJsonBody = JsonConvert.SerializeObject(chatRequestBody);

        using (UnityWebRequest chatRequest = UnityWebRequest.PostWwwForm("https://api.openai.com/v1/chat/completions", chatJsonBody))
        {
            chatRequest.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            chatRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(chatJsonBody));
            chatRequest.downloadHandler = new DownloadHandlerBuffer();
            chatRequest.SetRequestHeader("Content-Type", "application/json");

            var chatOperation = chatRequest.SendWebRequest();
            while (!chatOperation.isDone)
                await Task.Yield();

            if (chatRequest.result == UnityWebRequest.Result.Success)
            {
                var chatResponse = JsonUtility.FromJson<OpenAIResponse>(chatRequest.downloadHandler.text);
                if (chatResponse != null && chatResponse.choices.Length > 0)
                {
                    // Diviser la réponse en lignes pour récupérer les mots et définitions
                    string[] lignes = chatResponse.choices[0].message.content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var ligne in lignes)
                    {
                        var parties = ligne.Split(':', 2); // Diviser en mot et définition
                        if (parties.Length == 2)
                        {
                            string mot = parties[0].Trim();
                            string definition = parties[1].Trim();

                            // Étape 2 : Générer l'image pour chaque mot
                            string imageUrl = await GenererImage(mot, age);

                            // Ajouter le mot, sa définition et son URL d'image à la liste
                            motsInfos.Add(new MotInfo
                            {
                                Mot = mot,
                                Definition = definition,
                                LienImage = imageUrl
                            });

                            // Arrêter si nous avons atteint le nombre de mots demandé
                            if (motsInfos.Count >= nombreMots)
                                break;
                        }
                    }
                }
                else
                {
                    Debug.LogError("Aucun mot n'a été généré.");
                }
            }
            else
            {
                Debug.LogError("Erreur dans la requête de génération des mots : " + chatRequest.error);
            }
        }

        return motsInfos;
    }

    // Fonction pour générer une image pour un mot donné
    private async Task<string> GenererImage(string description, int age)
    {
        var imageRequestBody = new
        {
            model = "dall-e-3",
            prompt = $"Crée une image de {description}, dans un style de livre pour enfant de {age} ans.",
            n = 1,
            size = "1024x1024"
        };

        string imageJsonBody = JsonConvert.SerializeObject(imageRequestBody);

        using (UnityWebRequest imageRequest = UnityWebRequest.PostWwwForm("https://api.openai.com/v1/images/generations", imageJsonBody))
        {
            imageRequest.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            imageRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(imageJsonBody));
            imageRequest.downloadHandler = new DownloadHandlerBuffer();
            imageRequest.SetRequestHeader("Content-Type", "application/json");

            var imageOperation = imageRequest.SendWebRequest();
            while (!imageOperation.isDone)
                await Task.Yield();

            if (imageRequest.result == UnityWebRequest.Result.Success)
            {
                var imageResponse = JsonUtility.FromJson<ImageResponse>(imageRequest.downloadHandler.text);
                if (imageResponse != null && imageResponse.data.Length > 0)
                {
                    return imageResponse.data[0].url;
                }
                Debug.LogError("Erreur dans la génération de l'image.");
            }
            else
            {
                Debug.LogError("Erreur dans la requête de génération de l'image : " + imageRequest.error);
            }
        }

        return null; // Retourne null en cas d'erreur
    }
}

// Define the missing OpenAIResponse class
[System.Serializable]
public class OpenAIResponse
{
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public Message message;
}

[System.Serializable]
public class Message
{
    public string content;
}

// Define the missing ImageResponse class
[System.Serializable]
public class ImageResponse
{
    public ImageData[] data;
}

[System.Serializable]
public class ImageData
{
    public string url;
}
public class MotInfo
{
    public string Mot;
    public string Definition;
    public string LienImage;
}