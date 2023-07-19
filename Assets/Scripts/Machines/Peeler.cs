using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peeler : Machine {
    
    public override int getInputLimit() {
        return 2;
    }

    public override List<(MatType, int)> getInputList() {
        return new List<(MatType, int)> { (MatType.ONION, 1) };
    }
    
    public override List<(MatType, int)> getOutputList() {
        return new List<(MatType, int)> { (MatType.ONION_PEELING, 1), (MatType.ONION_HEART, 1) };
    }

    public override int getPrice() {
        return 400;
    }

    public override string getName() {
        return "Peeler";
    }

    public override string getDescription() {
        return "Peels onions.";
    }
}