using UnityEngine;

public class FurnitureEditor : MonoBehaviour, IStartable
{
    static FurnitureEditor instance;

    Animator animator;

    public static FurnitureEditor Instance => instance;


    void Awake()
    {
        instance = this;
    }

    void IStartable.Start()
    {
        animator = GetComponent<Animator>();

        gameObject.SetActive(false);
    }


    public void Move()
    {
        animator.SetTrigger("Exit");
        FurnitureMoveingMenu.Instance.Enter();
    }


    public void Enter()
    {
        gameObject.SetActive(true);
        animator.SetTrigger("Enter");
    }

    public void Exit()
    {
        animator.SetTrigger("Exit");
        OverlayMenuController.CurrentMenu.Enter();
    }

    void ExitAnimFinished()
    {
        gameObject.SetActive(false);
    }
}