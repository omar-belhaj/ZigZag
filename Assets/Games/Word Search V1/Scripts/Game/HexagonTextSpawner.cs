using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.Networking;


namespace WordSearch
{
    public class HexagonTextSpawner : MonoBehaviourPun
    {


        [Header("References")]
        public GameObject[] hexagons; // All hexagon GameObjects

        [Header("Word List")]
        List<string> threeLetterWords = new List<string>()
{
    "CAT", "DOG", "HAT", "BAT", "SUN", "MOM", "DAD", "CAR", "BUS", "RUN",
    "FUN", "ANT", "OWL", "PEN", "NET", "FAN", "KEY", "PIG", "RAT", "ZIP",
    "BOX", "JAM", "WEB", "HUG", "LOG", "VAN", "TOP", "WAR", "TIE", "WET",
    "MAP", "SAD", "JET", "RUG", "MUD", "COW", "POP", "MIX", "POT", "SIP",
    "LAP", "RAY", "TAP", "NUT", "PAD", "HOP", "DIG", "BID", "HEN", "JOY"
};


        List<string> fourLetterWords = new List<string>()
{
    "TIME", "GAME", "BOOK", "WORD", "PARK", "TREE", "FISH", "BALL", "FLAG", "PLAY",
    "POND", "JUMP", "COLD", "FAME", "DUST", "ROAD", "HILL", "FROG", "RING",
    "SAND", "BIRD", "SEAT", "WAVE", "LAMP", "LION", "SNOW", "CUBE", "BAND",
    "CARS", "HERO", "SILK", "PAST", "MILE", "FARM", "RAIN", "FIRE", "SEED", "MIST",
    "PUSH", "CAGE", "WIND", "GIFT", "HIDE", "GLOW", "MEAL", "SPOT", "WORM"
};


        List<string> fiveLetterWords = new List<string>()
{
    "HOUSE", "WATER", "APPLE", "TRAIN", "PLANE", "CHAIR", "BRAIN", "HORSE", "LIGHT", "SOUND",
    "INDIA", "TABLE", "FRESH", "CLOUD", "SMILE", "HEART", "SHEEP", "FLOOR", "DREAM", "GLASS",
    "PIZZA", "CRANE", "FIGHT", "GHOST", "BREAD", "BLOOM", "CROSS", "STORM", "FRUIT", "SHINE",
    "FENCE", "WHEEL", "TRUCK", "DANCE", "MOUSE", "NIGHT", "RIVER", "SPOON", "PLANT", "BLADE",
    "TIGER", "FLAME", "RANCH", "BLOOD", "CANDY", "SUGAR", "SPEAK", "SKATE", "GROWN", "PLUMB",
    "CROWN", "FIELD", "HAPPY", "CATCH", "SWING", "TROOP", "BASIC", "BEACH", "EARTH", "BROWN"
};

        public List<string> selectedWords = new List<string>();

        public static HexagonTextSpawner I;

        private void Awake()
        {
            I = this;
        }

        private void Start()
        {
            GameManager.onGameStart.SubscribeAndInvokeIfTriggered(OnGameStart);
        }

        // Called when the game starts
        public void OnGameStart()
        {
            // Only the master selects the words
            descriptionPanel.SetActive(false);
            SelectWords();
        }
        public List<MotInfo> listInfo;
        private async Task SelectWords()
        {

            listInfo = await WordGeneration.instance.StartWordGeneration("animaux", 3, 6, 8);
            foreach (MotInfo info in listInfo)
            {
                selectedWords.Add(info.Mot);
            }
            TileManager.I.SetWordBank(selectedWords);
            SpawnWords();
        }
        public GameObject descriptionPanel;
        public void ShowDescriptionPanel(string word)
        {
            int index = selectedWords.IndexOf(word);
            descriptionPanel.SetActive(true);
            ShowDescription.I.DisplayContent(listInfo[index].Mot, listInfo[index].LienImage, listInfo[index].Definition);
        }

        public void ReplaceGuessedWords()
        {
            // Temporary list to hold new words to be added to remainedWords
            List<string> newWords = new List<string>();

            foreach (string guessedWord in TileManager.I.guessedWords)
            {
                int wordLength = guessedWord.Length;
                string newWord = GetReplacementWord(wordLength);

                // Add the new word to the temporary list if it’s valid (not empty)
                if (!string.IsNullOrEmpty(newWord))
                {
                    newWords.Add(newWord);
                }
            }

            // Add new words to remainedWords to maintain the list of 6 words
            TileManager.I.remainedWords.AddRange(newWords);

            // Clear guessedWords list after replacing the words
            TileManager.I.guessedWords.Clear();
        }

        private string GetReplacementWord(int length)
        {
            List<string> sourceList;

            // Determine the correct list based on word length
            if (length == 3)
            {
                sourceList = threeLetterWords;
            }
            else if (length == 4)
            {
                sourceList = fourLetterWords;
            }
            else if (length == 5)
            {
                sourceList = fiveLetterWords;
            }
            else
            {
                return null;  // Return null if no matching list exists for the given length
            }

            // Filter out words that are already in remainedWords or guessedWords
            List<string> filteredList = sourceList
                .FindAll(word => !TileManager.I.remainedWords.Contains(word) && !TileManager.I.guessedWords.Contains(word));

            // Return a random word from the filtered list if available
            return filteredList.Count > 0 ? filteredList[Random.Range(0, filteredList.Count)] : null;
        }


        public void RespawnWords()
        {
            // Place the selected words
            //ReplaceGuessedWords();
            //TileManager.I.SetWordBank(TileManager.I.remainedWords);
            foreach (string word in TileManager.I.remainedWords)
            {
                if (!TryPlaceWord(word))
                {
                    Debug.Log(word + " is not placed correctly");
                    ResetHexagons();
                    RespawnWords();
                    return;
                }

            }

            // Fill remaining hexagons with random letters
            FillRemainingHexagons();
            Debug.Log("Wods spawned successfully");
        }


        // Master Client selects and spawns words
        private void SpawnWords()
        {

            // Place the selected words
            foreach (string word in selectedWords)
            {
                if (!TryPlaceWord(word))
                {
                    Debug.Log(word + " is not placed correctly");
                    ResetHexagons();
                    SpawnWords();
                    return;
                }

            }

            // Fill remaining hexagons with random letters
            FillRemainingHexagons();
            Debug.Log("Wods spawned successfully");
        }

        // Utility methods (unchanged from original)
        private List<string> GetRandomWords(List<string> wordList, int count)
        {
            List<string> randomWords = new List<string>();
            List<string> tempList = new List<string>(wordList);  // Temporary copy to avoid modifying the original list

            for (int i = 0; i < count; i++)
            {
                if (tempList.Count == 0) break;

                int randomIndex = Random.Range(0, tempList.Count);
                randomWords.Add(tempList[randomIndex]);
                tempList.RemoveAt(randomIndex);  // Remove the word to avoid duplicates
            }

            return randomWords;
        }

        private string GetRandomWord(List<string> wordList)
        {
            if (wordList.Count == 0) return string.Empty;

            int randomIndex = Random.Range(0, wordList.Count);
            return wordList[randomIndex];
        }

        // Methods for word placement, hexagon checks, etc.
        private bool TryPlaceWord(string word)
        {
            GameObject startHexagon;
            do
            {
                startHexagon = GetRandomHexagon();
            } while (startHexagon != null && (startHexagon.GetComponent<HexagoneTextManager>().isHexOcupied || startHexagon.GetComponent<HexagoneTextManager>().isLetterGuessed));

            if (startHexagon == null)
            {
                Debug.LogWarning("startHexagon is null");
                return false;
            }

            HexagoneTextManager startManager = startHexagon.GetComponent<HexagoneTextManager>();

            startManager.SetHexText(word[0].ToString());

            List<GameObject> occupiedHexagons = new List<GameObject> { startHexagon };
            GameObject currentHex;

            for (int i = 1; i < word.Length; i++)
            {
                currentHex = occupiedHexagons[i - 1];
                List<GameObject> availableNeighbors = currentHex.GetComponent<HexagoneCollitionManager>().GetAvailableCollisions();

                if (availableNeighbors.Count == 0)
                {
                    Debug.LogWarning("No more place in hex " + currentHex.name);
                    return false;
                }

                GameObject nextHexagon = availableNeighbors[Random.Range(0, availableNeighbors.Count)];
                HexagoneTextManager nextManager = nextHexagon.GetComponent<HexagoneTextManager>();

                nextManager.SetHexText(word[i].ToString());
                occupiedHexagons.Add(nextHexagon);
            }
            return true;
        }

        private void ResetHexagons()
        {
            foreach (GameObject hexagon in hexagons)
            {
                HexagoneTextManager hexManager = hexagon.GetComponent<HexagoneTextManager>();

                if (hexManager != null)
                {
                    hexManager.RemoveHexText();
                }
            }
        }

        private void FillRemainingHexagons()
        {
            foreach (GameObject hexagon in hexagons)
            {
                HexagoneTextManager hexManager = hexagon.GetComponent<HexagoneTextManager>();

                if (hexManager != null && !hexManager.isHexOcupied && !hexManager.isLetterGuessed)
                {
                    char randomLetter = (char)Random.Range('A', 'Z' + 1);
                    hexManager.SetHexText(randomLetter.ToString());
                }
            }
        }

        private GameObject GetRandomHexagon()
        {
            List<GameObject> availableHexagons = new List<GameObject>();

            foreach (GameObject hexagon in hexagons)
            {
                HexagoneTextManager hexManager = hexagon.GetComponent<HexagoneTextManager>();

                if (hexManager != null && !hexManager.isHexOcupied && !hexManager.isLetterGuessed)
                {
                    availableHexagons.Add(hexagon);
                }
            }

            if (availableHexagons.Count == 0)
            {
                return null;
            }

            int randomIndex = Random.Range(0, availableHexagons.Count);
            return availableHexagons[randomIndex];
        }
    }
}
