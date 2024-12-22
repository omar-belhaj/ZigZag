using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityToolbarExtender;

static class ToolbarStyles
{
    public static readonly GUIStyle commandButtonStyle;

    static ToolbarStyles()
    {
        commandButtonStyle = new GUIStyle("Command")
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageAbove,
            fontStyle = FontStyle.Bold
        };
    }
}

[InitializeOnLoad]
public class SceneSwitchLeftButton
{
    //0 => bot
    //1 => player 1
    //2 => player 2
    private static int startIdentity = -1;

    static SceneSwitchLeftButton()
    {
        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
    }

    static void OnToolbarGUI()
    {
        GUILayout.FlexibleSpace();

        Texture2D player1Icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Player1.png");
        Texture2D player2Icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Player2.png");
        Texture2D botIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Bot.png");
        GUI.backgroundColor = new Color(0.78f, 0.78f, 0.78f, 1);

        if (EditorApplication.isPlaying)
            return;

        if (GUILayout.Button(new GUIContent(player1Icon, "Start as Player 1"), ToolbarStyles.commandButtonStyle))
        {
            Debug.Log("Start as player 1()");
            StartPlayMode(1);
        }
        if (GUILayout.Button(new GUIContent(player2Icon, "Start as Player 2"), ToolbarStyles.commandButtonStyle))
        {
            Debug.Log("Start as player 2()");
            StartPlayMode(2);
        }
        if (GUILayout.Button(new GUIContent(botIcon, "Play Against Bot"), ToolbarStyles.commandButtonStyle))
        {
            Debug.Log("Play against Bot()");
            StartPlayMode(0);
        }
    }

    private static void StartPlayMode(int identity)
    {
        if (!EditorApplication.isPlaying)
            EditorApplication.isPlaying = true;

        startIdentity = identity;
        EditorApplication.update += InitiateGameParams;
    }

    private static void InitiateGameParams()
    {
        if (GameEntryPoint.I == null)
            return;
        EditorApplication.update -= InitiateGameParams;
        if (startIdentity == 0)
            GameEntryPoint.I.StartAgainstBot();
        else if (startIdentity == 1)
            GameEntryPoint.I.StartAsPlayer1();
        else if (startIdentity == 2)
            GameEntryPoint.I.StartAsPlayer2();
    }
}
