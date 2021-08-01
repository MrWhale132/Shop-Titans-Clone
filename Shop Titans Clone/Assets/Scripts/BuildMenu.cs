using UnityEngine;
using UnityEngine.UI;

public class BuildMenu : MonoBehaviour, IStartable
{
    static BuildMenu instance;

    [SerializeField]
    FitmentBlueprint furnCardPrefab;
    [SerializeField]
    Transform holder;

    EditUI editUI;
    ConstructionMenu constructionMenu;

    Animator animator;
    MouseController mouseC;
    BuildController buildC;

    public static BuildMenu Instance => instance;
    public FitmentBlueprint FurniturePortraitCardPrefab => furnCardPrefab;
    public Transform FurnCardsHolder => holder;


    void Awake()
    {
        instance = this;
    }

    void IStartable.Start()
    {
        animator = GetComponent<Animator>();
        mouseC = MouseController.Instace;
        buildC = BuildController.Instance;
        editUI = EditUI.Instance;
        constructionMenu = ConstructionMenu.Instance;

        foreach (var furn in buildC.FitmentPrefabs)
        {
            var card = Instantiate(furnCardPrefab, holder);
            card.GetComponent<Button>().onClick.AddListener(() => FurnitureSelected(card));
            card.SetFitment(furn);
        }

        gameObject.SetActive(false);
    }

    void FurnitureSelected(FitmentBlueprint card)
    {
        animator.SetTrigger("Exit");
        GridController.SetActiveBuildGrid(true);

        buildC.ToBuildFitment = Instantiate(card.Prefab, new Vector3(10, 0, 5), Quaternion.identity);
        buildC.ToMoveFitment = buildC.ToBuildFitment;
        buildC.ToBuildFitment.SetPosition(new Vector3Int(10, 0, 5));
        mouseC.AddControl(MouseController.UsageMode.DragLeft, buildC.DragFitment);

        constructionMenu.Enter();
    }





    public void Enter()
    {
        gameObject.SetActive(true);
        animator.SetTrigger("Enter");
    }

    public void BackToEditUI()
    {
        animator.SetTrigger("Exit");
        editUI.Enter();
    }


    void ExitAnimFinished()
    {
        gameObject.SetActive(false);
    }
}