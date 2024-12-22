
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ShowDescription : MonoBehaviour
{
    public static ShowDescription I;
    public Image targetImage;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public string imageLink;

    private void Awake()
    {
        I = this;
    }
    public void DisplayContent(string title,string link,string description)
    {
        this.title.text = title;
        this.description.text = description;
        imageLink = link;
        StartCoroutine(LoadImage(imageLink));
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

    public void OnexitPressed()
    {
        targetImage.sprite=null;
        gameObject.SetActive(false);
    }
}
