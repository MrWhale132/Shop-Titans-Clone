using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CraftController : MonoBehaviour
{
    static CraftController instance;

    List<Item> itemPrefabs = new List<Item>();
    [SerializeField]
    BPComponent componentPrefab;
    [SerializeField]
    int craftQueueSize;
    [SerializeField]
    CraftingItemCard craftingCardPrefab;
    [SerializeField]
    Transform cardHolder;
    [SerializeField]
    TreeNode itemPrefabTree;


    List<CraftingItemCard> craftQueue = new List<CraftingItemCard>();

    public static CraftController Instance => instance;
    public static BPComponent ComponentPrefab => instance.componentPrefab;
    public List<Item> ItemPrefabs => itemPrefabs;


    void Awake()
    {
        instance = this;
        LoadAssets(itemPrefabTree);
    }


    public void BlueprintCardClicked(BlueprintCard card)
    {
        if (craftQueue.Count == craftQueueSize)
        {
            MessageDisplayer.DisplayMessage("The crafting queue is full.");
            return;
        }
        var craftingCard = Instantiate(craftingCardPrefab, cardHolder);
        craftingCard.SetUp(card.Item);
        PlaceInQueue(craftingCard);
        instance.StartCoroutine(nameof(CraftItem), craftingCard);
    }

    IEnumerator CraftItem(CraftingItemCard card)
    {
        while (card.Done == false)
        {
            card.Tick();
            yield return null;
        }
    }

    void PlaceInQueue(CraftingItemCard card)
    {
        int i = 0;
        while (i < craftQueue.Count && card.Item.CraftTime > craftQueue[i].CraftingTime)
        {
            i++;
        }
        craftQueue.Insert(i, card);
        card.transform.SetSiblingIndex(i);
    }

    public void CraftingCardClicked(CraftingItemCard card)
    {
        var validFurn = Furniture.Furnitures.FirstOrDefault(furn => furn.Type == card.Item.PreferedFurnType && furn.IsFull == false);
        if (validFurn == null)
        {
            MessageDisplayer.DisplayMessage("There is no valid furniture for this item.");
            return;
        }
        Item craftedItem = Instantiate(card.Item);
        validFurn.AddItem(craftedItem);
        craftQueue.Remove(card);
        Destroy(card.gameObject);
    }



    void LoadAssets(TreeNode parent)
    {
        if (parent.IsLeaf)
        {
            itemPrefabs.AddRange(Resources.LoadAll<Item>(path.ToString() + parent.FileName));
            return;
        }
        path.Append(parent.FileName);
        foreach (TreeNode child in parent)
        {
            LoadAssets(child);
        }
        path.Remove(path.Length - parent.FileName.Length, parent.FileName.Length - 1);
    }
    StringBuilder path = new StringBuilder();
}