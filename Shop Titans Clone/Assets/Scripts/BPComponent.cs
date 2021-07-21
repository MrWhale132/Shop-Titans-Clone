using UnityEngine;
using UnityEngine.UI;

public class BPComponent : MonoBehaviour
{
    [SerializeField]
    Image materialImage;
    [SerializeField]
    Text recquiredAmount;
    [SerializeField]
    Text currentAmount;

    public void SetUp(Sprite materialSprite,int recquiredQuantity)
    {
        materialImage.sprite = materialSprite;
        recquiredAmount.text = recquiredQuantity.ToString();
        currentAmount.text = "10";
    }
}