using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using System.Net;

public class mainMenuHandler : MonoBehaviour {
    public GameObject mainMenu;
    public GameObject gameMenu;
    public GameObject optionsMenu;
    public GameObject savesMenu;
    public GameObject newSaveMenu;
    public GameObject leaderBoardMenu;

    public GameObject game;

    public GameObject previewPrefab;
    public GameObject saveViewPort;
    public GameObject previewSave;
    public GameObject saveNameInput;
    public GameObject alertSaveText;

    public GameObject scorePrefab;
    public GameObject scoresViewPort;
    public GameObject searchScoreInput;
    public GameObject loadingScoreFrame;

    public AudioSource mainAudioSrc;
    public AudioSource musicAudioSrc; 
    public GameObject mainAudioContainer;
    public GameObject musicAudioContainer;

    private Image prevSearch;

    private void Start() {
        prevSearch = null;

        setMusicVolume(PlayerPrefs.GetInt("musicVolume", 10));
        goToMainMenu();
    }

// MENUS BUTTONS

    public void goToMainMenu() {
        game.SetActive(false);
        gameMenu.SetActive(false);
        optionsMenu.SetActive(false);
        leaderBoardMenu.SetActive(false);

        mainMenu.SetActive(true);
    }

    public void goToLeaderBoardMenu() {
        mainMenu.SetActive(false);

        leaderBoardMenu.SetActive(true);
        getHighScores();
    }

    public void goToGameMenu() {
        newSaveMenu.SetActive(false);
        mainMenu.SetActive(false);
        savesMenu.SetActive(false);

        gameMenu.SetActive(true);
    }

    public void goToOptionsMenu() {
        mainMenu.SetActive(false);

        optionsMenu.SetActive(true);
    }

    public void quitGame() {
        Application.Quit();
    }

// LEADERBOARD MENU OPTIONS

    private void displayHighScores(Request.HighScores scores) {
        foreach (Transform child in scoresViewPort.transform) {
            GameObject.Destroy(child.gameObject);
        }
        if (scores != null) {
            int count = 1;
            foreach (Request.HighScore score in scores.scores) {
                GameObject newScore = Instantiate(scorePrefab, Vector3.zero, Quaternion.identity, scoresViewPort.transform);

                newScore.name = score.pseudonym;
                newScore.transform.Find("pseudonym").GetComponent<Text>().text = score.pseudonym;
                newScore.transform.Find("score").GetComponent<Text>().text = (score.score / 1000f).ToString("F3") + "s";
                newScore.transform.Find("rank").GetComponent<Text>().text = "#" + count.ToString();
                count++;
            }
        }

        loadingHighScore(false);
    }

    private void goToPseudonym(string pseudonym) {
        Transform child = null;
        if ((child = scoresViewPort.transform.Find(pseudonym)) != null) {
            if (prevSearch) {
                prevSearch.color = new Color(221 / 255f, 221 / 255f, 221 / 255f);
            }
            prevSearch = child.GetComponent<Image>();
            prevSearch.color = new Color(116 / 255f, 164 / 255f, 209 / 255f);
            scoresViewPort.transform.localPosition = new Vector3(scoresViewPort.transform.localPosition.x, -(child.transform.localPosition.y + 50), scoresViewPort.transform.localPosition.z);
        }
    }

    private void loadingHighScore(bool state) {
        loadingScoreFrame.SetActive(state);
    }

    public void getHighScores() {
        loadingHighScore(true);
        StartCoroutine(Request.getHighScoresFrom(GameManager.websiteUrl + "/high-score", displayHighScores));
    }

    private void getHighScoresWithPseudonym(string pseudonym) {
        loadingHighScore(true);
        StartCoroutine(Request.getHighScoresFrom(GameManager.websiteUrl +  "/high-score?pseudonym=" + pseudonym, displayHighScores));
    }

    public void addHighScore(string pseudonym, int score) {
        StartCoroutine(Request.addHighScoreTo(GameManager.websiteUrl + "/high-score", pseudonym, score));
    }

    public void searchHighScore() {
        string pseudonym = searchScoreInput.GetComponent<InputField>().text;
        
        if (pseudonym == "") {
            searchScoreInput.GetComponent<InputField>().text = "Pseudonym";
            return;
        }

        goToPseudonym(pseudonym);
    }

// GAME MENU BUTTONS

    public void goToCreateGameMenu() {
        gameMenu.SetActive(false);
        newSaveMenu.SetActive(true);

        saveNameInput.GetComponent<InputField>().text = "New Game";
        alertSaveText.GetComponent<Text>().text = "";
    }

    public void createNewGame() {
        string saveName = saveNameInput.GetComponent<InputField>().text;

        if (saveName == "") {
            alertSaveText.GetComponent<Text>().text = "Save name can't be blank";
            return ;
        }
        if (File.Exists(Path.Combine(GameManager.savesFolder, saveName + ".save"))) {
            alertSaveText.GetComponent<Text>().text = "Save with this name already exist";
            return ;
        }
        newSaveMenu.SetActive(false);
        game.SetActive(true);

        GameManager.startNewGame(saveName);
    }

    public void goToGameList() {
        gameMenu.SetActive(false);

        foreach (Transform child in saveViewPort.transform) {
            GameObject.Destroy(child.gameObject);
        }

        foreach (string fileName in GameManager.loadExistingSaves()) {
            string name = Path.GetFileName(fileName).Split('.')[0];
            string lastEdited = File.GetLastWriteTime(fileName).ToShortDateString() + "\n" +
                                File.GetLastWriteTime(fileName).ToLongTimeString();
            string creationDate = File.GetCreationTime(fileName).ToShortDateString() + "\n" +
                                File.GetCreationTime(fileName).ToLongTimeString();
            Sprite preview;
            if (File.Exists(Path.Combine(GameManager.savesPreviewFolder, name + ".png"))) {
                preview = LoadSprite(Path.Combine(GameManager.savesPreviewFolder, name + ".png"));
            } else {
                preview = null;
            }
            createSaveButton(name, lastEdited, creationDate, preview);
        }
        previewSave.SetActive(false);
        savesMenu.SetActive(true);
    }

    private GameObject createSaveButton(string name, string lastEdited, string creationDate, Sprite preview) {
        GameObject newSave = Instantiate(previewPrefab, Vector3.zero, Quaternion.identity, saveViewPort.transform);

        newSave.name = name;
        newSave.GetComponent<RectTransform>().anchoredPosition = previewPrefab.GetComponent<RectTransform>().anchoredPosition;
        newSave.transform.Find("previewImage").GetComponent<Image>().sprite = preview;
        newSave.transform.Find("saveName").GetComponent<Text>().text = name;
        newSave.transform.Find("saveDate").GetComponent<Text>().text = lastEdited;
        newSave.GetComponent<Button>().onClick.RemoveAllListeners();
        newSave.GetComponent<Button>().onClick.AddListener(() => onClickSavePreview(name, lastEdited, creationDate, preview));
    
        return newSave;
    }

    private Sprite LoadSprite(string path) {
        if (File.Exists(path)) {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }
        return null;
    }

// GAME LIST BUTTONS

    public void loadGame(string name) {
        savesMenu.SetActive(false);
        game.SetActive(true);

        GameManager.loadGame(name);
    }

    public void deleteSave(string name) {
        previewSave.SetActive(false);
        if (File.Exists(Path.Combine(GameManager.savesFolder, name + ".save"))) {
            File.Delete(Path.Combine(GameManager.savesFolder, name + ".save"));
        }
        if (File.Exists(Path.Combine(GameManager.savesPreviewFolder, name + ".png"))) {
            File.Delete(Path.Combine(GameManager.savesPreviewFolder, name + ".png"));
        }
        GameObject.Destroy(saveViewPort.transform.Find(name).gameObject);
    }

    private void onClickSavePreview(string name, string lastEdited, string creationDate, Sprite preview) {
        previewSave.SetActive(true);
        previewSave.transform.Find("previewImage").GetComponent<Image>().sprite = preview;
        previewSave.transform.Find("saveName").GetComponent<Text>().text = name;
        previewSave.transform.Find("lastEditedText").GetComponent<Text>().text = lastEdited;
        previewSave.transform.Find("creationDateText").GetComponent<Text>().text = creationDate;
        previewSave.transform.Find("loadButton").GetComponent<Button>().onClick.RemoveAllListeners();
        previewSave.transform.Find("loadButton").GetComponent<Button>().onClick.AddListener(() => loadGame(name));
        previewSave.transform.Find("deleteButton").GetComponent<Button>().onClick.RemoveAllListeners();
        previewSave.transform.Find("deleteButton").GetComponent<Button>().onClick.AddListener(() => deleteSave(name));
    }

// OPTIONS MENU BUTTONS

    public void setMusicVolume(int value) {
        musicAudioSrc.volume = value / 10f;
        Color color = Color.cyan;
        
        if (value == 0) {
            color = Color.red;
        }
        foreach (Transform child in musicAudioContainer.transform) {
            child.GetChild(0).GetComponent<Image>().color = color;
            if (value.ToString() == child.name) {
                color = Color.white;
            }
        }

        PlayerPrefs.SetInt("musicVolume", value);
    }

    public void onOptionsExit() {
        PlayerPrefs.Save();
    }
}
