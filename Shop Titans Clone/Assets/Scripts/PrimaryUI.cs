using UnityEngine;
using System.Collections;

public class PrimaryUI : MonoBehaviour, IStartable, IOverlayMenu
{
    static PrimaryUI instance;
    
    EditUI editUI;

    Animator animator;

    public static PrimaryUI Instance => instance;


    void Awake()
    {
        instance = this;
    }

    void IStartable.Start()
    {
        animator = GetComponent<Animator>();
        editUI = EditUI.Instance;
        OverlayMenuController.CurrentMenu = this;
    }


    public void Enter()
    {
        gameObject.SetActive(true);
        animator.SetTrigger("Enter");
        OverlayMenuController.CurrentMenu = this;
    }

    public void Exit()
    {
        animator.SetTrigger("Exit");
    }

    public void EnterEditUI()
    {
        animator.SetTrigger("Exit");
        editUI.Enter();
    }

    public void EnterCraftMenu()
    {
        animator.SetTrigger("Exit");
        CraftMenu.Instance.Enter();
    }


    void ExitAnimFinished()
    {
        gameObject.SetActive(false);
    }
}