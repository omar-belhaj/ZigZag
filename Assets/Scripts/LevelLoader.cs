using System.Collections;
using System.IO;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;

    private void Awake()
    {
        instance = this;
    }

    public void loadFile(string jsonFileName)
    {
        string jsonPath = Path.Combine(Application.persistentDataPath, jsonFileName);

        // Check if the file exists in the persistentDataPath
        if (!File.Exists(jsonPath))
        {
            CopyFileFromStreamingAssetsToPersistent(jsonFileName);
        }
        else
        {
            Debug.Log("Using existing JSON file from persistentDataPath.");
        }
    }

    private void CopyFileFromStreamingAssetsToPersistent(string jsonFileName)
    {
        string sourcePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        string destinationPath = Path.Combine(Application.persistentDataPath, jsonFileName);

        // Delete existing file if it exists to force update
        if (File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            StartCoroutine(CopyFromStreamingAssets(sourcePath, destinationPath));
        }
        else
        {
            File.Copy(sourcePath, destinationPath);
            Debug.Log("File copied to: " + destinationPath);
        }
    }

    private IEnumerator CopyFromStreamingAssets(string sourcePath, string destinationPath)
    {
        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(sourcePath))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                File.WriteAllText(destinationPath, request.downloadHandler.text);
                Debug.Log("File copied to: " + destinationPath);
            }
            else
            {
                Debug.LogError("Failed to copy file from StreamingAssets: " + request.error);
            }
        }
    }
}