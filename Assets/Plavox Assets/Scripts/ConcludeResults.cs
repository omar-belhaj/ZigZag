using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

    public class ConcludeResults : MonoBehaviour
    {
        public static ConcludeResults I;

        private bool alreadySentResults = false;

        [System.Serializable]
        public struct AbortEventData
        {
            public string datetime, error_code, winner;
            public string error_description;
            public AbortEventData(string error_code, string winner)
            {
                this.datetime = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
                this.winner = winner;
                this.error_code = error_code;
                error_description = "Player disconnected.";
            }
        }

        [System.Serializable]
        public struct EndEventData
        {
            public string datetime, winner, event_type;
            public int player1Score, player2Score;
            public EndEventData(int player1Score, int player2Score, string winner)
            {
                this.datetime = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
                event_type = "match_ended";
                this.winner = winner;
                this.player1Score = player1Score;
                this.player2Score = player2Score;
            }
        }

        [System.Serializable]
        private struct AbortEvent
        {
            public string token, eventType, message;
            public AbortEventData data;
            public AbortEvent(string winnerId, string loserId, string token)
            {
                this.token = token;
                eventType = "match_aborted";
                message = $"{winnerId} won because ${loserId} aborted the game.";
                data = new AbortEventData("Player disconnected.", winnerId);
            }
        }

        [System.Serializable]
        private struct EndEvent
        {
            public string token, eventType, message;
            public EndEventData data;
            public EndEvent(string winnerId, string loserId, int player1Score, int player2Score, string token)
            {
                this.token = token;
                eventType = "match_ended";
                this.message = $"{winnerId} won against {loserId}";
                data = new EndEventData(player1Score, player2Score, winnerId);
            }
        }

        private void Awake()
        {
            I = this;
        }

        public void SendMatchResults(bool wasGameAborted, string winnerId, string loserId, int localScore, int enemyScore, string returnURL, string token)
        {
            if (alreadySentResults)
                return;
            Debug.Log("will send match results to backend...");

            string jsonData = null;
            if (wasGameAborted)
            {
                AbortEvent abortEvent = new AbortEvent(winnerId, loserId, token);
                jsonData = JsonUtility.ToJson(abortEvent);
            }
            else
            {
                EndEvent endEvent = new EndEvent(winnerId, loserId, (int)localScore, (int)enemyScore, token);
                jsonData = JsonUtility.ToJson(endEvent);
            }
            // Send POST request
            Debug.Log("Match results in JSON Format:\n" + jsonData);
            CallMobileFunction.SendJson(jsonData);
            StartCoroutine(SendRequestCoroutine(returnURL, jsonData));
            alreadySentResults = true;
        }

        private IEnumerator SendRequestCoroutine(string url, string jsonData)
        {
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Notified external system of the victory.");
                }
                else
                {
                    Debug.Log("Error notifying external system: " + request.error);
                }
            }
        }

}
