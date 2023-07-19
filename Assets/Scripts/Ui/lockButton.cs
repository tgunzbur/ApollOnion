using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lockButton : MonoBehaviour {

    public Sprite locked;
    public Sprite unlocked;
    
    private MachineUI script;
    private Image image;

    private void Start() {
        script = transform.parent.GetComponent<MachineUI>();
        image = GetComponent<Image>();
        changeSprite();
    }

    public void changeLockState() {
        script.setLockState();
        changeSprite();
    }

    private void changeSprite() {
        if (script.getLockState()) {
            image.sprite = locked;
        } else {
            image.sprite = unlocked;
        }
    }
}
