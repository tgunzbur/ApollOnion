using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewingMachine : Machine {
    
    public override int getInputLimit() {
        return 10;
    }

    public override List<(MatType, int)> getInputList() {
        return new List<(MatType, int)> { (MatType.ONION_FABRIC, 2) , (MatType.ONION_FIBER, 3) };
    }
    
    public override List<(MatType, int)> getOutputList() {
        return new List<(MatType, int)> { (MatType.CLOTHE, 1) };
    }

    public override int getPrice() {
        return 600;
    }

    public override string getName() {
        return "Sewing Machine";
    }

    public override string getDescription() {
        return "Sews fabric in clothes.";
    }
}
