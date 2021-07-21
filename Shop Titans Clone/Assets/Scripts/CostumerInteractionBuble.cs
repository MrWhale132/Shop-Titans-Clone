using UnityEngine;
using UnityEngine.UI;

public class CostumerInteractionBuble : MonoBehaviour
{
    [SerializeField]
    Image iconImage;


    NPC owner;

    public NPC Owner { get => owner; set => owner = value; }



    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
    }


    public void OnClick()
    {
        CostumerInteractionsMenu.Instance.CostumerBubleClicked(this);

        
    }


    public void SetIcon(Sprite icon)
    {
        iconImage.sprite = icon;
    }
}