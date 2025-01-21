using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MineState
{
    None = 0,
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,

    Mine = -1,
}

public enum CellState
{
    Closed = 0,
    Open = 1,
    Flag = 2,
}
