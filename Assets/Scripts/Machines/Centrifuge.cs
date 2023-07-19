using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Centrifuge : Machine {
    
    public override int getInputLimit() {
        return 8;
    }

    public override List<(MatType, int)> getInputList() {
        return new List<(MatType, int)> { (MatType.ONION_JUICE, 5) };
    }
    
    public override List<(MatType, int)> getOutputList() {
        return new List<(MatType, int)> { (MatType.ONION_FUEL, 1), (MatType.ONION_PASTE, 1) };
    }

    public override int getPrice() {
        return 800;
    }

    public override string getName() {
        return "Centrifuge";
    }

    public override string getDescription() {
        return "Splits solid and liquid from juice.";
    }
}
