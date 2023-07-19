using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropMachine : MonoBehaviour {
    public bool canDrag = true;

    private Vector3 mouseOffset;
    private Camera  mainCamera;
    private Machine machine;

    private float clickTime;

    private void Start() {
        mainCamera = Camera.main;
        machine = GetComponent<Machine>();
        clickTime = -1;
    }

    public void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            clickTime = Time.time;
        }
        if (Time.time - clickTime < 0.25f && Input.GetMouseButtonUp(0)) {
            GameManager.machineUi.displayUiForMachine(machine);
            mouseOffset = transform.position - Input.mousePosition;
        }
    }

    public void OnMouseDrag() {
        if (!canDrag || Time.time - clickTime < 0.25f || EventSystem.current.IsPointerOverGameObject() || GameManager.machineUi.gameObject.activeSelf) {
            return;
        }
        GetComponent<Machine>().removeWarning();

        int layerMask = 1 << LayerMask.NameToLayer("Tile");
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, layerMask)) {
            Tile tile = hit.transform.gameObject.GetComponent<Tile>();
            if (tile.Machine == null && machine.canBeBuiltOn(tile.Type)) {
                transform.position = hit.transform.position + new Vector3(0, hit.transform.localScale.y / 2, 0);
                machine.Tile.Machine = null;
                machine.Tile = hit.transform.GetComponent<Tile>();
                hit.transform.GetComponent<Tile>().Machine = machine;
                GameManager.machineUi.hideUi();
            }
        }
    }
}
