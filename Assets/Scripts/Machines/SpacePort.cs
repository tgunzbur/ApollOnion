using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacePort : Machine {
    [HideInInspector]
    public bool isLaunchable = false;
    public GameObject rocket = null;

    private void Update() {
        if (!rocket) {
            foreach (Mat mat in inputs) {
                if (mat.GetComponent<Rocket>()) {
                    addRocket();
                    break;
                }
            }
        }
    }

    public void addRocket() {
        rocket = Instantiate(GameManager.GetMatPrefab(MatType.ROCKET).GetComponent<DragAndDropMat>().matPrefab, transform.position + Vector3.up * 0.6f, Quaternion.identity, transform);
        rocket.SetActive(true);
        rocket.transform.localScale = Vector3.one * 0.5f;
    }

    public override List<Mat> execute() {
        if (!rocket) {
            addRocket();
        }
        isLaunchable = true;
        return new List<Mat>();
    }

    public override int getInputLimit() {
        return 50;
    }

    public override List<(MatType, int)> getInputList() {
        return new List<(MatType, int)> { (MatType.ROCKET, 1), (MatType.ONION_FUEL, 33) };
    }
    
    public override List<(MatType, int)> getOutputList() {
        return new List<(MatType, int)> { };
    }

    public override int getPrice() {
        return 2500;
    }

    public override string getName() {
        return "SpacePort";
    }

    public override string getDescription() {
        return "Launches Rocket !";
    }
}