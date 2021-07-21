using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Furniture : MoveableObject
{
    public enum Types { Table, Rack, Shelf, Manique }

    [SerializeField]
    Types type;
    [SerializeField]
    Transform itemParent;
    [SerializeField]
    Transform[] itemPlaces;

    // Furnitures of the shop.
    static List<Furniture> furnitures = new List<Furniture>();
    
    Item[] itemSlots;

    public static List<Furniture> Furnitures => furnitures;
    public int Capacity => itemPlaces.Length;
    public bool IsFull => !itemSlots.Any(item => item == null);
    public bool IsEmpty => !itemSlots.Any(item => item != null);
    public Types Type => type;


    protected override void Awake()
    {
        base.Awake();

        furnitures.Add(this);

        itemSlots = new Item[itemPlaces.Length];
    }


    public Item GetRandomItem()
    {
        var items = itemSlots.Where(item => item != null);
        return items.ElementAt(new System.Random().Next(0, items.Count()));
    }

    public void AddItem(Item item)
    {
        if (IsFull)
        {
            Debug.LogError("The furniture can not accept more item.");
            return;
        }
        int at = Array.IndexOf(itemSlots, null);

        itemSlots[at] = item;
        item.transform.SetParent(itemParent);
        item.transform.position = itemPlaces[at].position + Vector3.up * item.transform.localScale.y / 2;
    }

    public void RemoveItem(Item item)
    {
        if (IsEmpty)
        {
            Debug.LogError("There is no item what you could remove");
            return;
        }
        if (itemSlots.Contains(item) == false)
        {
            Debug.LogError("You want to remove an item which the furniture do not have.");
            return;
        }

        itemSlots[Array.IndexOf(itemSlots, item)] = null;
    }


    public GridNode GetRandomVisitableGridNode()
    {
        var nodes = GetVisitableGridNodes();
        return nodes[new System.Random().Next(0, nodes.Count)];
    }


    public override void RemoveFromShop()
    {
        base.RemoveFromShop();
        furnitures.Remove(this);
    }

    public override void OnClicked()
    {
        base.OnClicked();
    }

    protected override List<GridNode> GetVisitableGridNodes()
    {
        return base.GetVisitableGridNodes();
    }
}