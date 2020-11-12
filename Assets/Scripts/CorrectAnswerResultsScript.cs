using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class CorrectAnswerResultsScript : MonoBehaviour
{
    public Text text;
    private string item;
    EndGameScript answerScrollList;

    public void Setup(string currentItem,  EndGameScript currentAnswerScrollList)
    {
        item = currentItem;
        text.text = item;
        answerScrollList = currentAnswerScrollList;
    }
}
