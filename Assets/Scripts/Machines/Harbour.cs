using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harbour : Machine {
    public GameObject coinPrefab;

    public Dictionary<MatType, int> prices = new Dictionary<MatType, int>() {
        [MatType.NONE] = 0,
        [MatType.ONION] = 1,
        [MatType.ONION_HEART] = 2,
        [MatType.ONION_PEELING] = 2,
        [MatType.ONION_PLATE] = 4,
        [MatType.ONION_JUICE] = 5,
        [MatType.ONION_FUEL] = 50,
        [MatType.ONION_PASTE] = 5,
        [MatType.ONION_FIBER] = 10,
        [MatType.ONION_CABLE] = 30,
        [MatType.ONION_GLASS] = 40,
        [MatType.ONION_FABRIC] = 80,
        [MatType.TANK] = 300,
        [MatType.COCKPIT] = 300,
        [MatType.ENGINE] = 300,
        [MatType.ROCKET] = 10000,
        [MatType.CLOTHE] = 500,
    };
    
    public override int getInputLimit() {
        return 6;
    }

    public override List<(MatType, int)> getInputList() {
        return new List<(MatType, int)> { };
    }
    
    public override List<(MatType, int)> getOutputList() {
        return new List<(MatType, int)> { };
    }

    public override int getPrice() {
        return 1100;
    }

    public override string getName() {
        return "Harbour";
    }

    public override string getDescription() {
        return "Sells anything against coins. Can only be built on sand.";
    }

    public override bool canBeBuiltOn(EnvironmentType type) {
        return type == EnvironmentType.SAND;
    }
    
    public override List<Mat> execute() {
        if (inputs.Count > 0) {
            Instantiate(coinPrefab, transform.position, coinPrefab.transform.rotation, GameManager.matsContainer);
        }
        foreach (Mat mat in inputs) {
            GameManager.Infos.Money += prices[mat.getMatType()];
            Destroy(mat.gameObject);
        }
        inputs.Clear();
        return new List<Mat>();
    }
}