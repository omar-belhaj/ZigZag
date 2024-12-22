using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace WordSearch
{
    public class HexagoneCollitionManager : MonoBehaviour
    {
        [SerializeField] public GameObject top;
        [SerializeField] public GameObject topleft;
        [SerializeField] public GameObject topright;
        [SerializeField] public GameObject bot;
        [SerializeField] public GameObject botleft;
        [SerializeField] public GameObject botright;

        public static HexagoneCollitionManager I;

        private void Awake()
        {
            I = this;
        }
        // Method to check if a collision occurs with any neighbor tiles
        public bool CheckForCollision(GameObject target)
        {
            Collider2D targetCollider = target.GetComponent<Collider2D>();

            if (targetCollider == null)
            {
                //     Debug.LogError("Target does not have a Collider2D attached.");
                return false;
            }

            // Check for collisions with all neighbors and log the collided object
            if (IsColliding(top, targetCollider)) return true;
            if (IsColliding(topleft, targetCollider)) return true;
            if (IsColliding(topright, targetCollider)) return true;
            if (IsColliding(bot, targetCollider)) return true;
            if (IsColliding(botleft, targetCollider)) return true;
            if (IsColliding(botright, targetCollider)) return true;

            // If none are colliding, return false
            return false;
        }

        // New method to return the exact colliding object (neighbor)
        public GameObject GetCollidingObject(GameObject target)
        {
            Collider2D targetCollider = target.GetComponent<Collider2D>();

            if (targetCollider == null)
            {
                Debug.LogError("Target does not have a Collider2D attached.");
                return null;
            }

            // Return the specific object that is colliding
            if (IsColliding(top, targetCollider)) return top;
            if (IsColliding(topleft, targetCollider)) return topleft;
            if (IsColliding(topright, targetCollider)) return topright;
            if (IsColliding(bot, targetCollider)) return bot;
            if (IsColliding(botleft, targetCollider)) return botleft;
            if (IsColliding(botright, targetCollider)) return botright;

            return null;  // No collision detected
        }

        // Helper method to check if a neighbor tile is colliding with the target
        private bool IsColliding(GameObject obj, Collider2D targetCollider)
        {
            if (obj == null || obj.GetComponent<Collider2D>() == null) return false;

            Collider2D objCollider = obj.GetComponent<Collider2D>();

            // If colliding, find the child object and activate it (e.g., arrow)
            if (objCollider.IsTouching(targetCollider))
            {
                // Assuming the first child is the arrow or object to be activated
                Transform child = obj.transform.GetChild(0);
                if (child != null && !child.gameObject.activeSelf)
                {
                    child.gameObject.SetActive(true);  // Activate the child object (e.g., arrow)
                                                       //  Debug.Log($"Activated child object of {obj.name}");
                }
                return true;
            }

            return false;
        }

        public List<GameObject> GetAvailableCollisions()
        {
            List<GameObject> availableCollisions = new List<GameObject>();

            // Check each neighboring hexagon
            CheckAndAddAvailable(top, availableCollisions);
            CheckAndAddAvailable(topleft, availableCollisions);
            CheckAndAddAvailable(topright, availableCollisions);
            CheckAndAddAvailable(bot, availableCollisions);
            CheckAndAddAvailable(botleft, availableCollisions);
            CheckAndAddAvailable(botright, availableCollisions);

            return availableCollisions;
        }
        private void CheckAndAddAvailable(GameObject obj, List<GameObject> availableCollisions)
        {
            // Ensure the object exists
            if (obj != null)
            {
                // Get the BoxCollider2D attached to the object
                BoxCollider2D boxCollider = obj.GetComponent<BoxCollider2D>();

                // Ensure the BoxCollider2D exists
                if (boxCollider != null)
                {
                    // Calculate the corners of the box collider
                    Vector2 min = boxCollider.bounds.min;
                    Vector2 max = boxCollider.bounds.max;

                    // Get all colliders in the area defined by the box collider's bounds
                    Collider2D[] colliders = Physics2D.OverlapAreaAll(min, max);

                    foreach (Collider2D col in colliders)
                    {
                        // Check if the colliding object has the tag "Hexagon"
                        if (col.CompareTag("Hexagon"))
                        {
                            // Get the HexagoneTextManager from the collided object
                            HexagoneTextManager hexManager = col.GetComponent<HexagoneTextManager>();

                            // Check if the hexagon is not occupied
                            if (hexManager != null && !hexManager.isHexOcupied && !hexManager.isLetterGuessed)
                            {
                                //  Debug.LogError("Added " + col.gameObject.name + " to the available collisions list.");
                                availableCollisions.Add(col.gameObject); // Add the collided object to the list
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError("The object does not have a BoxCollider2D component.");
                }
            }
        }

        public List<GameObject> GetAvailableCollisionsBot()
        {
            List<GameObject> availableCollisions = new List<GameObject>();

            // Check each neighboring hexagon
            CheckAndAddAvailableForBot(top, availableCollisions);
            CheckAndAddAvailableForBot(topleft, availableCollisions);
            CheckAndAddAvailableForBot(topright, availableCollisions);
            CheckAndAddAvailableForBot(bot, availableCollisions);
            CheckAndAddAvailableForBot(botleft, availableCollisions);
            CheckAndAddAvailableForBot(botright, availableCollisions);
            
            return availableCollisions;
        }

        public List<GameObject> GetNeighbours()
        {
            List<GameObject> availableCollisions = new List<GameObject>();

            // Check each neighboring hexagon
            CheckAndAddAvailableForBot(top, availableCollisions);
            CheckAndAddAvailableForBot(topleft, availableCollisions);
            CheckAndAddAvailableForBot(topright, availableCollisions);
            CheckAndAddAvailableForBot(bot, availableCollisions);
            CheckAndAddAvailableForBot(botleft, availableCollisions);
            CheckAndAddAvailableForBot(botright, availableCollisions);
           
            return availableCollisions;
        }

        private void CheckAndAddAvailableForBot(GameObject obj, List<GameObject> availableCollisions)
        {
            // Ensure the object exists
            if (obj != null)
            {
                // Get the BoxCollider2D attached to the object
                BoxCollider2D boxCollider = obj.GetComponent<BoxCollider2D>();

                // Ensure the BoxCollider2D exists
                if (boxCollider != null)
                {
                    // Calculate the corners of the box collider
                    Vector2 min = boxCollider.bounds.min;
                    Vector2 max = boxCollider.bounds.max;

                    // Get all colliders in the area defined by the box collider's bounds
                    Collider2D[] colliders = Physics2D.OverlapAreaAll(min, max);

                    foreach (Collider2D col in colliders)
                    {
                        // Check if the colliding object has the tag "Hexagon"
                        if (col.CompareTag("Hexagon"))
                        {
                            // Get the HexagoneTextManager from the collided object
                            HexagoneTextManager hexManager = col.GetComponent<HexagoneTextManager>();

                            // Check if the hexagon is not occupied
                            if (hexManager != null)
                            {
                                //  Debug.LogError("Added " + col.gameObject.name + " to the available collisions list.");
                                availableCollisions.Add(col.gameObject); // Add the collided object to the list
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError("The object does not have a BoxCollider2D component.");
                }
            }
        }


    }
}