using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Material
{
    public enum Type { Wood, Stone }

    static Sprite[] icons;

    [SerializeField]
    Type type;
    [SerializeField]
    int quantity;

    public Type MaterialType => type;
    public int Quantity { get => quantity; set => quantity = value; }


    public static void LoadAssets(string path)
    {
        List<Sprite> sprites = Resources.LoadAll<Sprite>(path).ToList();
        List<string> names = Enum.GetNames(typeof(Type)).ToList();
        Dictionary<string, int> indexes = new Dictionary<string, int>();
        for (int i = 0; i < names.Count; i++)
        {
            indexes.Add(names[i], i);
        }
        icons = new Sprite[sprites.Count];
        for (int i = sprites.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < names.Count; j++)
            {
                if (sprites[i].name.Contains(names[j]))
                {
                    icons[indexes[names[j]]] = sprites[i];
                    sprites.SwapLastAndRemove(i);
                    names.SwapLastAndRemove(j);
                    break;
                }
            }
        }
        if (sprites.Count > 0)
        {
            Debug.LogError("There are loaded material icons whos do not have a material enum member!");
        }
        if (names.Count > 0)
        {
            Debug.LogError("There are enum members whos do not have an icon to represent!");
        }
    }
    public Sprite GetIcon()
    {
        return icons[(int)type];
    }

    public static Sprite GetIcon(Type materialType)
    {
        return icons[(int)materialType];
    }
}