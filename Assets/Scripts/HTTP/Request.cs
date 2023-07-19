using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class Request {
    [System.Serializable]
    public class HighScore {
        public int id;
        public string pseudonym;
        public int score;

        public HighScore(string pseudonym, int score) {
            this.pseudonym = pseudonym;
            this.score = score;
        }
    }

    [System.Serializable]
    public class HighScores {
        public List<HighScore> scores;
    }

    public delegate void callBackDisplay(HighScores scores);

    public static IEnumerator addHighScoreTo(string url, string pseudonym, int score) {
        WWWForm form = new WWWForm();
        form.AddField("pseudonym", pseudonym);
        form.AddField("score", score);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            Debug.Log(www.downloadHandler.text);
            PlayerPrefs.SetString("playerRank", www.downloadHandler.text);
            PlayerPrefs.Save();
            Debug.Log("Added new score !");
        }
    }

    public static IEnumerator getHighScoresFrom(string url, callBackDisplay callback) {
        HighScores scores = null;
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            scores = JsonUtility.FromJson<HighScores>("{ \"scores\":" + www.downloadHandler.text + "}");
        }
        callback(scores);
    }
}