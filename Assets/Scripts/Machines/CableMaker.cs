using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableMaker : Machine {
    
    public override int getInputLimit() {
        return 6;
    }

    public override List<(MatType, int)> getInputList() {
        return new List<(MatType, int)> { (MatType.ONION_FIBER, 3) };
    }
    
    public override List<(MatType, int)> getOutputList() {
        return new List<(MatType, int)> { (MatType.ONION_CABLE, 1) };
    }

    public override int getPrice() {
        return 600;
    }

    public override string getName() {
        return "Cable Maker";
    }

    public override string getDescription() {
        return "Braids fibers in cable.";
    }
}
