using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour {

    public GridManager gridManager;
    /// <summary>
    /// Finds shortest Path from start to goal through the grid
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="start"> indices in grid representing start, so only ints </param>
    /// <param name="goal"> indices in grid representing end goal, so only ints</param>
    /// <returns></returns>

    public Node[,] nodeArray;
    public Vector2Int start;
    public List<Vector2Int> visited = new List<Vector2Int>();

    public List<int> FindShortestPath(Grid grid, Vector2Int Start, Vector2Int goal) {
        nodeArray = getNodeArray(grid);
        start = Start;


        nodeArray[goal.x, goal.y].isGoal = true;
        nodeArray[start.x, start.y].Priority = 0;

        PriorityQueue<Node> PQ = new PriorityQueue<Node>();
        foreach (Node node in nodeArray) {
            PQ.Enqueue(node);
        }
        while (PQ.Count() != 0) {
            Node currentNode = PQ.Dequeue();
            UpdateNeighbors(ref PQ, currentNode.ID.x, currentNode.ID.y);
        }

        // Checking any faulty blocks
        int width = nodeArray.GetLength(0);
        int height = nodeArray.GetLength(1);
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (nodeArray[x, y].prev.x == 10069) {
                    if (x == start.x && y == start.y) {
                        continue;
                    }
                    gridManager.grid.cellArray[x, y].GetComponent<SpriteRenderer>().color = new Color(0, 0.5f, 0);
                    Debug.Log("Faulty block found " + x + " " + y);
                }
            }
        }
        return shortestPathTo(goal.x, goal.y);

    }

    public List<int> shortestPathTo(int x, int y) {
        List<int> res = new List<int>();
        if (start.x == x && start.y == y) {
            return res;
        }
        if (nodeArray[x, y].prev.x == 10069) {
            Debug.Log("Grid wasn't fully evaluated before pathfinding. A square in the shortest path is a dead end");
            return res;
        }

        res.Add(y);
        res.Add(x);
        res.AddRange(shortestPathTo(nodeArray[x, y].prev.x, nodeArray[x, y].prev.y));
        return res;

    }


    public int shortestCostToReach(int x, int y) {
        if (start.x == x && start.y == y) {
            return 0;
        }
        if (x == 10069) {
            return 100000;
        }
        return nodeArray[x, y].costToGetTo + shortestCostToReach(nodeArray[x, y].prev.x, nodeArray[x, y].prev.y);

    }

    public void UpdateNeighbors(ref PriorityQueue<Node> pq, int x, int y) {
        visited.Add(new Vector2Int(x, y));
        List<Vector2Int> neighbors = getNeighbors(x, y);
        if (neighbors.Count == 0)
            return;

        // Cost to get somewhere depends on cost to the previous node, the cost to get somewhere (very expensive to get into a wall), and the amount of steps to get there.
        for (int i = 0; i < neighbors.Count; i++) {
            foreach (Node node in pq.Entries) {
                if (node.ID == neighbors[i]) {
                    //Handle nodes here
                    if (node.Priority > nodeArray[x, y].Priority + node.costToGetTo) {
                        node.Priority = nodeArray[x, y].Priority + node.costToGetTo;
                        node.prev = new Vector2Int(x, y);
                    }
                }
            }
            // We've been editting values without DE-ENqueue so we have to re-sort it.
            pq.ReSort();
            



            //Node temp = nodeArray[neighbors[i].x, neighbors[i].y];
            //if (neighbors[i].x == 4 && neighbors[i].y == 1) {
            //    Debug.Log("");
            //}
            //int shortestCostNeighbor = shortestCostToReach(neighbors[i].x, neighbors[i].y);
            //int shortestCostMe = shortestCostToReach(x, y) + temp.costToGetTo;
            //if (shortestCostNeighbor > shortestCostMe) {
            //    nodeArray[neighbors[i].x, neighbors[i].y].prev.x = x;
            //    nodeArray[neighbors[i].x, neighbors[i].y].prev.y = y;
            //}
        }
    }

    /// <summary>
    /// Converts a grid into a pathfindable grid
    /// This is where you set the cost for every type of tile
    /// </summary>
    /// <param name="grid"></param>
    /// <returns></returns>
    public Node[,] getNodeArray(Grid grid) {
        Node[,] nodeArray = new Node[grid.width, grid.height];
        for (int y = 0; y < grid.height; y++) {
            for (int x = 0; x < grid.width; x++) {
                Node current = new Node();
                current.visited = false;
                current.ID = new Vector2Int(x, y);
                Cell.CollisionMode cm = grid.cellArray[x, y].GetComponent<Cell>().CollisionMode1;
                if (cm == Cell.CollisionMode.Wall) {
                    current.costToGetTo = 100;
                } else if (cm == Cell.CollisionMode.Floor) {
                    current.costToGetTo = 1;
                } else if ((int)cm > 1) {
                    current.costToGetTo = 10;
                }

                nodeArray[x, y] = current;
            }
        }
        return nodeArray;
    }

    public List<Vector2Int> getNeighbors(int x, int y, bool onlyUnvisited = false) {
        List<Vector2Int> res = new List<Vector2Int>();
        int width = nodeArray.GetLength(0);
        int height = nodeArray.GetLength(1);
        if (x - 1 >= 0 && x - 1 < width) {
            if (!onlyUnvisited || nodeArray[x - 1, y].visited == false) {
                res.Add(new Vector2Int(x - 1, y));
            }

        }
        if (x + 1 >= 0 && x + 1 < width) {
            if (!onlyUnvisited || nodeArray[x + 1, y].visited == false) {
                res.Add(new Vector2Int(x + 1, y));
            }
        }
        if (y - 1 >= 0 && y - 1 < height) {
            if (!onlyUnvisited || nodeArray[x, y - 1].visited == false) {
                res.Add(new Vector2Int(x, y - 1));
            }
        }
        if (y + 1 >= 0 && y + 1 < height) {
            if (!onlyUnvisited || nodeArray[x, y + 1].visited == false) {
                res.Add(new Vector2Int(x, y + 1));
            }
        }
        return res;
    }
}
