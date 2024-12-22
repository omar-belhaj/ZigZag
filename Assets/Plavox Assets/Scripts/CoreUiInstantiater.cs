using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreUiInstantiater : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private V_Lobby lobbyCanvas_Portrait, lobbyCanvas_Landscape;
    [SerializeField] private GameObject networkCanvas_Portrait, networkCanvas_Landscape;

    private void Start()
    {
        if(gameManager.screenOrientation == ScreenOrientation.Portrait)
        {
            Instantiate(lobbyCanvas_Portrait).gameObject.SetActive(true);
            Instantiate(networkCanvas_Portrait);
        }
        else
        {
            Instantiate(lobbyCanvas_Landscape).gameObject.SetActive(true);
            Instantiate(networkCanvas_Landscape);
        }
        Destroy(gameObject);
    }
}
