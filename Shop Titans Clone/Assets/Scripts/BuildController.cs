using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BuildController : MonoBehaviour
{
    static BuildController instance;

    List<Fitment> fitmentprefabs = new List<Fitment>();
    Fitment toBuildFitment;
    Fitment toMoveFitment;
    MouseController mouseC;
    [SerializeField]
    TreeNode furniturePrefabTree;

    public static BuildController Instance => instance;
    public Fitment ToBuildFitment { get => toBuildFitment; set => toBuildFitment = value; }
    public Fitment ToMoveFitment { get => toMoveFitment; set => toMoveFitment = value; }
    public List<Fitment> FitmentPrefabs => fitmentprefabs;


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
        toBuildFitment.DoPaperWork();
        toBuildFitment.ValidatePosition();
        GridController.SetActiveBuildGrid(false);
        mouseC.RemoveControl(MouseController.UsageMode.DragLeft, DragFitment);
        toBuildFitment = null;
        toMoveFitment?.ValidatePosition();
    }


    public void CancelConstruction()
    {
        toBuildFitment.RemoveFromShop();
        Destroy(toBuildFitment.gameObject);

        toBuildFitment = null;
        toMoveFitment = null;
        GridController.SetActiveBuildGrid(false);
        mouseC.RemoveControl(MouseController.UsageMode.DragLeft, DragFitment);
    }


    // bool: wether to override the camerapan or not
    public bool DragFitment()
    {
        if (mouseC.LeftBtnDown && !mouseC.RightBtn && !mouseC.IsPointerOver)
        {
            if (toMoveFitment != null)
            {
                toMoveFitment.ValidatePosition();
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit info, 300f))
            {
                Fitment furn = info.collider.gameObject.GetComponent<Fitment>();
                if (furn != null)
                {
                    toMoveFitment = furn;
                    toMoveFitment.ChangePosition(Vector3Int.zero);
                    groundedMousePos.Set(info.point.x, 0, info.point.z);
                    draggingCoord = Vector3Int.RoundToInt(groundedMousePos);
                    return true;
                }
            }
            toMoveFitment = null;
        }
        else if (mouseC.LeftBtn && toMoveFitment != null && !mouseC.RightBtn)
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
                    toMoveFitment.ChangePosition(diff);
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
            fitmentprefabs.AddRange(Resources.LoadAll<Fitment>(path.ToString() + parent.FileName));
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