using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class BuildController : MonoBehaviour
{
    static BuildController instance;

    List<Furniture> furniturePrefabs = new List<Furniture>();
    MoveableObject toBuildFurn;
    MoveableObject moveable;
    MouseController mouseC;
    [SerializeField]
    TreeNode furniturePrefabTree;

    public static BuildController Instance => instance;
    public MoveableObject ToBuildFurn { get => toBuildFurn; set => toBuildFurn = value; }
    public MoveableObject ToMoveFurn { get => moveable; set => moveable = value; }
    public List<Furniture> FurnPrefabs => furniturePrefabs;


    void Awake()
    {
        instance = this;
        LoadAssets(furniturePrefabTree);
    }

    void Start()
    {
        mouseC = MouseController.Instace;
    }
   

    public void BuildMoveable()
    {
        toBuildFurn.ValidatePosition();
        GridController.SetActiveBuildGrid(false);
        mouseC.RemoveControl(MouseController.UsageMode.DragLeft, DragFurn);
        toBuildFurn = null;
    }


    public void CancelConstruction()
    {
        toBuildFurn.RemoveFromShop();
        Destroy(toBuildFurn.gameObject);

        toBuildFurn = null;
        moveable = null;
        GridController.SetActiveBuildGrid(false);
        mouseC.RemoveControl(MouseController.UsageMode.DragLeft, DragFurn);
    }


    // wether to override the camerapan or not
    public bool DragFurn()
    {
        if (mouseC.LeftBtnDown && !mouseC.RightBtn && !mouseC.IsPointerOver)
        {
            if (moveable != null)
            {
                moveable.ValidatePosition();
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit info, 300f))
            {
                MoveableObject furn = info.collider.gameObject.GetComponent<MoveableObject>();
                if (furn != null)
                {
                    moveable = furn;
                    moveable.ChangePosition(Vector3Int.zero);
                    groundedMousePos.Set(info.point.x, 0, info.point.z);
                    draggingCoord = Vector3Int.RoundToInt(groundedMousePos);
                    return true;
                }
            }
            moveable = null;
        }
        else if (mouseC.LeftBtn && moveable != null && !mouseC.RightBtn)
        {
            Vector3 dragVec = mouseC.GetDragVector();
            if (dragVec.x != 0 || dragVec.z != 0)
            {
                // Warning: no check for y component
                groundedMousePos += dragVec;
                Vector3Int mouseCoord = Vector3Int.RoundToInt(groundedMousePos);
                Vector3Int diff = mouseCoord - draggingCoord;
                if (diff != Vector3Int.zero)
                {
                    draggingCoord += diff;
                    moveable.ChangePosition(diff);
                }
            }
            return true;
        }

        return false;
    }
    Vector3 groundedMousePos;
    // The coordinate of the point of the furniture which clicked by the user
    // The dragging difference is calculated from this coord
    Vector3Int draggingCoord;



    void LoadAssets(TreeNode parent)
    {
        if (parent.IsLeaf)
        {
            furniturePrefabs.AddRange(Resources.LoadAll<Furniture>(path.ToString() + parent.FileName));
            return;
        }
        path.Append(parent.FileName);
        foreach (TreeNode child in parent)
        {
            LoadAssets(child);
        }
        path.Remove(path.Length - parent.FileName.Length, parent.FileName.Length - 1);
    }
    StringBuilder path = new StringBuilder();
}