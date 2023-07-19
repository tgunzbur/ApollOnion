using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankFactory : Machine {

    public override int getInputLimit() {
        return 30;
    }

    public override List<(MatType, int)> getInputList() {
        return new List<(MatType, int)> { (MatType.ONION_PLATE, 8), (MatType.ONION_PASTE, 4) };
    }
    
    public override List<(MatType, int)> getOutputList() {
        return new List<(MatType, int)> { (MatType.TANK, 1) };
    }

    public override int getPrice() {
        return 1400;
    }

    public override string getName() {
        return "Tank Factory";
    }

    public override string getDescription() {
        return "Producues rocket tanks.";
    }
}
