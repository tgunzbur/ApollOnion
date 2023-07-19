using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMachineInfos : MonoBehaviour {
    private Machine machine;
    private Text title;
    private Text price;

    void Start() {
        machine = GetComponentInChildren<DragAndDropMachineUi>().machinePrefab.GetComponent<Machine>();
        Text[] texts = GetComponentsInChildren<Text>();
        texts[0].text = machine.getName();
        texts[1].text = machine.getPrice().ToString();
    }
}
