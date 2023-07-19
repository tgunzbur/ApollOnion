using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compacter : Machine {
    
    public override int getInputLimit() {
        return 4;
    }

    public override List<(MatType, int)> getInputList() {
        return new List<(MatType, int)> { (MatType.ONION_HEART, 2) };
    }
    
    public override List<(MatType, int)> getOutputList() {
        return new List<(MatType, int)> { (MatType.ONION_PLATE, 1) };
    }

    public override int getPrice() {
        return 500;
    }

    public override string getName() {
        return "Compacter";
    }

    public override string getDescription() {
        return "Compacts onions in plate.";
    }
}
