using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BraidingMachine : Machine {
    
    public override int getInputLimit() {
        return 8;
    }

    public override List<(MatType, int)> getInputList() {
        return new List<(MatType, int)> { (MatType.ONION_PEELING, 4) };
    }
    
    public override List<(MatType, int)> getOutputList() {
        return new List<(MatType, int)> { (MatType.ONION_FIBER, 1) };
    }

    public override int getPrice() {
        return 500;
    }

    public override string getName() {
        return "Braiding Machine";
    }

    public override string getDescription() {
        return "Braids onions peeling in fiber.";
    }
}
