using Assets.Connection;
using Assets.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI txtMaximumJump;
    [SerializeField] TMPro.TextMeshProUGUI txtTotalTime;
    [SerializeField] UnityEngine.UI.Text warningText;
    [SerializeField] GameObject syncInProgress;
    [SerializeField] Component toggleList;
    public static List<Category> categoriesList { get; set; } = new List<Category>();
    public static List<Category> selectedCategories { get; set; }
    public static Settings settings { get; set; }
    private static SqliteDB _db;
    public static SqliteDB db { get { return _db; } }

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (_db != null && _db != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _db = new SqliteDB();
        }
    }
    private void Start()
    {
        LoadCategories();
    }

    private void LoadCategories()
    {
        GetCategoriesFromDB();
        if (categoriesList.Count <= 0)
            SyncCategories();
        LoadCategoriesInScreen();
    }

    public void PlayButton_click()
    {
        if (IsValid())
            SceneLoader.LoadLoadScreen();
    }

    private bool IsValid()
    {
        return IsValidCategories() && IsValidSettings();
    }

    private bool IsValidSettings()
    {
        int maximumJumps = Convert.ToInt32(txtMaximumJump.text.Replace("\u200B", ""));
        int totalTime = Convert.ToInt32(txtTotalTime.text.Replace("\u200B", ""));
        if (maximumJumps > 0 && totalTime > 0)
        {
            settings = new Settings()
            {
                MaximumJumps = maximumJumps,
                TotalTime = totalTime
            };
            return true;
        }
        else
        {
            warningText.text = "Arrume as configurações";
        }
        return false;
    }

    private bool IsValidCategories()
    {
        selectedCategories = new List<Category>();
        List<UnityEngine.UI.Toggle> toggleList = ((Toggle[])GameObject.FindObjectsOfType(typeof(Toggle))).Where(x => x.isOn).ToList();
        if (toggleList.Count() > 0)
        {
            LoadSelectedCategories(toggleList);
            return true;
        }
        else
            warningText.text = "Selecione ao menos uma categoria";
        return false;
    }

    private void LoadSelectedCategories(List<UnityEngine.UI.Toggle> toggleList)
    {
        foreach (UnityEngine.UI.Toggle item in toggleList)
        {
            Toggle toggle = item.GetComponent<Toggle>();
            UnityEngine.UI.Text textToggle = item.GetComponentInChildren<UnityEngine.UI.Text>();
            selectedCategories.Add(categoriesList.FirstOrDefault(x => x.FormattedName == textToggle.text));
        }
    }

    private async void SyncCategories()
    {
        syncInProgress.SetActive(true);
        await _db.Sync();
        LoadCategories();
        syncInProgress.SetActive(false);
    }

    private void GetCategoriesFromDB()
    {
        categoriesList = _db.GetCategories();
    }

    private void LoadCategoriesInScreen()
    {
        var toggles = toggleList.GetComponentsInChildren<Toggle>(true);
        int i = 0;
        foreach (var category in categoriesList)
        {
            toggles[i].gameObject.SetActive(true);
            toggles[i].isOn = false;
            UnityEngine.UI.Text textToggle = toggles[i].GetComponentInChildren<UnityEngine.UI.Text>();
            textToggle.text = category.FormattedName;
            i++;
        }
    }

    public void SyncCategories_Click()
    {
        SyncCategories();
        LoadCategoriesInScreen();
    }
}
