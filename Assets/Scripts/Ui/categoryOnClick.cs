using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class categoryOnClick : MonoBehaviour, IPointerDownHandler {
    public GameObject[] machines;

    private footerMenuScript script;

    private void Start() {
        script = transform.parent.parent.parent.GetComponent<footerMenuScript>();
    }

    public void OnPointerDown(PointerEventData clickData) {
        script.OnClickCategory(name, machines);
    }
}
