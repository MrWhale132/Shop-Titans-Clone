using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GridController;

public class BuildNode
{
    public enum States { Empty, Occupied, Valid, Invalid }

    Image image;
    List<MoveableObject> occupiers;

    public MoveableObject Owner => occupiers.Count > 0 ? occupiers[0] : null;
    public Vector3 Position => image.transform.position;
    public bool Crowded => occupiers.Count > 1;


    public BuildNode(Image image)
    {
        this.image = image;
        occupiers = new List<MoveableObject>();
    }


    public void AddOccupier(MoveableObject moveable)
    {
        if (occupiers.Contains(moveable))
        {
            Debug.LogError("You want to add a moveable into a BuildNode when it is already there!");
        }
        else occupiers.Add(moveable);
    }

    public void RemoveOccupier(MoveableObject moveable)
    {
        if (occupiers.Remove(moveable) == false)
        {
            Debug.LogError("You want to remove a moveable from a BuildNode what is do not present in the occupiers list!");
        }
    }

    public void DetermineState()
    {
        if (occupiers.Count > 1)
        {
            SetState(States.Invalid);
        }
        else if (occupiers.Count == 1)
            SetState(States.Occupied);
        else
            Debug.LogError("This method can only by used if there is at least one moveable to occupie the same tile!");
    }



    public void SetState(States states)
    {
        image.sprite = ValidationSprite;
        switch (states)
        {
            case States.Empty:
                image.sprite = DefaultBuildNode;
                image.color = Color.white;
                break;
            case States.Valid:
                image.color = ValidColor;
                break;
            case States.Invalid:
                image.color = InvalidColor;
                break;
            case States.Occupied:
                image.color = OccupiedColor;
                break;
            default: break;
        }
    }
}