using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (SceneManager.sceneCountInBuildSettings < currentSceneIndex + 1)
            LoadStartScene();
        else
            SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public static void LoadStartScene()
    {
        SceneManager.LoadScene(0);
    }

    public static void LoadScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }

    public static void LoadLoadScreen()
    {
        SceneManager.LoadScene(3);
    }

}
