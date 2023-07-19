using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : Machine {
    
    public override int getInputLimit() {
        return 8;
    }

    public override List<(MatType, int)> getInputList() {
        return new List<(MatType, int)> { (MatType.ONION_PEELING, 5) };
    }
    
    public override List<(MatType, int)> getOutputList() {
        return new List<(MatType, int)> { (MatType.ONION_GLASS, 1) };
    }

    public override int getPrice() {
        return 350;
    }

    public override string getName() {
        return "Furnace";
    }

    public override string getDescription() {
        return "Melts onion peeling into glass";
    }
}
