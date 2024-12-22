using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Buttonscreensmanager : MonoBehaviour
{
   public void OnbuttonPress()
    {
        SceneManager.LoadScene("ThemeChoice");
    }
    public void Onbuttongotomath()
    {
        SceneManager.LoadScene("quizmath");
    }
    public void Onbuttongotowordsearch()
    {
        SceneManager.LoadScene("Word Search");
    }
    public void Onbuttongotochatbot()
    {
        SceneManager.LoadScene("chatbot");
    }
}
