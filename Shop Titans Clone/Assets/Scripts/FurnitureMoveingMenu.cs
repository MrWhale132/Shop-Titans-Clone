using UnityEngine;

public class FurnitureMoveingMenu : MonoBehaviour, IStartable
{
    static FurnitureMoveingMenu instance;

    Animator animator;

    public static FurnitureMoveingMenu Instance => instance;

    public static bool Moveing => instance.gameObject.activeInHierarchy;

    public static Fitment ObjectToMove {
        get => BuildController.Instance.ToMoveFitment;
        set => BuildController.Instance.ToMoveFitment = value;
    }


    void Awake()
    {
        instance = this;
    }

    void IStartable.Start()
    {
        animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }


    public  void Rotate(int degree)
    {
        ObjectToMove.Rotate(degree);
    }

    public void Enter()
    {
        gameObject.SetActive(true);
        animator.SetTrigger("Enter");
        GridController.SetActiveBuildGrid(true);
        MouseController.Instace.AddControl(MouseController.UsageMode.DragLeft, BuildController.Instance.DragFitment);
    }

    public void Back()
    {
        animator.SetTrigger("Exit");
        PrimaryUI.Instance.Enter();
        GridController.SetActiveBuildGrid(false);   
        ObjectToMove?.ValidatePosition();
        MouseController.Instace.RemoveControl(MouseController.UsageMode.DragLeft, BuildController.Instance.DragFitment);
    }

    void OnExitAnimFinished()
    {
        gameObject.SetActive(false);
    }
}