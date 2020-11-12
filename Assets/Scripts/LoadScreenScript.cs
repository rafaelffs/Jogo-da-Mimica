using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreenScript : MonoBehaviour
{
    [SerializeField] Text countDownText;
    void Start()
    {
        StartCoroutine(Countdown(4));
    }

    IEnumerator Countdown(int seconds)
    {
        int count = seconds;

        while (count > 0)
        {
            yield return new WaitForSeconds(1);
            if (count != 1)
            {
                countDownText.text = (count - 1).ToString();
                SoundManagerScript.playSecondSound();
            }
            count--;
        }
        StartGame();
    }
    async void StartGame()
    {
        SoundManagerScript.playEndSound();
        await Task.Delay(100);
        SceneLoader.LoadScene(1);
    }

}
