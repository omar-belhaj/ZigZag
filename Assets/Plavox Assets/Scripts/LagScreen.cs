using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LagScreen : MonoBehaviour
{
    private int ping;
    [SerializeField] private GameObject lagIndicator;
    [SerializeField] private TextMeshProUGUI lagText;
    [SerializeField] private GameObject wildLagIndicator;
    [SerializeField] private TextMeshProUGUI wildLagText;

    [SerializeField] private TextMeshProUGUI regionText;

    [SerializeField] private bool forceShowLag;
    [SerializeField] private bool forceShowWildLag;

    private const string ms = " ms";
    private float pingUpdateTimer = 0f;

    private void Update()
    {
        ping = PhotonManager.ping;
        if (forceShowLag || (200 < ping && ping < 500))
        {
            pingUpdateTimer -= Time.deltaTime;
            if (pingUpdateTimer < 0f)
            {
                lagText.text = string.Concat(ping, ms);
                pingUpdateTimer = 0.5f;
            }
            if (!lagIndicator.activeSelf)
            {
                lagIndicator.SetActive(true);
                wildLagIndicator.SetActive(false);
            }
        }
        else if (forceShowWildLag || ping > 500)
        {
            pingUpdateTimer -= Time.deltaTime;
            if (pingUpdateTimer < 0f)
            {
                wildLagText.text = string.Concat(ping, ms);
                pingUpdateTimer = 0.5f;
            }
            if (!wildLagIndicator.activeSelf)
            {
                lagIndicator.SetActive(false);
                wildLagIndicator.SetActive(true);
                regionText.text = $"Connecting to {PhotonManager.Region} servers...";
            }
        }
        else if (lagIndicator.activeSelf)
        {
            pingUpdateTimer = 0f;
            lagIndicator.SetActive(false);
            wildLagIndicator.SetActive(false);
        }
    }
}
