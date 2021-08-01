using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public static class PlayerStats
{
    public static event Action<Resource.Type> NewResourceAchived;
    static Action<int>[] binfulnessChanged = new Action<int>[Enum.GetValues(typeof(Resource.Type)).Length];


    static int goldAmount;
    static Dictionary<Resource.Type, ResourceTypePropertys> resourcePropertys = new Dictionary<Resource.Type, ResourceTypePropertys>();


    public static int GoldAmount => goldAmount;


    public static void AddBinFulnessLevelIncreasedCallback(Resource.Type type, Action<int> callback) =>
                          binfulnessChanged[(int)type] += callback;

    public static void RemoveBinFulnessLevelIncreasedCallback(Resource.Type type, Action<int> callback) =>
                          binfulnessChanged[(int)type] -= callback;

    public static bool IsResourceAchived(Resource.Type type)
    {
        return resourcePropertys.ContainsKey(type);
    }

    public static bool IsResourceRegenerating(Resource.Type type)
    {
        return resourcePropertys[type].IsRegenerating;
    }

    public static float GetResourceRegenerationProgress(Resource.Type type) =>
                            resourcePropertys[type].RegenerationProgress;

    public static int GetResourceTotalQuantity(Resource.Type type) =>
                        resourcePropertys[type].TotalQuantity;

    public static int GetResourceFulnessLevel(Resource.Type type) =>
                        resourcePropertys[type].FulnessLevel;


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


    public static void ConsumeResource(Resource.Type type, int quantity)
    {
        if (resourcePropertys[type].TotalQuantity - quantity < 0)
        {
            Debug.LogError($"You can not subtract more ({quantity}) than the currently avaible quantity" +
                           $"({resourcePropertys[type].TotalQuantity}) of {type}");
            return;
        }
        resourcePropertys[type].TotalQuantity -= quantity;
        CheckRegeneration(type);
    }

    public static void IncreaseResourceCapacity(Resource.Type type, int extraCapacity)
    {
        var resource = resourcePropertys[type];
        if (resourcePropertys.ContainsKey(type) == false)
        {
            Debug.LogError("You want to increase such a resource max capacity what the player do not have.");
            return;
        }
        resource.MaxCapacity += extraCapacity;
        bool levelChanged = resource.SetFulness(out int newLevel);
        if (levelChanged)
        {
            BinController.EnqueueSetFulness(binfulnessChanged[(int)type], newLevel);
        }
        CheckRegeneration(type);
    }

    static void CheckRegeneration(Resource.Type type)
    {
        if (resourcePropertys[type].TotalQuantity < resourcePropertys[type].MaxCapacity &&
            regeneratingResources.Contains(resourcePropertys[type]) == false)
        {
            regeneratingResources.Add(resourcePropertys[type]);
            if (regeneratingResources.Count == 1)
                Task.Factory.StartNew(() => RegenerateResources(tokenSource.Token));
            resourcePropertys[type].IsRegenerating = true;
        }
    }


    public static void AddAchivedResource(Resource.Type type)
    {
        if (resourcePropertys.ContainsKey(type))
        {
            Debug.LogError("An already achived resource should not be tried to add multiple times to the list.");
            return;
        }
        resourcePropertys.Add(type, CraftController.Instance.GetResourceProperty(type));
        NewResourceAchived(type);
    }


    // Warning: this method run on a seperate thread.
    static void RegenerateResources(CancellationToken token)
    {
        while (regeneratingResources.Count > 0)
        {
            Task.Delay(regeneration_milliseconds).Wait();

            for (int i = 0; i < regeneratingResources.Count; i++)
            {
                if (token.IsCancellationRequested) return;

                var resource = regeneratingResources[i];

                bool regenerationUnitCompleted = resource.AddRegeneration(regeneration_milliseconds / 1000f);
                bool levelChanged = resource.SetFulness(out int newLevel);
                if (levelChanged)
                {
                    BinController.EnqueueSetFulness(binfulnessChanged[(int)resource.Type], newLevel);
                }
                if (regenerationUnitCompleted &&
                    resource.TotalQuantity >= resource.MaxCapacity)
                {
                    resource.IsRegenerating = false;
                    regeneratingResources.RemoveAt(i);
                    i--;
                }
            }
        }
    }
    static List<ResourceTypePropertys> regeneratingResources = new List<ResourceTypePropertys>();
    public static CancellationTokenSource tokenSource = new CancellationTokenSource();
    const int regeneration_milliseconds = 10;
}