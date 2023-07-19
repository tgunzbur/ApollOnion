using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropMachineMenuUI : MonoBehaviour, IDragHandler, IPointerDownHandler {
    private Vector3 mouseOffset;
    private MachineUI toDrag;

    private void Start() {
        toDrag = transform.parent.GetComponent<MachineUI>();
    }

    public void OnPointerDown(PointerEventData data) {
        if (data.button == PointerEventData.InputButton.Left) {
            mouseOffset = toDrag.transform.position - Input.mousePosition;
        }
    }

    public void OnDrag(PointerEventData data) {
        toDrag.setPos(mouseOffset + Input.mousePosition);
    }
}
