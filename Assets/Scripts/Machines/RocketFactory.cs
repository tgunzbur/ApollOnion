using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketFactory : Machine {
    
    public override int getInputLimit() {
        return 5;
    }

    public override List<(MatType, int)> getInputList() {
        return new List<(MatType, int)> { (MatType.TANK, 2), (MatType.ENGINE, 1), (MatType.COCKPIT, 1) };
    }
    
    public override List<(MatType, int)> getOutputList() {
        return new List<(MatType, int)> { (MatType.ROCKET, 1) };
    }

    public override int getPrice() {
        return 1600;
    }

    public override string getName() {
        return "Rocket Factory";
    }

    public override string getDescription() {
        return "Produces rocket.";
    }
}
