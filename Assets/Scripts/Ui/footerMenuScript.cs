using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class footerMenuScript : MonoBehaviour {
    public GameObject machinesContainer;
    
    private string currentCategory;

    public void OnClickCategory(string name, GameObject[] machines) {
        if (currentCategory == name) {
            currentCategory = "";
            foreach (Transform machine in machinesContainer.transform) {
                if (!machine.gameObject.activeSelf) {
                    machine.gameObject.SetActive(true);
                }
            }
        } else {
            currentCategory = name;
            foreach (Transform machine in machinesContainer.transform) {
                machine.gameObject.SetActive(false);
            }
            foreach (GameObject machine in machines) {
                machine.gameObject.SetActive(true);
            }
        }
    }
}
