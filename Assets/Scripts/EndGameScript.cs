using Assets.Models;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameScript : MonoBehaviour
{
    public List<string> selectedItems;
    [SerializeField] TMPro.TextMeshProUGUI txtCorrectAnswersCount;
    [SerializeField] TMPro.TextMeshProUGUI txtTotalJumpsCount;
    [SerializeField] TMPro.TextMeshProUGUI txtTimeTotalCount;

    public Transform contentPanel;
    public CorrectAnswerPoolScript correctAnswerPoolScript;

    public static int TotalJumps { get; set; }
    public static List<Item> CorrectAnswers { get; } = new List<Item>();
    public static int TotalTime { get; set; }

    void Start()
    {
        EndGameResults();
    }
    private void EndGameResults()
    {
        txtCorrectAnswersCount.text = CorrectAnswers.Count.ToString();
        txtTotalJumpsCount.text = TotalJumps.ToString();
        txtTimeTotalCount.text = TotalTime.ToString() + " segundo(s)";
        AddCorrectAnswers();
    }

    public void AddCorrectAnswers()
    {
        for (int i = 0; i < CorrectAnswers.Count; i++)
        {
            GameObject newCorrectAnswer = correctAnswerPoolScript.GetObject();
            newCorrectAnswer.transform.SetParent(contentPanel);
            CorrectAnswerResultsScript sampleCorrectAnswerResultsScript = newCorrectAnswer.GetComponent<CorrectAnswerResultsScript>();
            sampleCorrectAnswerResultsScript.transform.localScale = new Vector3(1, 1, 1);
            sampleCorrectAnswerResultsScript.text.fontSize = 35;
            sampleCorrectAnswerResultsScript.Setup("- " + CorrectAnswers[i].Name, this);
        }
    }

    public void MainMenu_Click()
    {
        ClearScore();
        SceneLoader.LoadStartScene();
    }

    public void Restart_Click()
    {
        ClearScore();
        SceneLoader.LoadLoadScreen();
    }

    private void ClearScore()
    {
        CorrectAnswers.Clear();
        TotalJumps = 0;
    }
}
