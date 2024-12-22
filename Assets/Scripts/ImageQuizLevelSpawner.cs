using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks; // Ensure this is included
using UnityEngine.UI;
using TMPro;

public class ImageQuizLevelSpawner : MonoBehaviour
{
    public Image targetImage; // Assign the UI Image component in the Inspector
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;

    async Task Start()
    {
        List<MotInfo> list = await WordGeneration.instance.StartWordGeneration("animaux", 3, 5, 8);
        StartCoroutine(SpawnLevelsWithDelay(list));
    }

    private IEnumerator SpawnLevelsWithDelay(List<MotInfo> list)
    {
        foreach (MotInfo info in list)
        {
            Debug.Log("mot: " + info.Mot);
            Debug.Log("def: " + info.Definition);

            if (!string.IsNullOrEmpty(info.LienImage))
            {
                Debug.Log("Image URL: " + info.LienImage);

                // Load and display the image
                yield return StartCoroutine(LoadImage(info.LienImage));

                // Wait for 3 seconds before continuing
                yield return new WaitForSeconds(3f);
            }
        }
    }

    private IEnumerator LoadImage(string imagePath)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imagePath);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Get the texture from the request
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            // Convert the texture into a Sprite
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );

            // Assign the sprite to the target Image component
            if (targetImage != null)
            {
                targetImage.sprite = sprite;
                Debug.Log("Image loaded and assigned successfully!");
            }
            else
            {
                Debug.LogError("Target Image component is not assigned!");
            }
        }
        else
        {
            Debug.LogError("Failed to load image: " + request.error);
        }
    }
}
