using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    Furniture.Types preferedFurnType;
    [SerializeField]
    Sprite icon;
    [SerializeField]
    string itemName;
    [SerializeField]
    int value;
    [SerializeField]
    int craftingTime;
    [SerializeField]
    Material[] recquiredMaterials;
    [SerializeField]
    bool complexModel;

    Renderer rend;
    Color originalColor;

    Renderer[] childrensRend;

    public Sprite Icon => icon;
    public string Name => itemName;
    public int Value => value;
    public int CraftTime => craftingTime;
    public Furniture.Types PreferedFurnType => preferedFurnType;
    public Material[] RecquiredMaterials => recquiredMaterials;


    void Start()
    {
        if (complexModel)
        {
            childrensRend = GetComponentsInChildren<Renderer>();
            originalColor = childrensRend[1].material.color;
        }
        else
        {
            rend = GetComponent<Renderer>();
            originalColor = rend.material.color;
        }
    }


    public void PlayFlashingAnim()
    {
        StopAllCoroutines();
        StartCoroutine(nameof(FlashingAnimation));
    }

    IEnumerator FlashingAnimation()
    {
        float fadeLength = 0.35f;
        float fadeFactor = 0.6f;
        Color fadeColor = new Color(fadeFactor, fadeFactor, fadeFactor);

        float timer = 0;
        while (timer < fadeLength)
        {
            timer += Time.deltaTime;
            ChangeColor(Color.Lerp(originalColor, fadeColor, timer / fadeLength));
            yield return null;
        }
        timer = 0;
        while (timer < fadeLength)
        {
            timer += Time.deltaTime;
            ChangeColor(Color.Lerp(fadeColor, originalColor, timer / fadeLength));
            yield return null;
        }
        timer = 0;
        while (timer < fadeLength)
        {
            timer += Time.deltaTime;
            ChangeColor(Color.Lerp(originalColor, fadeColor, timer / fadeLength));
            yield return null;
        }
        timer = 0;
        while (timer < fadeLength)
        {
            timer += Time.deltaTime;
            ChangeColor(Color.Lerp(fadeColor, originalColor, timer / fadeLength));
            yield return null;
        }
        ChangeColor(originalColor);
    }

    void ChangeColor(Color newColor)
    {
        if (complexModel)
        {
            foreach (var rend in childrensRend)
            {
                rend.material.color = newColor;
            }
        }
        else rend.material.color = newColor;
    }
}