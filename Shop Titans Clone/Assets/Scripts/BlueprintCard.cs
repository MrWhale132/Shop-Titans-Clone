using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlueprintCard : MonoBehaviour
{
    [SerializeField]
    Image itemImage;
    [SerializeField]
    Text itemName;
    [SerializeField]
    Transform componentsHolder;

    List<BPComponent> components = new List<BPComponent>();

    Item prefab;

    public Item Item => prefab;


    void Update()
    {
        for (int i = 0; i < components.Count; i++)
        {
            if (PlayerStats.IsResourceAchived(Item.RequiredResources[i].ResourceType))
                components[i].SetBaseResourceState(Item.RequiredResources[i].Quantity <= PlayerStats.GetResourceTotalQuantity(Item.RequiredResources[i].ResourceType));
            else components[i].SetBaseResourceState(false);
        }
    }


    public void SetUp(Item item)
    {
        prefab = item;
        itemImage.sprite = item.Icon;
        itemName.text = item.Name;

        for (int i = 0; i < componentsHolder.childCount; i++)
        {
            Destroy(componentsHolder.GetChild(i).gameObject);
        }
        componentsHolder.DetachChildren();

        foreach (Resource resource in item.RequiredResources)
        {
            BPComponent component = Instantiate(CraftController.ComponentPrefab, componentsHolder);
            component.SetUp(resource);
            components.Add(component);
        }
    }

    public void OnClick()
    {
        CraftController.Instance.BlueprintCardClicked(this);
    }
}