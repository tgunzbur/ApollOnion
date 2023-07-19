using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMoney : MonoBehaviour {
    private Text        text;

    private void Start() {
        text = GetComponent<Text>();
    }

    private void Update() {
        UpdateMoney();
    }

    private void UpdateMoney() {
        text.text = GameManager.Infos.Money.ToString();
    }
}
