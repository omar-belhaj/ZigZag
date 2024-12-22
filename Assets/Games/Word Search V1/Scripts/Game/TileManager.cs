using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using ExampleGame;
using Photon.Realtime;

namespace WordSearch
{
    public class TileManager : MonoBehaviourPun
    {
        public static TileManager I;

        private void Awake()
        {
            I = this;
        }


        [SerializeField] public Color originalColor = Color.white;

        [Header("Hexa Holder Settings")]
        [SerializeField] private GameObject hexaHolder; // The parent object holding all the hexagonal tiles

        public List<GameObject> tiles = new List<GameObject>(); // Holds all the hexagons
        public List<GameObject> swipedTiles = new List<GameObject>(); // Track swiped tiles

        [Header("Word Bank")]
        [SerializeField] public List<string> wordBank = new List<string>(); // List of valid words
        [Header("guessed words")]
        [SerializeField] public List<string> guessedWords = new List<string>(); // List of guessed words
        [Header("remained words")]
        [SerializeField] public List<string> remainedWords = new List<string>(); // List of guessed words
        [Header("Words Holder")]
        public List<GameObject> wordHolder = new List<GameObject>();

        private int swipedCount = 0;
        private GameObject lastTile = null;  // This is the previously swiped tile

        [Header("Tag Settings")]
        public string targetTag = "Hexagon";

        [Header("Sound")]
        [SerializeField] private AudioClip correctGuessSound;

        [Header("Particle system")]
        [SerializeField] private ParticleSystem firework;

        [Header("SFX")]
        public AudioClip colorSFX;
        public AudioClip uncolorSFX;



        private bool isTouching = false;
        public bool isPlayerInputEnabled = true;  // Track if player input is enabled


        private void Start()
        {
            GameManager.onGameStart.SubscribeAndInvokeIfTriggered(OnGameStart);
        }
        public void SetWordBank(List<string> list)
        {
            string[] wordArray = list.ToArray();  // Convert list to array
            wordBank = new List<string>(wordArray);  // Convert array back to list
            remainedWords = new List<string>(wordBank);
            for (int i = 0; i < remainedWords.Count; i++)
            {
                wordHolder[i].GetComponent<WordHolderManager>().SetWord(remainedWords[i]);
            }
            Debug.Log("wordBank populated with words: " + string.Join(", ", wordBank));
            //photonView.RPC("RPCSetWordBank", RpcTarget.All, wordArray);
        }
        /*
        [PunRPC]
        public void RPCSetWordBank(string[] wordArray)
        {
            wordBank = new List<string>(wordArray);  // Convert array back to list
            remainedWords = new List<string>(wordBank);
            for (int i = 0; i < remainedWords.Count; i++)
            {
                wordHolder[i].GetComponent<WordHolderManager>().SetWord(remainedWords[i]);
            }
            Debug.Log("wordBank populated with words: " + string.Join(", ", wordBank));
        }
        */
        private void OnGameStart()
        {
            // Get all child tiles from the Hexa Holder
            if (hexaHolder != null)
            {
                foreach (Transform tile in hexaHolder.transform)
                {
                    if (tile.CompareTag(targetTag))
                    {
                        tiles.Add(tile.gameObject); // Add valid tiles to the list
                    }
                }

                if (tiles.Count == 0)
                {
                    Debug.LogError("No tiles with the tag " + targetTag + " found!");
                }
            }
            else
            {
                Debug.LogError("Hexa Holder is not assigned!");
            }
        }

        private void Update()
        {
            if (!isPlayerInputEnabled) return;  // Disable input when it's not the player's turn

            if (Input.GetMouseButtonDown(0))
            {
                isTouching = true;
                HandleSwipe(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
            else if (Input.GetMouseButton(0) && isTouching)
            {
                HandleSwipe(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isTouching = false;
                CancelSwipe(true);  // Finish the swipe and register the word
               
            }
        }

        private void HandleSwipe(Vector2 touchPos)
        {
            RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);

            if (hit.collider != null && hit.transform.CompareTag(targetTag))
            {
                GameObject currentTile = hit.transform.gameObject;

                // Check if the current tile is already guessed
                HexagoneTextManager hexText = currentTile.GetComponent<HexagoneTextManager>();
                if (hexText != null && hexText.isLetterGuessed)
                {
                    Debug.Log("This tile is already guessed and cannot be swiped.");
                    return; // Exit if the tile is already guessed
                }

                if (currentTile != lastTile)
                {
                    // Check if the tile is adjacent to the previous one (lastTile)
                    if (lastTile != null && !lastTile.GetComponent<HexagoneCollitionManager>().CheckForCollision(currentTile))
                    {
                        Debug.LogWarning("Tile is not adjacent to the previous tile.");
                        return; // Exit if the tile is not adjacent
                    }

                    // Color the current tile and show the arrow on the last tile
                    if (!swipedTiles.Contains(currentTile))
                    {
                        AddSwipedTile(currentTile);
                        //Handheld.Vibrate();
                        if (lastTile != null)
                        {
                            // Show the arrow on the last tile, pointing towards the current one
                            ShowArrowOnLastTile(lastTile, currentTile);
                        }

                        swipedCount++;
                    }
                    else
                    {
                        Backtrack(currentTile);  // Handle backtracking
                    }

                    // Update the last tile to the current one
                    lastTile = currentTile;

                    // Check if the formed word is in the word bank during the swipe
                    CheckForMatchedWord();
                }
            }
        }

        public void AddSwipedTile(GameObject hex)
        {
            swipedTiles.Add(hex);
            hex.GetComponent<HexagoneTextManager>().ColorTile();
        }

        /*
        [PunRPC]
        public void SyncGuessedWord(string playerId, string word)
        {
            Debug.Log($"Word guessed: {word}");
            guessedWords.Add(word);
            PlayerTimerVisual.I.AddPoints();
            remainedWords.Remove(word);
            foreach (GameObject gobject in wordHolder)
            {
                if (guessedWords.Contains(gobject.GetComponent<WordHolderManager>().GetWordText()))
                {
                    gobject.GetComponent<WordHolderManager>().SetWordGuessed();
                }
            }

            firework.Play();
            SoundManager.I.PlaySFX(correctGuessSound);
            ScoreManager.I.AddToScore(playerId == GameManager.player1Id, word.Length);
            CrownScoreManager.I.AddToScore(playerId == GameManager.player1Id, 1);
            // Colors and arrows should remain for the matched word
            foreach (GameObject tile in swipedTiles)
            {
                tile.GetComponent<HexagoneTextManager>().ColorGuessedTile();
            }
             foreach(GameObject arrow in arrowsss)
             {
                arrow.transform.parent?.gameObject.GetComponent<ArrowManager>().ShowArrow();
            }
            // Optionally reset swiped tiles for the next round
            arrowsss.Clear();
            swipedTiles.Clear();
            swipedCount = 0;
            lastTile = null;

        }
        */

        public void SyncWordGuess(string playerId, string word)
        {
            Debug.Log($"Word guessed: {word}");
            guessedWords.Add(word);
            HexagonTextSpawner.I.ShowDescriptionPanel(word);
            PlayerTimerVisual.I.AddPoints();
            remainedWords.Remove(word);
            foreach (GameObject gobject in wordHolder)
            {
                if (guessedWords.Contains(gobject.GetComponent<WordHolderManager>().GetWordText()))
                {
                    gobject.GetComponent<WordHolderManager>().SetWordGuessed();
                }
            }

            firework.Play();
            SoundManager.I.PlaySFX(correctGuessSound);
            ScoreManager.I.AddToScore(playerId == GameManager.player1Id, word.Length);
           // CrownScoreManager.I.AddToScore(playerId == GameManager.player1Id, 1);
            // Colors and arrows should remain for the matched word
            foreach (GameObject tile in swipedTiles)
            {
                tile.GetComponent<HexagoneTextManager>().ColorGuessedTile();
            }
            foreach (GameObject arrow in arrowsss)
            {
                arrow.transform.parent?.gameObject.GetComponent<ArrowManager>().ShowArrow();
            }
            HexagonTextSpawner.I.RespawnWords();

            // Optionally reset swiped tiles for the next round
            arrowsss.Clear();
            swipedTiles.Clear();
            swipedCount = 0;
            lastTile = null;
        }
        public void FinishSwipe(bool localPlayer)
        {
            if (swipedTiles.Count > 0)
            {
                string formedWord = GetWordFromSwipedTiles(); // Get the formed word
                                                              //   Debug.Log("Formed word: " + formedWord);
                if (IsWordInBank(formedWord))
                {
                    Debug.Log("Matched word found: " + formedWord);
                    // Sync the guessed word across all players
                    string playerId = localPlayer ? GameManager.player1Id : GameManager.player2Id;
                    if (GameManager.isAgainstBot)
                    {
                        SyncWordGuess(playerId, formedWord);
                    }
                    else
                    {
                        //photonView.RPC("SyncGuessedWord", RpcTarget.All, playerId, formedWord);
                        SyncWordGuess(playerId, formedWord);
                    }
                }
            }
        }

        private string GetWordFromSwipedTiles()
        {
            string word = "";
            foreach (GameObject tile in swipedTiles)
            {
                // Assuming each tile has a HexagoneTextManager that stores the letter
                HexagoneTextManager hexText = tile.GetComponent<HexagoneTextManager>(); // Adjust if needed
                if (hexText != null)
                {
                    word += hexText.hexaText.text; // Assuming letter is a public string
                }
            }
            return word;
        }

        private bool IsWordInBank(string word)
        {
            return remainedWords.Contains(word);
        }
        string formedWord;
        public void CheckForMatchedWord()
        {
            // Check if the currently swiped tiles form a valid word
            formedWord = GetWordFromSwipedTiles();
            if (IsWordInBank(formedWord))
            {
                Debug.Log("Currently formed word is valid: " + formedWord);
                // You can add additional logic here if needed (e.g., updating UI)
            }
        }
        /*
        [PunRPC]
        private void KeepSwipedTiles()
        {
            ScoreManager.I.AddToScore(true, swipedCount);
            // Colors and arrows should remain for the matched word
            foreach (GameObject tile in swipedTiles)
            {
                tile.GetComponent<HexagoneTextManager>().ColorGuessedTile();
            }
            Debug.Log(formedWord);
            guessedWords.Add(GetWordFromSwipedTiles());
            // Optionally, you can reset the swiped tiles for the next round
            swipedTiles.Clear();
            swipedCount = 0;
            lastTile = null;
        }*/
         public List<GameObject> arrowsss;
        // Show the arrow on the last tile's collider pointing towards the current tile
        public Color localPlayer = Color.blue; // First player's color
        public Color enemyPlayer = Color.red;  // Second player's color
        private void ShowArrowOnLastTile(GameObject lastTile, GameObject currentTile)
        {
            HexagoneCollitionManager collitionManager = lastTile.GetComponent<HexagoneCollitionManager>();
            if (collitionManager == null) return;

            GameObject collidingObject = collitionManager.GetCollidingObject(currentTile);

            if (collidingObject != null)
            {
                Transform arrow = collidingObject.transform.GetChild(0);  // Adjust if the
                                                                          // is not the first child
                if (arrow != null)
                {
                    arrow.gameObject.SetActive(true);// Activate the arrow
                    arrow.gameObject.GetComponent<SpriteRenderer>().color = TurnManager.I.isLocalPlayerTurn ? localPlayer : enemyPlayer;
                    arrowsss.Add(arrow.gameObject);
                }
                else
                {
                    Debug.LogWarning($"No arrow found in collider: {collidingObject.name}");
                }
            }
            else
            {
                Debug.LogWarning("No collision detected between tiles.");
            }
        }

        // Deactivate all arrows in a specific tile
        private void DeactivateArrows(GameObject tile)
        {
            HexagoneCollitionManager collitionManager = tile.GetComponent<HexagoneCollitionManager>();
            if (collitionManager == null) return;
            
            DeactivateArrowInCollider(collitionManager.top);
            DeactivateArrowInCollider(collitionManager.topleft);
            DeactivateArrowInCollider(collitionManager.topright);
            DeactivateArrowInCollider(collitionManager.bot);
            DeactivateArrowInCollider(collitionManager.botleft);
            DeactivateArrowInCollider(collitionManager.botright);
        }

        // Helper function to deactivate a specific arrow
        private void DeactivateArrowInCollider(GameObject colliderObject)
        {

            if (colliderObject != null)
            {
                Transform arrow = colliderObject.transform.GetChild(0);  // Adjust if the arrow is not the first child
                if (arrow != null && arrow.gameObject.activeSelf)
                {
                    arrow.gameObject.SetActive(false);  // Deactivate the arrow
                    
                }
            }
        }

        // Handle backtracking: deactivate arrows and reset tiles when backtracking
        private void Backtrack(GameObject currentTile)
        {
            int tileIndex = swipedTiles.IndexOf(currentTile);

            if (tileIndex != -1 && tileIndex < swipedCount - 1) // If we are backtracking
            {
                for (int i = swipedCount - 1; i > tileIndex; i--) // Resetting colors and arrows for swiped tiles
                {
                    GameObject tileToReset = swipedTiles[i];

                    DeactivateArrows(lastTile);  // Deactivate arrows on the last tile
                    DeactivateArrows(tileToReset);  // Deactivate arrows on the tile being backtracked over
                    
                    tileToReset.GetComponent<HexagoneTextManager>().UncolorTile(i);
                    arrowsss.RemoveAt(arrowsss.Count - 1);
                    //ChangeTileColor(tileToReset, originalColor);

                    swipedTiles.RemoveAt(i);
                    swipedCount--;
                }

                DeactivateArrows(currentTile);
                lastTile = currentTile;
            }
        }

        // Cancel the swipe and reset everything
        public void CancelSwipe(bool localPlayer, bool animated = true)
        {
            FinishSwipe(localPlayer);
            int i = 0;
            foreach (GameObject tile in swipedTiles)
            {
                if (animated)
                    tile.GetComponent<HexagoneTextManager>().UncolorTile(i);
                else
                    tile.GetComponent<SpriteRenderer>().color = originalColor;
                DeactivateArrows(tile);  // Deactivate arrows
                
                i++;
            }
            Debug.Log("swipe canceled");
            arrowsss.Clear();
            swipedTiles.Clear();
            swipedCount = 0;
            lastTile = null;
        }

        // Enable player input for the turn
        public void EnablePlayerInput()
        {
            isPlayerInputEnabled = true;
            //PlayerTimerVisual.I.StartPlayerTurn();
        }

        // Disable player input when it's not the player's turn
        public void DisablePlayerInput()
        {
            CancelSwipe(true);
            
            isPlayerInputEnabled = false;
        }
    }
}
