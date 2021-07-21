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

    Item prefab;

    public Item Item => prefab;


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

        foreach (Material material in item.RecquiredMaterials)
        {
            BPComponent component = Instantiate(CraftController.ComponentPrefab, componentsHolder);
            component.SetUp(Material.GetIcon(material.MaterialType), material.Quantity);

        }
    }

    public void OnClick()
    {
        CraftController.Instance.BlueprintCardClicked(this);
    }
}