using UnityEngine;
using UnityEngine.UI;

public class BuildMenu : MonoBehaviour, IStartable
{
    static BuildMenu instance;

    [SerializeField]
    FurniturePortraitCard furnCardPrefab;
    [SerializeField]
    Transform holder;

    EditUI editUI;
    ConstructionMenu constructionMenu;

    Animator animator;
    MouseController mouseC;
    BuildController buildC;

    public static BuildMenu Instance => instance;
    public FurniturePortraitCard FurniturePortraitCardPrefab => furnCardPrefab;
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

        foreach (var furn in buildC.FurnPrefabs)
        {
            var card = Instantiate(furnCardPrefab, holder);
            card.GetComponent<Button>().onClick.AddListener(() => FurnitureSelected(card));
            card.SetFurniture(furn);
        }

        gameObject.SetActive(false);
    }

    void FurnitureSelected(FurniturePortraitCard card)
    {
        animator.SetTrigger("Exit");
        GridController.SetActiveBuildGrid(true);

        buildC.ToBuildFurn = Instantiate(card.Prefab, new Vector3(10.5f, 0, 5), Quaternion.identity);
        buildC.ToMoveFurn = buildC.ToBuildFurn;
        mouseC.AddControl(MouseController.UsageMode.DragLeft, buildC.DragFurn);

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