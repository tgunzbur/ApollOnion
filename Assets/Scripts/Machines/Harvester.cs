using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvester : Machine {

    public override int getInputLimit() {
        return 1;
    }

    public override List<(MatType, int)> getInputList() {
        return new List<(MatType, int)>();
    }
    
    public override List<(MatType, int)> getOutputList() {
        return new List<(MatType, int)> { (MatType.ONION, 1) };
    }

    public override int getPrice() {
        return 1000;
    }

    public override string getName() {
        return "Harvester";
    }

    public override string getDescription() {
        return "Harvests onions.";
    }
}