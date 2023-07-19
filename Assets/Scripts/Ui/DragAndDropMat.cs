using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropMat : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    public GameObject matPrefab;

    public static MatType dragging;

    private Vector3 startingPosition;
    private Vector3 mouseOffset;

    private Image   image;
    private MatType matType;

    private void Start() {
        image = GetComponent<Image>();
        matType = matPrefab.GetComponent<Mat>().getMatType();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        image.raycastTarget = false;
        startingPosition = transform.localPosition;
        mouseOffset = transform.position - Input.mousePosition;
        dragging = matType;
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = mouseOffset + Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        image.raycastTarget = true;
        transform.localPosition = startingPosition;
        dragging = MatType.NONE;
    }
}
