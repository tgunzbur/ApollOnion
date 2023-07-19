using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class endSceneManager : MonoBehaviour {
    public float speed;
    public GameObject background;
    public GameObject recap;
    public GameObject recapCongrat;
    public GameObject recapTime;
    public GameObject recapPlace;
    public GameObject cloudPrefab;
    public GameObject starPrefab;
    public GameObject rocket;

    void Start() {
        playAnimation();
    }

    void Update() {
        if (background.transform.position.y < -50 && background.transform.position.y > -350) {
            recap.transform.position += Vector3.up * Time.deltaTime * speed;
            if (recapCongrat.GetComponent<RectTransform>().localPosition.x < -9) {
                recapCongrat.GetComponent<RectTransform>().localPosition += Vector3.right * Time.deltaTime * speed;
            }
            if (background.transform.position.y < -100 && recapTime.GetComponent<RectTransform>().localPosition.x < -8) {
                recapTime.GetComponent<RectTransform>().localPosition += Vector3.right * Time.deltaTime * speed;
            }
            if (background.transform.position.y < -150 && recapPlace.GetComponent<RectTransform>().localPosition.x < -8) {
                recapPlace.GetComponent<RectTransform>().localPosition += Vector3.right * Time.deltaTime * speed;
            }
        }
        if (background.transform.position.y < -400 && background.transform.position.y > -450) {
            float newSpeed = speed * (background.transform.position.y + 450) / 50;
            background.transform.position += -Vector3.up * Time.deltaTime * (newSpeed > speed / 3 ? newSpeed : speed / 3);
            rocket.transform.position += Vector3.up * Time.deltaTime * (speed - (newSpeed > speed / 3 ? newSpeed : speed / 3));
        } else if (background.transform.position.y > -450) {
            background.transform.position += -Vector3.up * Time.deltaTime * speed;
        } else if (background.transform.position.y > -480) {
            background.transform.position += -Vector3.up * Time.deltaTime * speed / 5;
        } else {
            rocket.transform.position -= Vector3.up * Time.deltaTime * speed / 4;
            rocket.transform.rotation = Quaternion.Euler(0, 0, -75);
            if (Input.GetKeyDown(KeyCode.Space)) {
                SceneManager.LoadScene("MainScene");
            }
        }
    }

    private string AddOrdinal(string num) {
        int value = 0;
        int.TryParse(num, out value);

        switch (value % 100) {
            case 11:
            case 12:
            case 13:
                return num + "th";
        }

        switch (value % 10) {
            case 1:
                return num + "st";
            case 2:
                return num + "nd";
            case 3:
                return num + "rd";
            default:
                return num + "th";
        }
    }

    void playAnimation() {
        recapCongrat.GetComponent<Text>().text = "Congratulations " + PlayerPrefs.GetString("playerName", "None") + "!";

        float result = 0;
        float.TryParse(PlayerPrefs.GetString("playerTime", "0"), out result);
        recapTime.GetComponent<Text>().text = "You have finished the game in " + (Mathf.Round(result * 1000) / 1000).ToString() + "s";
        string rank = AddOrdinal(PlayerPrefs.GetString("playerRank", "Unknown").Trim());
        recapPlace.GetComponent<Text>().text = "You take the " + rank + " position in the leaderboard";
        for (int count = 0; count < 30; count++) {
            Vector3 pos = Vector3.zero;
            pos.x = Random.Range(-10, 10);
            pos.y = Random.Range(50, 350);
            pos.z = Random.Range(-3, 2);
            Quaternion rot = cloudPrefab.transform.rotation;
            GameObject newCloud = Instantiate(cloudPrefab, pos, rot, background.transform);
            float randomSize = Random.Range(2f, 2.5f);
            newCloud.transform.localScale = new Vector3(Random.Range(4, 6), randomSize, randomSize / 2);
        }

        for (int count = 0; count < 450; count++) {
            Vector3 pos = Vector3.zero;
            pos.x = Random.Range(-15, 15);
            pos.y = Random.Range(700, 1000);
            pos.z = Random.Range(2, 0.1f);
            Quaternion rot = cloudPrefab.transform.rotation;
            GameObject newCloud = Instantiate(starPrefab, pos, rot, background.transform);
            float randomSize = Random.Range(0.02f, 0.2f);
            newCloud.transform.localScale = new Vector3(randomSize, randomSize, randomSize);
        }
    }
}
