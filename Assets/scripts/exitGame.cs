using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exitGame : MonoBehaviour
{
    public void OnExitButtonClick()
    {
        Debug.Log("Exit button clicked - Exiting game...");

        // Exit the game
        Application.Quit();

        // If you want to also stop play mode in the Unity Editor, use this line:
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
    }
}

