using UnityEngine;
using UnityEngine.UI;

public class ResourceStat : MonoBehaviour
{
    [SerializeField]
    Image resourceImage;
    [SerializeField]
    Image loadBar;
    [SerializeField]
    Text quantity;

    Resource.Type type;

    public Resource.Type Type => type;
    public int Quantity { set => quantity.text = value.ToString(); }
    public float LoadPercent { set => loadBar.fillAmount = value; }

    public void SetType(Resource.Type type)
    {
        this.type = type;
        resourceImage.sprite = Resource.GetIcon(type);
    }
}