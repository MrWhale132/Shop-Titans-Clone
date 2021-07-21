
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TreeNode : IEnumerable
{
    [UnityEngine.SerializeField]
    string fileName;
    [UnityEngine.SerializeField]
    TreeNode[] children;

    public bool IsLeaf => children.Length == 0;
    public TreeNode[] Children => children;
    public string FileName => fileName;

    public IEnumerator GetEnumerator()
    {
        for (int i = 0; i < children.Length; i++)
        {
            yield return children[i];
        }
    }
}