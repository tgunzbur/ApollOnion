using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MachineInfoUi : MonoBehaviour {
    public GameObject itemIconPrfab;

    private Transform outInContainer;
    private Transform inputCol;
    private Transform outputCol;

    private void Start() {
        GameManager.machineInfosUi = this;

        outInContainer = transform.Find("outInContainer");
        inputCol = outInContainer.Find("inputCol");
        outputCol = outInContainer.Find("outputCol");

        gameObject.SetActive(false);
    }

    public void showInfos(GameObject icon, Machine machine) {
        Vector2 newPos = transform.position;
        Vector2 actualSize = GetComponent<RectTransform>().sizeDelta * transform.lossyScale;
        Vector2 iconPos = icon.transform.position;
        newPos.x = iconPos.x;
        if (newPos.x - actualSize.x / 2 < 0) {
            newPos.x = actualSize.x / 2;
        } else if (newPos.x + actualSize.x / 2 > Screen.width) {
            newPos.x = Screen.width - actualSize.x / 2;
        }
        transform.position = newPos;
        SetOutInItems(machine);
        gameObject.SetActive(true);
    }

    private void SetOutInItems(Machine machine) {
        ClearContainer(inputCol);
        ClearContainer(outputCol);
        foreach ((MatType mat, int number) in machine.getInputList()) {
            AddNewIconToCol(mat, number, inputCol);
        }

        foreach ((MatType mat, int number) in machine.getOutputList()) {
            AddNewIconToCol(mat, number, outputCol);
        }
    }

    private void AddNewIconToCol(MatType mat, int number, Transform col) {
        GameObject matIcon = Instantiate(itemIconPrfab, col);
        matIcon.GetComponentInChildren<Image>().sprite = GameManager.GetMatImage(mat);
        matIcon.GetComponentInChildren<Text>().text = number.ToString();
    }

    private void ClearContainer(Transform container) {
        foreach (Transform child in container) {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void hideUi() {
        gameObject.SetActive(false);
    }
}
