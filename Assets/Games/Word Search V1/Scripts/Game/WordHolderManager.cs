using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

namespace WordSearch
{
    public class WordHolderManager : MonoBehaviourPun
    {
        public TextMeshProUGUI word;
        public bool isWordGuesed;
        public Image image;
        // Start is called before the first frame update
        void Start()
        {
            word.text = "";
            isWordGuesed = false;
        }

        public void SetWord(string text)
        {
            isWordGuesed = false;
            word.text = text;
            Color currentColor = image.color;
            currentColor.a = 1f;
            image.color = currentColor;
        }

        public void SetWordGuessed()
        {
            isWordGuesed = true;

            // Apply strikethrough to the current word text
            word.text = "<s>" + word.text + "</s>";

            // Set the image opacity to 60%
            Color currentColor = image.color;  // Get the current color of the image
            currentColor.a = 0.6f;             // Set alpha to 0.6 (60% opacity)
            image.color = currentColor;        // Apply the updated color back to the image
        }


        public string GetWordText()
        {
            return word.text;
        }

    }
}