using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class MouseController : MonoBehaviour
{
    public enum UsageMode { DragLeft }
    public enum UsageRule { Override }

    static MouseController instance;

    [SerializeField]
    int cameraPanSpeed;
    [SerializeField]
    int cameraPanAroundSpeed;
    [SerializeField]
    float panAroundRingRadius;
    [SerializeField]
    int cameraZoomDistance;
    [SerializeField]
    LayerMask UI_LayerMask;
    [SerializeField]
    LayerMask moveableMask;

    GraphicRaycaster raycaster;
    PointerEventData pointerData;
    List<RaycastResult> results;
    [SerializeField]
    LayerMask gridNodeLayer;

    bool leftBtn;
    bool leftBtnDown;
    bool leftBtnUp;
    bool rightBtn;
    bool rightBtnDown;
    bool isPointerOver;

    Vector3 prevMousePos;
    Vector3 panAroundPoint;
    Plane dragPlane;
    Ray ray;

    List<Func<bool>>[] usages;

    public static MouseController Instace => instance;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        raycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
        results = new List<RaycastResult>();

        Array usageValues = Enum.GetValues(typeof(UsageMode));
        usages = new List<Func<bool>>[usageValues.Length];
        foreach (int usage in usageValues)
        {
            usages[usage] = new List<Func<bool>>();
        }
        usages[(int)UsageMode.DragLeft].Add(() => { PanCamera(); return false; });
    }


    public void AddControl(UsageMode usage, Func<bool> control)
    {
        usages[(int)usage].Add(control);
    }

    public void RemoveControl(UsageMode usage, Func<bool> control)
    {
        var controlList = usages[(int)usage];
        if (controlList[controlList.Count - 1] != control)
        {
            Debug.LogError("A mouse control try to remove it self from the list, although he is not the current control.");
        }
        controlList.RemoveAt(controlList.Count - 1);
    }


    public void ZoomToCostumer(Vector3 targetPos, Vector3 rotateToward)
    {
        zooming = true;
        rotating = true;
        targetZoomPos = targetPos;
        this.rotateToward = rotateToward;
        UpdateZoomPlane();
    }



    void Update()
    {
        UpdateInputs();
        PanAroundCamera();
        ZoomCamera();
        SmoothCameraRotateToward();
        DetectFurnitureClick();

        foreach (var usage in usages)
        {
            for (int i = usage.Count - 1; i >= 0; i--)
            {
                bool overRide = usage[i]();
                if (overRide)
                    break;
            }
        }
        prevMousePos = Input.mousePosition;
    }


    void UpdateInputs()
    {
        leftBtn = Input.GetMouseButton(0);
        leftBtnDown = Input.GetMouseButtonDown(0);
        leftBtnUp = Input.GetMouseButtonUp(0);
        rightBtn = Input.GetMouseButton(1);
        rightBtnDown = Input.GetMouseButtonDown(1);

        pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;
        results.Clear();
        raycaster.Raycast(pointerData, results);
        if (results.Count > 0)
        {
            isPointerOver = true;
        }
        else isPointerOver = false;
    }


    void DetectFurnitureClick()
    {
        if (isPointerOver) return;

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (leftBtnDown && Physics.Raycast(ray, out hitInfo, 300f))
        {
            clickedMoveable = hitInfo.collider.GetComponent<MoveableObject>();
        }
        if (leftBtnUp && Physics.Raycast(ray, out hitInfo, 300f, moveableMask))
        {
            if (clickedMoveable == hitInfo.collider.GetComponent<MoveableObject>())
            {
                if (!FurnitureMoveingMenu.Moveing &&
                    !ConstructionMenu.Constructing)
                    clickedMoveable.OnClicked();
            }
        }
    }
    MoveableObject clickedMoveable;
    RaycastHit hitInfo;


    void PanCamera()
    {
        if (leftBtn && !leftBtnDown && !rightBtn && !isPointerOver)
        {
            zooming = false;
            rotating = false;
            UpdateDragPlane();

            Vector3 dragDir = GetDragVector();
            dragDir *= -1;
            Camera.main.transform.Translate(dragDir * Time.deltaTime * cameraPanSpeed, Space.World);
        }
    }


    void PanAroundCamera()
    {
        if (rightBtnDown && !leftBtn && !isPointerOver)
        {
            panAroundPoint = GetGroundIntersectPoint();
            panAroundRingRadius = Mathf.Sqrt(Camera.main.transform.position.y);
        }
        else if (rightBtn && !leftBtn && !isPointerOver)
        {
            zooming = false;
            rotating = false;
            // some jittering occur, probably cause of diff.x
            Vector3 diff = Input.mousePosition - prevMousePos;
            if (diff.x == 0)
                return;

            float angle = diff.x * Time.deltaTime * cameraPanAroundSpeed;
            Camera.main.transform.RotateAround(prevRingPoint, Vector3.up, angle);

            Vector3 projectedForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
            Vector3 distance = projectedForward.normalized * panAroundRingRadius;
            Vector3 ringpoint = panAroundPoint + distance;
            Vector3 lookAngles = Quaternion.LookRotation(ringpoint - Camera.main.transform.position).eulerAngles;
            lookAngles.x = Camera.main.transform.eulerAngles.x;
            Camera.main.transform.rotation = Quaternion.Euler(lookAngles);

            prevRingPoint = ringpoint;
        }
    }
    Vector3 prevRingPoint;


    void SmoothCameraRotateToward()
    {
        if (rotating)
        {
            Quaternion lookRot = Quaternion.LookRotation(rotateToward - Camera.main.transform.position);
            Quaternion smoothedRot = Quaternion.Lerp(Camera.main.transform.rotation, lookRot, rotationSmoothnes);
            Vector3 eulers = smoothedRot.eulerAngles;
            eulers.x = 50;
            Quaternion fixedRot = Quaternion.Euler(eulers);
            Camera.main.transform.rotation = fixedRot;

            eulers = lookRot.eulerAngles;
            eulers.x = 50;
            fixedRot = Quaternion.Euler(eulers);
            if (Quaternion.Angle(Camera.main.transform.rotation, fixedRot) < 0.2)
            {
                Camera.main.transform.rotation = fixedRot;
                rotating = false;
            }
        }
    }
    [SerializeField]
    float rotationSmoothnes;
    Vector3 rotateToward;
    bool rotating;


    void ZoomCamera()
    {
        int delta = (int)Input.mouseScrollDelta.y;
        if (delta != 0 && !isPointerOver)
        {
            Vector3 distance = Camera.main.transform.forward * delta * cameraZoomDistance;

            // both of them has the same sign so the zooming hapening in the same direction as the prev
            if (delta / prevDelta > 0 && zooming)
            {
                targetZoomPos += distance;
            }
            else targetZoomPos = Camera.main.transform.position + distance;

            Vector3 normal = (Camera.main.transform.position - targetZoomPos).normalized;
            zoomPlane.SetNormalAndPosition(normal, targetZoomPos + normal * 0.025f);

            zooming = true;
            prevDelta = delta;
        }
        if (zooming)
        {
            Vector3 smoothedPos = Vector3.Lerp(Camera.main.transform.position, targetZoomPos, smoothness);
            Vector3 dir = smoothedPos - Camera.main.transform.position;
            Vector3 distance = dir * Time.deltaTime * cameraZoomDistance;
            Camera.main.transform.Translate(distance, Space.World);

            if (zoomPlane.GetSide(Camera.main.transform.position) == false)
            {
                Camera.main.transform.position = targetZoomPos;
                zooming = false;
            }
        }
    }
    [SerializeField]
    float zoomStoppingTreshhold;
    [SerializeField]
    float smoothness;
    float prevDelta;
    bool zooming;
    Vector3 targetZoomPos;
    Plane zoomPlane;



    public Vector3 GetDragVector()
    {
        ray = Camera.main.ScreenPointToRay(prevMousePos);
        dragPlane.Raycast(ray, out float distance);
        Vector3 dragStart = ray.GetPoint(distance);
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        dragPlane.Raycast(ray, out distance);
        Vector3 dragEnd = ray.GetPoint(distance);
        Vector3 dragDir = dragEnd - dragStart;
        //dragDir.Set(dragDir.x, 0, dragDir.z);
        return dragDir;
    }


    void UpdateDragPlane()
    {
        Vector3 groundIntersectPoint = GetGroundIntersectPoint();
        Vector3 normal = (Camera.main.transform.position - groundIntersectPoint).normalized;
        dragPlane.SetNormalAndPosition(Vector3.up, Vector3.zero);
    }

    void UpdateZoomPlane()
    {
        Vector3 normal = (Camera.main.transform.position - targetZoomPos).normalized;
        zoomPlane.SetNormalAndPosition(normal, targetZoomPos + normal * 0.025f);
    }


    Vector3 GetGroundIntersectPoint()
    {
        float hypotenuse = Camera.main.transform.position.y / Mathf.Sin(Mathf.Deg2Rad * Camera.main.transform.eulerAngles.x);
        return Camera.main.transform.position + Camera.main.transform.forward * hypotenuse;
    }


    public bool LeftBtn => leftBtn;
    public bool LeftBtnDown => leftBtnDown;
    public bool RightBtn => rightBtn;
    public bool RightBtnDown => rightBtnDown;
    public bool IsPointerOver => isPointerOver;
    public Vector3 MousePos => Input.mousePosition;
    public Vector3 PrevMousePos => prevMousePos;

}