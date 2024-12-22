using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    [SerializeField] private Button sendMessageButton;
    [SerializeField] private TMP_InputField messageIF;
    [SerializeField] private Transform messagesParentT;
    [SerializeField] private MessageItem botMessageItemRef;
    [SerializeField] private MessageItem localMessageItemRef;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject botIsTyping;

    [SerializeField] private TMP_Dropdown choiceDropdown;

    void Start()
    {
        sendMessageButton.onClick.AddListener(Button_SendMessage);
        SocketManager.I.onBotMessage += OnBotMessage;
    }

    private void OnBotMessage(string message)
    {
        Debug.Log("Received Bot Message: " + message);
        InstantiateMessage(botMessageItemRef, message);
        scrollRect.normalizedPosition = Vector2.zero;
        botIsTyping.SetActive(false);
    }

    private void InstantiateMessage(MessageItem messageItemRef, string message)
    {
        MessageItem messageItem = Instantiate(messageItemRef, messagesParentT);
        messageItem.gameObject.SetActive(true);
        messageItem.Initialize(message);
    }

    private void Button_SendMessage()
    {
        if (string.IsNullOrWhiteSpace(messageIF.text))
            return;
        SocketManager.I.SendMessageToBot(messageIF.text, choiceDropdown.options[choiceDropdown.value].text);
        InstantiateMessage(localMessageItemRef, messageIF.text);
        messageIF.text = string.Empty;
        StartCoroutine(AnimateTyping());
    }

    private IEnumerator AnimateTyping()
    {
        yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        botIsTyping.transform.SetSiblingIndex(botIsTyping.transform.parent.childCount);
        botIsTyping.SetActive(true);
    }
}
