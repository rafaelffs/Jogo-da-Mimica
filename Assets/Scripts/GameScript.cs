using Assets.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    public List<string> selectedItems;
    [SerializeField] TMPro.TextMeshProUGUI txtSelectedItem;
    [SerializeField] TMPro.TextMeshProUGUI txtSelectedCategory;
    [SerializeField] TMPro.TextMeshProUGUI txtCorrectAnswersCount;
    [SerializeField] TMPro.TextMeshProUGUI txtJumpsLeftCount;
    [SerializeField] UnityEngine.UI.Button btnJump;
    [SerializeField] UnityEngine.UI.Button btnEndGame;
    [SerializeField] UnityEngine.UI.Button btnCorrectAnswer;
    [SerializeField] TMPro.TextMeshProUGUI txtTimeLeftCount;
    [SerializeField] GameObject timesUpImage;
    private bool timerIsRunning = false;
    private int correctAnswersCount = 1;
    private int jumpsLeft;
    private float timeRemaining;
    private bool soundHasPlayed;

    public List<Item> listItems { get; set; } = new List<Item>();

    private void Awake()
    {
        listItems = MainMenuScript.db.GetItemsByCategories(MainMenuScript.selectedCategories);
    }
    void Start()
    {
        StartGame();
    }
    void Update()
    {
        Timer();
    }
    private void Timer()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                txtTimeLeftCount.text = timeRemaining.ToString("f0") + " seconds";

                if (Convert.ToInt32(timeRemaining.ToString("f0")) == 5 && !soundHasPlayed)
                {
                    soundHasPlayed = true;
                    StartCoroutine(PlaySoundEvery(1, 5));
                }
            }
            else
            {
                EndGame();
            }
        }
    }
    private async void EndGame()
    {
        SoundManagerScript.playEndGameSound();
        EndGameScript.TotalTime = (MainMenuScript.settings.TotalTime * 60) - Convert.ToInt32(timeRemaining.ToString("f0"));
        timeRemaining = 0;
        timerIsRunning = false;
        btnJump.interactable = false;
        btnEndGame.enabled = false;
        btnCorrectAnswer.enabled = false;
        timesUpImage.SetActive(true);
        await Task.Delay(1500);
        LoadEndGamePage();
    }
    private void LoadEndGamePage()
    {
        SceneLoader.LoadNextScene();
    }
    private void StartGame()
    {
        EndGameScript.TotalTime = 0;
        btnJump.enabled = true;
        btnEndGame.enabled = true;
        btnCorrectAnswer.enabled = true;
        timesUpImage.SetActive(false);
        UpdateItem();
        LoadJumps();
        StartTimer();
    }
    private void StartTimer()
    {
        timeRemaining = MainMenuScript.settings.TotalTime * 60;
        timerIsRunning = true;
    }
    private void LoadJumps()
    {
        EndGameScript.TotalJumps = 0;
        txtJumpsLeftCount.text = (MainMenuScript.settings.MaximumJumps).ToString();
        jumpsLeft = MainMenuScript.settings.MaximumJumps;
        if (jumpsLeft == 0)
            DisableJumpButton();
    }
    private void UpdateJumps()
    {
        jumpsLeft--;
        txtJumpsLeftCount.text = jumpsLeft.ToString();
        if (jumpsLeft == 0)
            DisableJumpButton();
        EndGameScript.TotalJumps++;
    }
    private void DisableJumpButton()
    {
        btnJump.interactable = false;
    }
    public void UpdateItem()
    {
        if (MainMenuScript.selectedCategories != null && MainMenuScript.selectedCategories.Count > 0)
        {
            Item item = MainMenuScript.db.GetRandomItem(listItems, EndGameScript.CorrectAnswers);
            this.txtSelectedItem.text = item.Name;
            this.txtSelectedCategory.text = "Category: " + item.Category.FormattedName;
        }
    }
    public void CorrectAnswer_Click()
    {
        SoundManagerScript.playCorrectSound();
        txtCorrectAnswersCount.text = (correctAnswersCount++).ToString();
        EndGameScript.CorrectAnswers.Add(listItems.FirstOrDefault(x => x.Name.Equals(txtSelectedItem.text)));
        UpdateItem();
    }
    public void EndGame_Click()
    {
        EndGame();
    }
    public void Jump_Click()
    {
        SoundManagerScript.playJumpSound();
        UpdateItem();
        UpdateJumps();
    }
    IEnumerator PlaySoundEvery(float t, int times)
    {
        for (int i = 0; i < times; i++)
        {
            SoundManagerScript.playSecondSound();
            yield return new WaitForSeconds(t);
        }
    }
}
