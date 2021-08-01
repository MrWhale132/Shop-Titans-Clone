using UnityEngine;

public class ConstructionMenu : MonoBehaviour, IStartable
{
    static ConstructionMenu instance;

    Animator animator;
    PrimaryUI primaryUI;
    BuildController buildC;

    public static ConstructionMenu Instance => instance;
    public static bool Constructing => instance.gameObject.activeInHierarchy;


    void Awake()
    {
        instance = this;
    }

    void IStartable.Start()
    {
        animator = GetComponent<Animator>();
        primaryUI = PrimaryUI.Instance;
        buildC = BuildController.Instance;

        gameObject.SetActive(false);
    }


    public void BuildFurniture()
    {
        if (PlayerStats.GoldAmount < buildC.ToBuildFitment.BuildCost)
        {
            MessageDisplayer.DisplayMessage("You do not have enough gold to build this furniture.");
            return;
        }
        if (buildC.ToBuildFitment.HasValidPosition() == false)
        {
            MessageDisplayer.DisplayMessage("The position of your furniture is invalid.");
            return;
        }
        PlayerStats.TakeGold(buildC.ToBuildFitment.BuildCost);
        buildC.BuildMoveable();
        ExitToPrimaryUI();
    }

    public void CancelConstruction()
    {
        buildC.CancelConstruction();
        ExitToPrimaryUI();
    }

    public void RotateFurniture(int degres)
    {
        buildC.ToMoveFitment.Rotate(degres);
    }


    public void Enter()
    {
        gameObject.SetActive(true);
        animator.SetTrigger("Enter");
    }

    public void ExitToPrimaryUI()
    {
        animator.SetTrigger("Exit");
        primaryUI.Enter();
    }


    void ExitAnimFinished()
    {
        gameObject.SetActive(false);
    }
}