using System.Collections;
using System.Collections.Generic;

public static class IListExtensions
{
    public static void SwapLast(this IList list, int index)
    {
        object temp = list[index];
        list[index] = list[list.Count - 1];
        list[list.Count - 1] = temp;
    }

    public static void SwapLastAndRemove(this IList list, int index)
    {
        list.SwapLast(index);
        list.RemoveAt(list.Count - 1);
    }

    //public static void SwapLast<T>(this IList<T> list, int index)
    //{
    //    T temp = list[index];
    //    list[index] = list[list.Count - 1];
    //    list[list.Count - 1] = temp;
    //}
}