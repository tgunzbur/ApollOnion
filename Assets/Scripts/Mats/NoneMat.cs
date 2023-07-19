using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoneMat : Mat {
    public override MatType getMatType() {
        return MatType.NONE;
    }
}