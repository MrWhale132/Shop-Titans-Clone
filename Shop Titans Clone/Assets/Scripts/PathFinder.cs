using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    public static Vector3[] FindPath(Vector3 startPos, Vector3 endPos)
    {
        GridNode start = GridController.GetNodeAt(startPos);
        GridNode end = GridController.GetNodeAt(endPos);

        double diameter = Math.Sqrt(GridController.NodesCount);
        GridNodeHeap openSet = new GridNodeHeap(Convert.ToInt32(diameter * 5));
        HashSet<GridNode> closedSet = new HashSet<GridNode>();

        openSet.Add(start);
        while (openSet.Count > 0)
        {
            GridNode current = openSet.RemoveFirst();

            if (current == end)
            {
                return ReTracePath(start, end);
            }

            closedSet.Add(current);

            foreach (GridNode neighbour in current.Neighbours)
            {
                if ((neighbour.Walkable == false || closedSet.Contains(neighbour)) &&
                    neighbour != end)
                {
                    continue;
                }

                bool contain = openSet.Contains(neighbour);
                int new_move_cost = current.Gcost + GetDistance(current, neighbour);
                if (new_move_cost < neighbour.Gcost || contain == false)
                {
                    neighbour.Gcost = new_move_cost;
                    neighbour.Parent = current;
                    if (contain == false)
                    {
                        neighbour.Hcost = GetDistance(neighbour, end);
                        openSet.Add(neighbour);
                    }
                    else openSet.UpdateItem(neighbour);
                }
            }
        }
        return default;
    }

    static Vector3[] ReTracePath(GridNode start, GridNode end)
    {
        List<Vector3> path = new List<Vector3>();
        GridNode current = end;
        while (current != start)
        {
            path.Add(current.Position);
            current = current.Parent;
        }
        path.Reverse();
        return path.ToArray();
    }

    static int GetDistance(GridNode a, GridNode b)
    {
        int distX = Math.Abs(a.X - b.X);
        int distY = Math.Abs(a.Y - b.Y);

        return Math.Min(distY, distX) * 14 + 10 * Math.Abs(distY - distX);
    }
}