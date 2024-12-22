using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordSearch
{
    public class BackGroundController : MonoBehaviour
    {
        [Range(-1f, 1f)]
        public float scrollSpeed = 0.5f;
        public float moveDistance = 0.5f; // Distance to move up and down
        private float offset;
        private Material mat;
        private Vector2 originalOffset;

        void Start()
        {
            mat = GetComponent<Renderer>().material;
            originalOffset = mat.GetTextureOffset("_MainTex");
        }

        void Update()
        {
            offset += (Time.deltaTime * scrollSpeed) / 10f;
            float verticalOffset = Mathf.PingPong(offset, moveDistance) - (moveDistance / 2); // Move up and down

            // Set the new texture offset
            mat.SetTextureOffset("_MainTex", new Vector2(originalOffset.x, originalOffset.y + verticalOffset));
        }
    }
}
