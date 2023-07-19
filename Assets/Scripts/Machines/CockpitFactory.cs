using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockpitFactory : Machine {
    
    public override int getInputLimit() {
        return 20;
    }

    public override List<(MatType, int)> getInputList() {
        return new List<(MatType, int)> { (MatType.ONION_PLATE, 6), (MatType.ONION_GLASS, 6) };
    }
    
    public override List<(MatType, int)> getOutputList() {
        return new List<(MatType, int)> { (MatType.COCKPIT, 1) };
    }

    public override int getPrice() {
        return 1400;
    }

    public override string getName() {
        return "Cockpit Factory";
    }

    public override string getDescription() {
        return "Produces rocket cockpit.";
    }
}
