using UnityEngine;
using UnityEngine.UI;

public class BuildController : MonoBehaviour
{
    static BuildController instance;

    [SerializeField]
    Furniture[] furnitures;
    MoveableObject toBuildFurn;
    MoveableObject moveable;
    MouseController mouseC;

    public static BuildController Instance => instance;
    public MoveableObject ToBuildFurn { get => toBuildFurn; set => toBuildFurn = value; }
    public MoveableObject ToMoveFurn { get => moveable; set => moveable = value; }
    public Furniture[] FurnPrefabs => furnitures;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        mouseC = MouseController.Instace;

        Furniture forTest = Instantiate(furnitures[0], new Vector3(15.5f, 0, 5), Quaternion.identity);
        toBuildFurn = forTest;
        toBuildFurn.ValidatePosition();

        System.Random r = new System.Random();
        int length = r.Next(2, forTest.Capacity);
        for (int i = 0; i < length; i++)
        {
            Item prefab = itemPrefabs[r.Next(0, itemPrefabs.Length)];
            Item itemToAdd = Instantiate(prefab);
            forTest.AddItem(itemToAdd);
        }
    }
   

    public void BuildMoveable()
    {
        toBuildFurn.ValidatePosition();
        GridController.SetActiveBuildGrid(false);
        mouseC.RemoveControl(MouseController.UsageMode.DragLeft, DragFurn);

        Furniture forTest = toBuildFurn as Furniture;
        System.Random r = new System.Random();
        int length = r.Next(0, forTest.Capacity);
        for (int i = 0; i < 6; i++)
        {
            Item prefab = itemPrefabs[r.Next(0, itemPrefabs.Length)];
            Item itemToAdd = Instantiate(prefab);
            forTest.AddItem(itemToAdd);
        }
        toBuildFurn = null;
    }

    public Item[] itemPrefabs;

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
}