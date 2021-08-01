using UnityEngine;
using UnityEngine.UI;

public class BPComponent : MonoBehaviour
{
    [SerializeField]
    Image materialImage;
    [SerializeField]
    Text requiredAmount;
    [SerializeField]
    Text slash;
    [SerializeField]
    Text currentAmount;


    public void SetUp(Resource resource)
    {
        materialImage.sprite = resource.GetIcon();
        requiredAmount.text = resource.Quantity.ToString();
        if (PlayerStats.IsResourceAchived(resource.ResourceType))
        {
            currentAmount.text = PlayerStats.GetResourceTotalQuantity(resource.ResourceType).ToString();
            // TODO: find a proper condition to know when to ignore these.
            slash.gameObject.SetActive(false);
            currentAmount.gameObject.SetActive(false);
        }
    }

    public void SetBaseResourceState(bool isThereEnough)
    {
        if (isThereEnough)
        {
            requiredAmount.color = Color.white;
        }
        else requiredAmount.color = Color.red;
    }
}