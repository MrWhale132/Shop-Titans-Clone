using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftMenu : MonoBehaviour, IStartable
{
    static CraftMenu instance;

    [SerializeField]
    BlueprintCard cardPrefab;
    [SerializeField]
    Transform cardHolder;

    Animator animator;

    public static CraftMenu Instance => instance;


    void Awake()
    {
        instance = this;
    }

    void IStartable.Start()
    {
        animator = GetComponent<Animator>();

        foreach (Item item in CraftController.Instance.ItemPrefabs)
        {
            BlueprintCard card = Instantiate(cardPrefab, cardHolder);
            card.SetUp(item);
        }

        gameObject.SetActive(false);
    }


    public void Enter()
    {
        gameObject.SetActive(true);
        animator.SetTrigger("Enter");
    }

    public void Exit()
    {
        animator.SetTrigger("Exit");
        PrimaryUI.Instance.Enter();
    }


    void OnExitAnimFinished()
    {
        gameObject.SetActive
            (false);
    }
}