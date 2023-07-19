using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineFactory : Machine {
    
    public override int getInputLimit() {
        return 30;
    }

    public override List<(MatType, int)> getInputList() {
        return new List<(MatType, int)> { (MatType.ONION_PLATE, 6), (MatType.ONION_PASTE, 4), (MatType.ONION_CABLE, 6) };
    }
    
    public override List<(MatType, int)> getOutputList() {
        return new List<(MatType, int)> { (MatType.ENGINE, 1) };
    }

    public override int getPrice() {
        return 1400;
    }

    public override string getName() {
        return "Engine Factory";
    }

    public override string getDescription() {
        return "Produces rocket engine.";
    }
}
