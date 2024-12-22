using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;

    public void Initialize(string message)
    {
        messageText.text = message;
        messageText.ForceMeshUpdate();
        RectTransform RT = GetComponent<RectTransform>();
        RT.sizeDelta = new Vector2(RT.sizeDelta.x, messageText.preferredHeight);
    }
}
