using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structs
{
    public struct IDAndSpeed
    {
        public int ID;
        public double effectiveSpeed;

        public IDAndSpeed(int intValue, double doubleValue)
        {
            ID = intValue;
            effectiveSpeed = doubleValue;
        }
    }

}
