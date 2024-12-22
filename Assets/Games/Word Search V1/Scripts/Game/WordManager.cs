using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace WordSearch
{
    public class WordManager : MonoBehaviour
    {
        public static WordManager instance;

        [Header("References")]
        public GameObject frameCharacterPrefab; // Assign your frame_character prefab here
        public Tilemap tilemap; // Reference to your Tilemap component

        [Header("Word List")]
        public List<string> words = new List<string>()
        {
            "CAT", "DOG", "HAT", "BAT", "SUN", "MOM", "DAD", "CAR", "BUS", "RUN", "FUN",
            "TIME", "GAME", "BOOK", "WORD", "MOUSE", "HAPPY", "LIGHT", "SHINE", "RAIN", "SNOW"
        };

        private List<Vector3Int> usedPositions = new List<Vector3Int>(); // To store used positions

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        private List<Vector3Int> GetNeighboringTiles(Vector3Int currentPos)
        {
            // Define the 6 possible neighboring directions in a hexagonal grid
            List<Vector3Int> neighbors = new List<Vector3Int>
            {
                currentPos + new Vector3Int(1, 0, 0),    // Right
                currentPos + new Vector3Int(-1, 0, 0),   // Left
                currentPos + new Vector3Int(0, 1, 0),    // Top Right
                currentPos + new Vector3Int(0, -1, 0),   // Bottom Left
                currentPos + new Vector3Int(1, 1, 0),    // Top Left
                currentPos + new Vector3Int(-1, -1, 0)   // Bottom Right
            };

            return neighbors;
        }

        public void GenerateWordsOnHexTiles()
        {
            // Pick 5 random words
            List<string> selectedWords = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                string randomWord = words[Random.Range(0, words.Count)];
                selectedWords.Add(randomWord);
            }

            // Place each word on the grid
            foreach (string word in selectedWords)
            {
                // Find a starting position that hasn't been used
                Vector3Int startPos = FindAvailableStartPosition();

                if (startPos != Vector3Int.zero)
                {
                    // Try placing the word in a random direction
                    PlaceWordInGrid(word, startPos);
                }
                else
                {
                    Debug.LogError("No available positions to start the word placement.");
                }
            }
        }

        private Vector3Int FindAvailableStartPosition()
        {
            foreach (var pos in tilemap.cellBounds.allPositionsWithin)
            {
                if (tilemap.HasTile(pos) && !usedPositions.Contains(pos))
                {
                    return pos;
                }
            }
            return Vector3Int.zero; // No available position
        }

        private void PlaceWordInGrid(string word, Vector3Int startPos)
        {
            Vector3Int currentPos = startPos;
            usedPositions.Add(currentPos); // Mark the start position as used

            for (int i = 0; i < word.Length; i++)
            {
                // Convert the tile position to world position
                Vector3 worldPosition = tilemap.GetCellCenterWorld(currentPos);

                // Instantiate a frame_character prefab at the tile's position
                GameObject frameCharacter = Instantiate(frameCharacterPrefab, worldPosition, Quaternion.identity, tilemap.transform);

                // Set the character for this tile
                frameCharacter.GetComponentInChildren<Text>().text = word[i].ToString();

                // Try to get neighboring tiles for the next letter
                List<Vector3Int> neighbors = GetNeighboringTiles(currentPos);

                // Filter out neighbors that have already been used
                neighbors.RemoveAll(pos => usedPositions.Contains(pos) || !tilemap.HasTile(pos));

                // If there are available neighbors, choose one at random for the next letter
                if (neighbors.Count > 0)
                {
                    currentPos = neighbors[Random.Range(0, neighbors.Count)];
                    usedPositions.Add(currentPos); // Mark the new position as used
                }
                else
                {
                    Debug.LogError("No available neighboring tile for placing the next letter.");
                    break;
                }
            }
        }

        private void Start()
        {
            GenerateWordsOnHexTiles();
        }
    }
}
