using UnityEngine;
using System.Collections.Generic;

namespace WordSearch
{
    public class TileColorChanger : MonoBehaviour
    {
        private Color originalColor;
        [SerializeField] private Color swipedColor = Color.red;
        private SpriteRenderer tileRenderer;
        private bool isTouching = false;

        // To keep track of the path of swiped tiles
        private List<GameObject> swipedTiles = new List<GameObject>();

        private GameObject[] swipedTileArray = new GameObject[100]; // Adjust size as needed
        private int swipedCount = 0; // Count of swiped tiles
        private GameObject lastTile = null; // Track the last tile swiped

        [Header("Tag Settings")]
        public string targetTag = "Hexagon";



        private void Start()
        {
            tileRenderer = GetComponent<SpriteRenderer>();

            if (tileRenderer != null)
            {
                originalColor = tileRenderer.color; // Use color from the SpriteRenderer
            }
            else
            {
                Debug.LogError("SpriteRenderer component not found on this GameObject!");
            }
        }

        private void Update()
        {
            // Handle input using mouse button instead of touch
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
                isTouching = false; // Reset touch state on mouse release
                CancelSwipe(); // Reset swiped tiles when the mouse is released (cancel the swipe)
            }
        }

        private void HandleSwipe(Vector2 touchPos)
        {
            RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);

            if (hit.collider != null && hit.transform.CompareTag(targetTag))
            {
                GameObject tile = hit.transform.gameObject;

                // Check if the tile is different from the last one to prevent flickering
                if (tile != lastTile)
                {
                    // Check if the tile is adjacent to the previous one (lastTile)
                    if (lastTile != null)
                    {
                        // Check if the last tile is adjacent using the collision manager
                        if (!lastTile.gameObject.GetComponent<HexagoneCollitionManager>().CheckForCollision(tile))
                        {
                            Debug.LogWarning("Tile is not adjacent to the previous tile.");
                            return; // Exit if the tile is not adjacent
                        }
                    }

                    // Check if the tile has already been swiped
                    if (!swipedTiles.Contains(tile))
                    {
                        // Add new tile to the list and color it
                        swipedTiles.Add(tile);
                        tile.GetComponent<SpriteRenderer>().color = swipedColor;

                        // Store in array for backtracking
                        if (swipedCount < swipedTileArray.Length)
                        {
                            swipedTileArray[swipedCount] = tile;
                            swipedCount++;
                        }
                    }
                    else
                    {
                        // Handle backtracking if already swiped
                        Backtrack(tile);
                    }

                    // Update the last tile swiped
                    lastTile = tile;
                }
            }
        }

        // Backtracking method to uncolor and remove tiles from the list
        private void Backtrack(GameObject currentTile)
        {
            int tileIndex = swipedTiles.IndexOf(currentTile);
            if (tileIndex != -1 && tileIndex < swipedCount - 1) // If we are backtracking
            {
                // Remove all tiles ahead of the current one in the array and reset their colors
                for (int i = swipedCount - 1; i > tileIndex; i--)
                {
                    GameObject tileToReset = swipedTileArray[i];
                    tileToReset.GetComponent<SpriteRenderer>().color = originalColor;
                    swipedTiles.Remove(tileToReset); // Also remove from swipedTiles list
                    swipedTileArray[i] = null; // Clear the array entry
                    swipedCount--; // Decrease swiped count
                }
            }
        }

        // Method to reset the last swiped tile and cancel the swipe action
        private void CancelSwipe()
        {
            // Reset the color of all swiped tiles
            foreach (GameObject tile in swipedTiles)
            {
                if (tile != null)
                {
                    tile.GetComponent<SpriteRenderer>().color = originalColor;
                }
            }

            // Clear the list of swiped tiles
            swipedTiles.Clear();
            swipedCount = 0;

            // Reset last tile
            lastTile = null;
        }
    }
}
