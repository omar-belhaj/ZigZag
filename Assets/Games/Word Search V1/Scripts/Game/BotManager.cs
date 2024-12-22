using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WordSearch
{
    public class BotManager : MonoBehaviour
    {
        public static BotManager I;
        private void Awake()
        {
            I = this;
        }
        private bool isBotInputEnabled = false;
        // Start is called before the first frame update
        void Start()
        {
            GameManager.onGameStart.SubscribeAndInvokeIfTriggered(OnGameStart);
            GameManager.onGameEnd.SubscribeAndInvokeIfTriggered(OnGameEnd);
        }

        private void OnDestroy()
        {
            if (GameManager.onGameEnd != null)
                GameManager.onGameEnd.Unsubscribe(OnGameEnd);
        }

        private void OnGameEnd(bool won)
        {
            this.enabled = false;
        }

        private void OnGameStart()
        {
            if (!GameManager.isAgainstBot)
            {
                gameObject.SetActive(false);
            }
        }
        public void EnableBotInput()
        {
            //  Debug.Log("bot turn");
            isBotInputEnabled = true;
            StartGuessing();
        }

        public void DisableBotInput()
        {
            // Debug.Log("player turn");
            isBotInputEnabled = false;
        }
        // Update is called once per frame
        void Update()
        {
            if (!isBotInputEnabled)
                return;
        }

        private void StartGuessing()
        {
            Debug.Log("Bot: StartGuessing()");
            string word = GetRandomWord();
            Debug.Log($"Will try match word: {word}");
            List<GameObject> firstHexList = GetAllHexWithFirstLetter(word);
            List<GameObject> result = new List<GameObject>();
            foreach (GameObject searchStart in firstHexList)
            {
                if (SearchForWord(result, searchStart, word, 0))
                {
                    result.Add(searchStart);
                    result.Reverse();
                    Debug.Log("Found path: ");
                    StartCoroutine(HighlightPath(result));
                    break;
                }
            }
            //CompleteWordCheck(word, firstHexList);
        }

        private WaitForSeconds SwipeDelay = new WaitForSeconds(0.5f);
        private WaitForSeconds ThinkDelay = new WaitForSeconds(1.5f);
        private IEnumerator HighlightPath(List<GameObject> path)
        {
            yield return ThinkDelay;
            foreach (GameObject hex in path)
            {
                TileManager.I.AddSwipedTile(hex);
                yield return SwipeDelay;
                Debug.Log("finished delay: is local :" + TurnManager.I.isLocalPlayerTurn);
                if (TurnManager.I.isLocalPlayerTurn)
                {
                    TileManager.I.CancelSwipe(false);
                    yield break;
                }
            }
            TileManager.I.FinishSwipe(false);

            yield return ThinkDelay;

            if (!TurnManager.I.isLocalPlayerTurn)
                StartGuessing();
        }

        private string GetRandomWord()
        {
            if (TileManager.I.remainedWords.Count == 0)
                return string.Empty;
            int randomIndex = Random.Range(0, TileManager.I.remainedWords.Count);
            return TileManager.I.remainedWords[randomIndex];
        }

        private List<GameObject> GetAllHexWithFirstLetter(string firstLetter)
        {
            List<GameObject> hexList = new List<GameObject>();
            foreach (GameObject hex in HexagonTextSpawner.I.hexagons)
            {
                if (hex.GetComponent<HexagoneTextManager>().hexaText.text == firstLetter[0].ToString() && !hex.GetComponent<HexagoneTextManager>().isLetterGuessed)
                {
                    // hex.GetComponent<HexagoneTextManager>().ColorGuessedTile();
                    hexList.Add(hex);
                }
            }
            return hexList;
        }

        /// <returns>True: when it finds the word</returns>
        private bool SearchForWord(List<GameObject> resultPath, GameObject searchStart, string word, int startIndex)
        {
            Debug.Log($"will explore new path ({startIndex}/{word.Length - 1})");
            if (startIndex == word.Length - 1)// Has reached the word end
            {
                Debug.Log("successfully reached the end!");
                return true;
            }

            char targetLetter = word[startIndex + 1];
            ///Check neighbouring hexes
            List<GameObject> neighbours = searchStart.GetComponent<HexagoneCollitionManager>().GetAvailableCollisionsBot();
            foreach (GameObject neighbour in neighbours)
            {
                HexagoneTextManager neighbourHex = neighbour.GetComponent<HexagoneTextManager>();
                if (neighbourHex.isLetterGuessed)
                    continue;
                if (neighbourHex.Letter == targetLetter)
                {
                    neighbourHex.isLetterGuessed = true;
                    if (SearchForWord(resultPath, neighbour, word, startIndex + 1))
                    {
                        resultPath.Add(neighbour);
                        neighbourHex.isLetterGuessed = false;
                        return true;
                    }
                    neighbourHex.isLetterGuessed = false;
                }
            }
            Debug.Log($"Failed to finish the path ({startIndex}/{word.Length - 1})");
            return false;
        }
    }
}

