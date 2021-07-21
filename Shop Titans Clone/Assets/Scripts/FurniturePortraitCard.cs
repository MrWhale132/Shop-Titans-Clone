using UnityEngine;
using UnityEngine.UI;

public class FurniturePortraitCard : MonoBehaviour
{
    [SerializeField]
    Image furnImage;
    [SerializeField]
    Text furnName;

    Furniture prefab;

    public Furniture Prefab => prefab;

    public void SetFurniture(Furniture prefab)
    {
        this.prefab = prefab;
        furnImage.sprite = prefab.Portrait;
        furnName.text = prefab.Name;
    }
}