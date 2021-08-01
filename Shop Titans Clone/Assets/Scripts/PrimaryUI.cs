using UnityEngine;
using System.Collections.Generic;
using static PlayerStats;

public class PrimaryUI : MonoBehaviour, IStartable, IOverlayMenu
{
    static PrimaryUI instance;

    [SerializeField]
    Transform resourceStatHolder;
    [SerializeField]
    ResourceStat resourceStatPrefab;

    EditUI editUI;

    Animator animator;

    List<ResourceStat> resourceStats = new List<ResourceStat>();

    public static PrimaryUI Instance => instance;


    void Awake()
    {
        instance = this;
        NewResourceAchived += OnNewResourceAchived;
    }


    void IStartable.Start()
    {
        animator = GetComponent<Animator>();
        editUI = EditUI.Instance;
        OverlayMenuController.CurrentMenu = this;
    }


    void Update()
    {
        foreach (var stat in resourceStats)
        {
            stat.Quantity = GetResourceTotalQuantity(stat.Type);
            if (IsResourceRegenerating(stat.Type))
            {
                stat.LoadPercent = GetResourceRegenerationProgress(stat.Type);
            }
            else
            {
                stat.LoadPercent = 0;
            }
        }
    }


    void OnNewResourceAchived(Resource.Type type)
    {
        var stat = Instantiate(resourceStatPrefab, resourceStatHolder);
        stat.transform.SetSiblingIndex((int)type);
        stat.SetType(type);
        stat.Quantity = 0;
        stat.LoadPercent = 0;
        resourceStats.Add(stat);
    }


    void OnDestroy()
    {
        NewResourceAchived -= OnNewResourceAchived;
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