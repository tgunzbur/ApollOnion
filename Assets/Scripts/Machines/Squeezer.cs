﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squeezer : Machine {
    
    public override int getInputLimit() {
        return 6;
    }

    public override List<(MatType, int)> getInputList() {
        return new List<(MatType, int)> { (MatType.ONION_HEART, 3) };
    }
    
    public override List<(MatType, int)> getOutputList() {
        return new List<(MatType, int)> { (MatType.ONION_JUICE, 1) };
    }

    public override int getPrice() {
        return 400;
    }

    public override string getName() {
        return "Squeezer";
    }

    public override string getDescription() {
        return "Squeezes onions into juice.";
    }
}
