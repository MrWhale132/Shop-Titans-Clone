using System;
using System.Text;
using UnityEngine;

public class GameSetUp : MonoBehaviour
{
    [SerializeField]
    GameObject[] startables;
    [SerializeField]
    TreeNode resourcesTree;

    void Start()
    {
        LoadAssets(resourcesTree);
        // Awake
        foreach (GameObject go in startables)
        {
            go.SetActive(true);
        }
        //Start
        for (int i = startables.Length - 1; i >= 0; i--)
        {
            startables[i].GetComponent<IStartable>().Start();
        }
        // beacuse there is no file to load from
        ShopInfoMenu.GoldAmount = 0;
    }




    void LoadAssets(TreeNode parent)
    {
        if (parent.IsLeaf)
        {
            Type type = Type.GetType(parent.FileName);
            type.GetMethod("LoadAssets").Invoke(null, new[] { path.ToString() + parent.FileName });
            return;
        }
        path.Append(parent.FileName);
        foreach (TreeNode child in parent)
        {
            LoadAssets(child);
        }
        path.Remove(path.Length - parent.FileName.Length, parent.FileName.Length - 1);
    }
    StringBuilder path = new StringBuilder();
}