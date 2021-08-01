using System;
using UnityEngine;

[Serializable]
public class ResourceTypePropertys
{
    [SerializeField]
    Resource.Type type;
    [SerializeField]
    int totalQuantity;
    [SerializeField]
    int maxCapacity;
    [SerializeField]
    int regenerationQuantity;
    [SerializeField]
    float regenerationTime;
    [SerializeField]
    int defaultRegenerationQuantity;
    [SerializeField]
    float defaultRegenrationTime;
    [SerializeField]
    int[] binFulnessPercentages;

    float regenarationProgress;
    bool isRegenerating;
    int fulnessIndex;

    public ResourceTypePropertys(Resource.Type type)
    {
        this.type = type;
    }

    public Resource.Type Type { get => type; set => type = value; }
    public int TotalQuantity { get => totalQuantity; set => totalQuantity = value; }
    public int MaxCapacity { get => maxCapacity; set => maxCapacity = value; }
    public int RegenerationQuantity { get => regenerationQuantity; set => regenerationQuantity = value; }
    public float RegenerationTime { get => regenerationTime; set => regenerationTime = value; }
    public bool IsRegenerating { get => isRegenerating; set => isRegenerating = value; }
    public float RegenerationProgress => regenarationProgress;
    public int FulnessLevel { get => fulnessIndex; set => fulnessIndex = value; }

    public bool AddRegeneration(float time)
    {
        regenarationProgress += time / regenerationTime;
        if (regenarationProgress >= 1)
        {
            regenarationProgress = 0;
            totalQuantity += regenerationQuantity;
            return true;
        }
        return false;
    }

    public bool SetFulness(out int newLevel)
    {
        int originalLevel = fulnessIndex;
        float fulness = totalQuantity / (float)maxCapacity * 100;
        while (fulness < binFulnessPercentages[fulnessIndex])
        {
            fulnessIndex--;
        }
        while (fulnessIndex + 1 < binFulnessPercentages.Length &&
               fulness >= binFulnessPercentages[fulnessIndex + 1])
        {
            fulnessIndex++;
        }
        newLevel = fulnessIndex;
        return originalLevel != newLevel;
    }
}