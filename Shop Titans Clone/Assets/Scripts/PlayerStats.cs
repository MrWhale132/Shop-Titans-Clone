

using UnityEngine;

public static class PlayerStats
{
    static int goldAmount;


    public static int GoldAmount => goldAmount;
    public static void AddGold(int amount)
    {
        goldAmount += amount;
        ShopInfoMenu.GoldAmount = goldAmount;
    }
    public static void TakeGold(int amount)
    {
        goldAmount -= amount;
        if (goldAmount < 0)
            Debug.LogError("The amount of gold what the player have cant be negative!");
        ShopInfoMenu.GoldAmount = goldAmount;
    }
}