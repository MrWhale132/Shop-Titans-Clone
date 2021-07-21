using System;
using System.Collections.Generic;
using UnityEngine;
using static GridController;


public class GridNode : IComparable<GridNode>
{
    List<GridNode> neighbours;
    GridNode parent;
    Vector3 position;
    int x;
    int y;
    int gCost;
    int hCost;
    bool walkable;
    int heapIndex;
    IPathUnit pathUnit;

    public GridNode(Vector3 worldPosition)
    {
        position = worldPosition;
        neighbours = new List<GridNode>();
        walkable = true;
        x = Mathf.FloorToInt(position.x);
        y = Mathf.FloorToInt(position.z);
    }

    public int X => x;
    public int Y => y;
    public int Gcost { get => gCost; set => gCost = value; }
    public int Hcost { get => hCost; set => hCost = value; }
    public int Fcost => gCost + hCost;
    public int HeapIndex { get => heapIndex; set => heapIndex = value; }
    public bool Walkable => walkable;
    public List<GridNode> Neighbours => neighbours;
    public GridNode Parent { get => parent; set => parent = value; }
    public Vector3 Position => position;
    public IPathUnit PathUnit => pathUnit;

    public void SetPathUnit(IPathUnit newUnit)
    {
        if (pathUnit != null && newUnit != null)
        {
            Debug.LogError("Somebody else already occupying this node. " + x + " " + y + " " +position +" "+pathUnit +" "+newUnit);
        }
        else if (pathUnit == null && newUnit == null)
        {
            Debug.LogError("This GridNode was already empty before you tried to leave it. " + x + " " + y + " " + position + " " + pathUnit + " " + newUnit);
        }
        pathUnit = newUnit;
    }


    public void SetWalkable(bool walkable)
    {
        if (this.walkable == walkable)
            return;

        this.walkable = walkable;
        GridNode current;
        GridNode vertNeighbour;
        GridNode horzNeighbour;
        if (walkable)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    current = GetNodeAt(x - i, y - j);
                    if (current == null)
                        continue;

                    if (i != 0 && j != 0)
                    {
                        vertNeighbour = GetNodeAt(x - i, y);
                        horzNeighbour = GetNodeAt(x, y - j);
                        if (vertNeighbour == null || vertNeighbour.Walkable == false ||
                            horzNeighbour == null || horzNeighbour.Walkable == false)
                            continue;
                    }
                    neighbours.Add(current);
                }
            }
            for (int vert = -1; vert <= 1; vert += 2)
            {
                for (int horz = -1; horz <= 1; horz += 2)
                {
                    current = GetNodeAt(x - vert, y - horz);
                    if (current != null && current.walkable == true)
                    {
                        vertNeighbour = GetNodeAt(x - vert, y);
                        horzNeighbour = GetNodeAt(x, y - horz);
                        if (vertNeighbour == null || horzNeighbour == null)
                            continue;
                        vertNeighbour.neighbours.Add(horzNeighbour);
                        horzNeighbour.neighbours.Add(vertNeighbour);
                    }
                }
            }
        }
        else
        {
            for (int vert = -1; vert <= 1; vert += 2)
            {
                for (int horz = -1; horz <= 1; horz += 2)
                {
                    vertNeighbour = GetNodeAt(x - vert, y);
                    horzNeighbour = GetNodeAt(x, y - horz);
                    if (vertNeighbour == null || horzNeighbour == null)
                        continue;
                    vertNeighbour.neighbours.Remove(horzNeighbour);
                    horzNeighbour.neighbours.Remove(vertNeighbour);
                }
            }
            neighbours.Clear();
        }
    }

    public int CompareTo(GridNode other)
    {
        if (Fcost == other.Fcost)
        {
            if (hCost < other.hCost) return 1;
            if (hCost > other.hCost) return -1;
            return 0;
        }
        if (Fcost < other.Fcost) return 1;
        return -1;
    }
}