
public class GridNodeHeap
{
    GridNode[] items;
    int currentItemCount;

    public GridNodeHeap(int maxHeapSize)
    {
        items = new GridNode[maxHeapSize];
    }

    public void Add(GridNode item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public GridNode RemoveFirst()
    {
        GridNode firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    public void UpdateItem(GridNode item)
    {
        SortUp(item);
    }

    public int Count {
        get
        {
            return currentItemCount;
        }
    }

    public bool Contains(GridNode item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    void SortDown(GridNode item)
    {
        while (true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex;

            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                        swapIndex = childIndexRight;
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                    Swap(item, items[swapIndex]);
                else
                    return;
            }
            else
                return;
        }
    }

    void SortUp(GridNode item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            GridNode parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
                break;

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    void Swap(GridNode itemA, GridNode itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}