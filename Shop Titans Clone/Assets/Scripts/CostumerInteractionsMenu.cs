using UnityEngine;

public class CostumerInteractionsMenu : MonoBehaviour, IStartable
{
    static CostumerInteractionsMenu instance;

    [SerializeField]
    CostumerInteractionBuble bublePrefab;
    Animator animator;
    NPC selectedCostumer;

    Vector3 originalCamPos;
    Vector3 originalLookPoint;

    public static CostumerInteractionsMenu Instance => instance;
    public CostumerInteractionBuble BublePrefab => bublePrefab;


    void Awake()
    {
        instance = this;
    }

    void IStartable.Start()
    {
        animator = GetComponent<Animator>();

        gameObject.SetActive(false);
    }


    public void Sell()
    {
        selectedCostumer.LeaveShop(NPCController.Instance.NPC_LeftTheShop);
        PlayerStats.AddGold(selectedCostumer.ItemToBuy.Value);
        selectedCostumer.TakeItem();

        NextOrExit();
    }

    public void Refuse()
    {
        selectedCostumer.ItemToBuy = null;
        selectedCostumer.LeaveShop(NPCController.Instance.NPC_LeftTheShop);

        NextOrExit();
    }

    public void Wait()
    {
        Exit();
    }


    public void CostumerBubleClicked(CostumerInteractionBuble buble)
    {
        if (gameObject.activeInHierarchy == false)
        {
            gameObject.SetActive(true);
            animator.SetTrigger("Enter");
            PrimaryUI.Instance.Exit();

            originalCamPos = Camera.main.transform.position;
            originalLookPoint = originalCamPos + Camera.main.transform.forward * 100000;
        }

        ConsumeNextCostumer(buble.Owner);
    }


    void NextOrExit()
    {
        if (Countier.IsWaiting)
        {
            ConsumeNextCostumer(Countier.GetNextCostumer());
        }
        else Exit();
    }

    void ConsumeNextCostumer(NPC costumer)
    {
        selectedCostumer = costumer;
        Vector3 forward = Vector3.ProjectOnPlane(costumer.Buble.transform.forward, Vector3.up).normalized;
        Vector3 targetPos = costumer.Buble.transform.position + forward * 3.5f + Vector3.up * 3;
        MouseController.Instace.ZoomToCostumer(targetPos, costumer.Buble.transform.position);
    }


    public void Exit()
    {
        selectedCostumer = null;
        animator.SetTrigger("Exit");
        PrimaryUI.Instance.Enter();
        MouseController.Instace.ZoomToCostumer(originalCamPos, originalLookPoint);
    }

    void ExitAnimFinished()
    {
        gameObject.SetActive(false);
    }
}