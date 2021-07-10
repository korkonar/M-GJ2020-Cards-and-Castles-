using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IPrioritizable
{
    public Vector2Int ID;
    public bool visited;
    public int costToGetTo;
    private int shortestCostSoFar=100000000;
    public Vector2Int prev = new Vector2Int(10069, 10069);
    public bool isGoal = false;

    public int Priority {
        get {
            return shortestCostSoFar;
        }
        set {
            shortestCostSoFar = value;
        }
    }
}
