using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loom : Machine {
    
    public override int getInputLimit() {
        return 6;
    }

    public override List<(MatType, int)> getInputList() {
        return new List<(MatType, int)> { (MatType.ONION_FIBER, 3) };
    }
    
    public override List<(MatType, int)> getOutputList() {
        return new List<(MatType, int)> { (MatType.ONION_FABRIC, 1) };
    }

    public override int getPrice() {
        return 450;
    }

    public override string getName() {
        return "Loom";
    }

    public override string getDescription() {
        return "Weaves fibers into fabric.";
    }
}
