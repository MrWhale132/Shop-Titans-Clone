using UnityEngine;
using UnityEngine.UI;

public class FitmentBlueprint : MonoBehaviour
{
    [SerializeField]
    Image furnImage;
    [SerializeField]
    Text furnName;

    Fitment prefab;

    public Fitment Prefab => prefab;

    public void SetFitment(Fitment prefab)
    {
        this.prefab = prefab;
        furnImage.sprite = prefab.Portrait;
        furnName.text = prefab.Name;
    }
}