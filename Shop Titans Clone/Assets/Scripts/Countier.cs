using System.Collections.Generic;
using UnityEngine;

public class Countier : MoveableObject
{
    static Countier instance;

    List<NPC> costumers = new List<NPC>();


    public static bool IsWaiting => instance.costumers.Count > 0;


    protected override void Awake()
    {
        instance = this;
        base.Awake();
        ValidatePosition();
    }


    public static NPC GetNextCostumer()
    {
        if (instance.costumers.Count == 0)
        {
            throw new UnityException("There is no more costumers to interact!");
        }

        return instance.costumers[instance.costumers.Count - 1];
    }

    public static void AddCostumerToQueue(NPC costumer)
    {
        if (instance.costumers.Contains(costumer))
        {
            Debug.LogError("You want to add a costumer to the countier queue twice!");
            return;
        }

        instance.costumers.Add(costumer);
    }

    public static void RemoveCostumerFromQueue(NPC costumer)
    {
        if (instance.costumers.Contains(costumer) == false)
        {
            Debug.LogError("You want to remove a costumer from the queue who are not part of the queue!");
            return;
        }

        instance.costumers.Remove(costumer);
    }


    public static GridNode GetValidNearByNode()
    {
        var nodes = instance.GetVisitableGridNodes();
        return nodes[new System.Random().Next(0, nodes.Count)];
    }

    protected override List<GridNode> GetVisitableGridNodes()
    {
        List<GridNode> nodes = new List<GridNode>();
        Vector3Int start = pivotCoord + forward * 2;
        for (int i = 0; i < size.x; i++)
        {
            ProcessGridNodeAt(start + right * i, ref nodes);
        }

        start = pivotCoord + forward - right;
        ProcessGridNodeAt(start, ref nodes);
        start += right * (size.x + 1);
        ProcessGridNodeAt(start, ref nodes);

        return nodes;
    }

    public override void OnClicked()
    {
        base.OnClicked();
    }
}