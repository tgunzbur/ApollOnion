using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropMachineUi : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler {
    public GameObject machinePrefab;

    private Vector3 startingPosition;
    private Vector3 mouseOffset;

    private GameObject previewObject;
    private GameObject tileOn;
    private bool        buyable;

    private Camera   mainCamera;
    private Image    image;
    private Machine  machine;

    private bool dragging;

    private void Start() {
        buyable = true;
        startingPosition = transform.localPosition;
        previewObject = null;
        tileOn = null;

        dragging = false;

        mainCamera = Camera.main;
        image = GetComponent<Image>();
        machine = machinePrefab.GetComponent<Machine>();
    }

    private void Update() {
        handleAvailability();
    }

    private bool isBuyable() {
        return (GameManager.Infos.Money >= machine.getPrice());
    }

    private void handleAvailability() {
        if (isBuyable() != buyable) {
            buyable = isBuyable();
            if (buyable) {
                image.color = Color.white;
            } else {
                image.color = Color.gray;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            startingPosition = transform.localPosition;
            mouseOffset = transform.position - Input.mousePosition;
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (!buyable) {
            return;
        }
        transform.position = mouseOffset + Input.mousePosition;
        dragging = true;

        int layerMask = 1 << LayerMask.NameToLayer("Tile");
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, layerMask)) {
            Tile tile = hit.transform.gameObject.GetComponent<Tile>();
            if (tile.Machine == null && machinePrefab.GetComponent<Machine>().canBeBuiltOn(tile.Type)) {
                if (!previewObject) {
                    previewObject = Instantiate(machinePrefab, hit.transform.position + new Vector3(0, machinePrefab.transform.localScale.y / 2, 0), machinePrefab.transform.rotation, GameManager.machinesContainer);
                    previewObject.GetComponent<Machine>().enabled = false;
                    if (previewObject.GetComponent<DragAndDropMachine>()) {
                        previewObject.GetComponent<DragAndDropMachine>().enabled = false;
                    }
                } else {
                    previewObject.SetActive(true);
                    image.color = Color.clear;
                    previewObject.transform.position = hit.transform.position + new Vector3(0, hit.transform.localScale.y / 2, 0);
                }
                tileOn = hit.transform.gameObject;
            }
            else {
                if (previewObject) {
                    previewObject.SetActive(false);
                }
                image.color = Color.red;
                tileOn = null;
            }
        }
        else {
            if (previewObject) {
                previewObject.SetActive(false);
            }
            image.color = Color.white;
            tileOn = null;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!buyable) {
            return;
        }
        image.color = Color.white;
        transform.localPosition = startingPosition;
        dragging = false;
        if (previewObject) {
            if (tileOn) {
                previewObject.GetComponent<Machine>().enabled = true;
                previewObject.GetComponent<DragAndDropMachine>().enabled = true;

                Machine placedMachine = previewObject.GetComponent<Machine>();
                Tile    placedTile = tileOn.GetComponent<Tile>();
                placedTile.Machine = placedMachine;
                placedMachine.Tile = placedTile;
                previewObject = null;
                GameManager.Infos.Money -= placedMachine.getPrice();
            }
            else {
                previewObject.SetActive(false);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (dragging) {
            return ;
        }
        GameManager.machineInfosUi.showInfos(gameObject, machine);

    }

    public void OnPointerExit(PointerEventData eventData) {
        GameManager.machineInfosUi.hideUi();
    }
}