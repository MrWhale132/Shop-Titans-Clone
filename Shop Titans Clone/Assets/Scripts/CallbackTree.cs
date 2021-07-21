using System.Collections.Generic;
using System;
using UnityEngine;

public class CallbackTree<T> where T : Delegate
{
    List<T> tree;
    List<Dictionary<string, T>> memory;

    public int Length => tree.Count;
    T TreeTop { get => tree[tree.Count -1]; set => tree[tree.Count - 1] = value; }
    Dictionary<string, T> MemoryTop { get => memory[memory.Count -1]; set => memory[memory.Count - 1] = value; }


    public CallbackTree()
    {
        tree = new List<T>();
        memory = new List<Dictionary<string, T>>();
    }


    public void Stack(T callback)
    {
        tree.Add(callback);
        memory.Add(new Dictionary<string, T>());
    }

    public T Take()
    {
        memory.RemoveAt(memory.Count - 1);
        T temp = TreeTop;
        tree.RemoveAt(tree.Count - 1);
        return temp;
    }

    public void Add(T callback)
    {
        TreeTop = (T)Delegate.Combine(TreeTop, callback);
    }

    public void Add(T callback, string memoryKey)
    {
        Add(callback);
        if (MemoryTop.ContainsKey(memoryKey))
            Debug.LogError("You try to add the same callback twice to the memory");
        MemoryTop.Add(memoryKey, callback);
    }

    public bool Remove(T callback)
    {
        int before = TreeTop.GetInvocationList().Length;
        TreeTop = (T)Delegate.Remove(TreeTop, callback);
        if (TreeTop.GetInvocationList().Length == before)
            return false;
        return true;
    }

    public bool Remove(string memoryKey)
    {
        if (MemoryTop.ContainsKey(memoryKey) == false) // later should Debug.LogError
            return false;
        if (Remove(MemoryTop[memoryKey]) == false)
        {
            Debug.LogError("Fatal exception: The memory contain a key what is point to a callback which not repreresent in the tree!");
            return false;
        }
        MemoryTop.Remove(memoryKey);
        return true;
    }

    public T Last()
    {
        return TreeTop;
    }
}