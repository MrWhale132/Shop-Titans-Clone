using UnityEngine;
using UnityEngine.UI;

public class ShopInfoMenu : MonoBehaviour, IStartable
{
    static ShopInfoMenu instance;

    [SerializeField]
    Text goldAmount;


    public static int GoldAmount { private get => -1; set => instance.goldAmount.text = value.ToString(); }


    void Awake()
    {
        instance = this;
    }

    void IStartable.Start()
    {
        
    }
}