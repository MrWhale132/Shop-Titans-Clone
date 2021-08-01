using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridController;


public class Fitment : MonoBehaviour
{
    [SerializeField]
    protected Transform pivot;
    [SerializeField]
    string furnName;
    [SerializeField]
    Sprite portrait;
    [SerializeField]
    protected Vector2Int size;
    [SerializeField]
    bool blockOthers;
    [SerializeField]
    bool[] visitableSides;
    [SerializeField]
    int buildCost;


    protected Vector3Int pivotCoord;
    protected Vector3Int forward;
    protected Vector3Int right;


    public string Name => furnName;
    public Sprite Portrait => portrait;
    public int BuildCost => buildCost;



    protected virtual void Awake()
    {
        forward = Vector3Int.RoundToInt(transform.forward);
        right = Vector3Int.RoundToInt(transform.right);

        pivot.forward = forward;
        pivot.position = transform.position - (Vector3)forward * (size.y / 2f - 0.1f) - (Vector3)right * (size.x / 2f - 0.1f);

        pivotCoord = Vector3Int.FloorToInt(pivot.position);

        foreach (BuildNode buildNode in GetOccupiedTiles())
        {
            GridNode pathNode = GetNodeAt(buildNode.Position);
            pathNode.SetWalkable(false);
            buildNode.AddOccupier(this);
        }

        ValidatePosition();
    }



    public virtual void OnClicked()
    {
        OverlayMenuController.CurrentMenu.Exit();
        FurnitureMoveingMenu.ObjectToMove = this;
        // this is only temporary: every derived class should override to call its own editor
        FurnitureEditor.Instance.Enter();
    }


    public virtual void DoPaperWork()
    {

    }


    public void SetPosition(Vector3Int newCoord)
    {
        ClearPosition();

        pivotCoord = newCoord;
        Vector3 halfWidth = (Vector3)right * (size.x / 2f);
        Vector3 halfHeight = (Vector3)forward * (size.y / 2f);
        transform.position = newCoord + halfHeight + halfWidth;

        CheckoutPosition();
    }


    public void ChangePosition(Vector3Int moveCoord)
    {
        if (CheckMapBoundary(pivotCoord + moveCoord) == false)
            return;

        ClearPosition();

        pivotCoord += moveCoord;
        Vector3 newPos = new Vector3(transform.position.x + moveCoord.x, transform.position.y, transform.position.z + moveCoord.z);
        transform.position = newPos;

        CheckoutPosition();
    }


    public void ValidatePosition()
    {
        foreach (BuildNode node in GetOccupiedTiles())
        {
            node.DetermineState();
        }
    }


    void CheckoutPosition()
    {
        foreach (GridNode gridNode in GetOccupiedPathNodes())
        {
            BuildNode buildNode = GetBuildNodeAt(gridNode.X, gridNode.Y);

            if (buildNode.Owner != null && buildNode.Owner.blockOthers && buildNode.Owner != this)
            {
                buildNode.SetState(BuildNode.States.Invalid);
            }
            else
            {
                gridNode.SetWalkable(false);
                buildNode.SetState(BuildNode.States.Valid);
            }
            buildNode.AddOccupier(this);
        }
    }


    public virtual void RemoveFromShop()
    {
        ClearPosition();
    }


    public bool HasValidPosition()
    {
        foreach (BuildNode node in GetOccupiedTiles())
        {
            if (node.Crowded)
                return false;
        }
        return true;
    }


    public bool Rotate(int degrees)
    {
        Vector3 oldPos = pivot.position;
        Quaternion oldRot = pivot.rotation;
        pivot.RotateAround(pivotCoord, Vector3.up, degrees % 360);
        bool validRot = CheckRotationByPivot();
        pivot.position = oldPos;
        pivot.rotation = oldRot;

        if (validRot == false)
        {
            return false;
        }

        ClearPosition();

        transform.RotateAround(Vector3Int.RoundToInt(pivot.position), Vector3.up, degrees % 360);
        forward = Vector3Int.RoundToInt(pivot.forward);
        right = Vector3Int.RoundToInt(pivot.right);
        pivotCoord = Vector3Int.FloorToInt(pivot.position);

        CheckoutPosition();


        if (BuildController.Instance.ToMoveFitment != this)
        {
            BuildController.Instance.ToMoveFitment.ValidatePosition();
            BuildController.Instance.ToMoveFitment = this;
        }

        return true;
    }


    bool CheckRotationByPivot()
    {
        foreach (BuildNode node in GetTilesByPivot())
            if (node == null)
                return false;
        return true;
    }


    bool CheckMapBoundary(Vector3Int from)
    {
        foreach (BuildNode node in GetTilesFrom(from))
            if (node == null)
                return false;
        return true;
    }


    void ClearPosition()
    {
        foreach (GridNode gridNode in GetOccupiedPathNodes())
        {
            BuildNode buildNode = GetBuildNodeAt(gridNode.X, gridNode.Y);
            buildNode.RemoveOccupier(this);
            if (buildNode.Owner == null)
            {
                gridNode.SetWalkable(true);
                buildNode.SetState(BuildNode.States.Empty);
            }
            else
            {
                buildNode.DetermineState();
            }
        }
    }


    protected virtual List<GridNode> GetVisitableGridNodes()
    {
        List<GridNode> visitables = new List<GridNode>();
        Vector3Int bottomLeft = pivotCoord - forward - right;
        int side = 0;
        // 0: bottom, 2: top
        for (int y = 0; side < 4; y = size.y + 1)
        {
            if (visitableSides[side])
            {
                Vector3Int row = bottomLeft + forward * y;
                for (int x = 0; x < size.x + 2; x++)
                {
                    ProcessGridNodeAt(row + right * x, ref visitables);
                }
            }
            side += 2;
        }
        side = 1;
        // 1: left, 3: right
        for (int x = 0; side < 4; x = size.x + 1)
        {
            if (visitableSides[side])
            {
                Vector3Int col = bottomLeft + right * x;
                for (int y = 1; y < size.y + 1; y++)
                {
                    ProcessGridNodeAt(col + forward * y, ref visitables);
                }
            }
            side += 2;
        }
        return visitables;
    }

    protected void ProcessGridNodeAt(Vector3Int at, ref List<GridNode> nodes)
    {
        GridNode node = GetNodeAt(at.x, at.z);
        if (node != null && node.Walkable)
        {
            nodes.Add(node);
        }
    }



    IEnumerable GetOccupiedTiles()
    {
        for (int i = 0; i < size.y; i++)
        {
            Vector3Int row = pivotCoord + forward * i;
            for (int j = 0; j < size.x; j++)
            {
                yield return GetBuildNodeAt(row + right * j);
            }
        }
    }

    IEnumerable GetOccupiedPathNodes()
    {
        for (int i = 0; i < size.y; i++)
        {
            Vector3Int row = pivotCoord + forward * i;
            for (int j = 0; j < size.x; j++)
            {
                yield return GetNodeAt(row + right * j);
            }
        }
    }

    IEnumerable GetTilesFrom(Vector3Int pivotCoord)
    {
        for (int i = 0; i < size.y; i++)
        {
            Vector3Int row = pivotCoord + forward * i;
            for (int j = 0; j < size.x; j++)
            {
                yield return GetBuildNodeAt(row + right * j);
            }
        }
    }

    IEnumerable GetTilesByPivot()
    {
        Vector3Int pivotCoord = Vector3Int.FloorToInt(pivot.position);
        Vector3Int forward = Vector3Int.RoundToInt(pivot.forward);
        Vector3Int right = Vector3Int.RoundToInt(pivot.right);
        for (int i = 0; i < size.y; i++)
        {
            Vector3Int row = pivotCoord + forward * i;
            for (int j = 0; j < size.x; j++)
            {
                yield return GetBuildNodeAt(row + right * j);
            }
        }
    }
}