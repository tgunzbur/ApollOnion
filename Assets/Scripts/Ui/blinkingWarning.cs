using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class blinkingWarning : MonoBehaviour {
    private float startingOpacity;
    private Image image;

    void Start() {
        image = GetComponent<Image>();
        startingOpacity = image.color.a;
        StartCoroutine(blinkingAnim());
    }

    private IEnumerator blinkingAnim() {
        float step = 0.01f;
        Color newColor;

        while (true) {
            while (image.color.a < 1) {
                newColor = image.color;
                newColor.a += step;
                image.color = newColor;
                yield return new WaitForSeconds(0.05f);
            }
            while (image.color.a > startingOpacity) {
                newColor = image.color;
                newColor.a -= step;
                image.color = newColor;
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
