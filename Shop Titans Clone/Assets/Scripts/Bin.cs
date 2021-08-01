using UnityEngine;

public class Bin : Fitment
{
    [SerializeField]
    Resource.Type resourceType;
    [SerializeField]
    int startCapacity;
    [SerializeField]
    GameObject[] fullfilnessPrefabs;

    GameObject currentfulness;

    public int Capacity => startCapacity;

    public override void DoPaperWork()
    {
        base.DoPaperWork();
        if (PlayerStats.IsResourceAchived(resourceType) == false)
        {
            PlayerStats.AddAchivedResource(resourceType);
        }
        PlayerStats.AddBinFulnessLevelIncreasedCallback(resourceType, SetFulnessLevel);
        PlayerStats.IncreaseResourceCapacity(resourceType, Capacity);
    }

    void SetFulnessLevel(int level)
    {
        if (currentfulness != null)
            Destroy(currentfulness);
        if (level != 0)
        {
            currentfulness = Instantiate(fullfilnessPrefabs[level], transform.position, transform.rotation);
            currentfulness.transform.parent = transform;
        }
    }
}