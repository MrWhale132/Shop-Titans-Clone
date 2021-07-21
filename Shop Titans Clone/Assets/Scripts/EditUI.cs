using UnityEngine;

public class EditUI : MonoBehaviour, IStartable, IOverlayMenu
{
    static EditUI instance;

    PrimaryUI primaryUI;
    BuildMenu buildMenu;

    Animator animator;

    public static EditUI Instance => instance;


    void Awake()
    {
        instance = this;
    }

    void IStartable.Start()
    {
        animator = GetComponent<Animator>();
        primaryUI = PrimaryUI.Instance;
        buildMenu = BuildMenu.Instance;

        gameObject.SetActive(false);
    }


    public void Enter()
    {
        gameObject.SetActive(true);
        animator.SetTrigger("Enter");
        OverlayMenuController.CurrentMenu = this;
    }

    public void EnterBuildMenu()
    {
        animator.SetTrigger("Exit");
        buildMenu.Enter();
    }

    public void Back()
    {
        Exit();
        primaryUI.Enter();
    }

    public void Exit()
    {
        animator.SetTrigger("Exit");
    }


    void ExitAnimFinished()
    {
        gameObject.SetActive(false);
    }
}