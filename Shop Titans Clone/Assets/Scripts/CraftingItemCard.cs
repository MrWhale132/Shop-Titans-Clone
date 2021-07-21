using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CraftingItemCard : MonoBehaviour
{
    [SerializeField]
    Image itemImage;
    [SerializeField]
    Image fillter;
    [SerializeField]
    Text time;

    Item itemToCraft;
    float craftingTime;

    bool done;

    public Item Item => itemToCraft;
    public float CraftingTime => craftingTime;
    public bool Done => done;

    public void SetUp(Item item)
    {
        itemToCraft = item;
        craftingTime = item.CraftTime;
        itemImage.sprite = item.Icon;
        fillter.fillAmount = 0;
        time.text = item.CraftTime.ToString();
    }

    public void Tick()
    {
        craftingTime -= Time.deltaTime;
        time.text = Mathf.CeilToInt(craftingTime).ToString();
        fillter.fillAmount = 1 - (craftingTime / itemToCraft.CraftTime);

        if (craftingTime <= 0)
        {
            done = true;
            time.text = "";
            fillter.fillAmount = 1;
        }
    }

    public void OnClick()
    {
        CraftController.Instance.CraftingCardClicked(this);
    }
}