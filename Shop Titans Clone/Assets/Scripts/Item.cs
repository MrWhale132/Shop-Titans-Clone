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
    Vector3 preferedOffset;
    [SerializeField]
    Vector3 preferedRotOnFurn;
    [SerializeField]
    Renderer[] renderers;

    Color originalColor;


    public Sprite Icon => icon;
    public string Name => itemName;
    public int Value => value;
    public int CraftTime => craftingTime;
    public Furniture.Types PreferedFurnType => preferedFurnType;
    public Material[] RecquiredMaterials => recquiredMaterials;



    public void SetUpDisplayPosition(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        transform.Translate(preferedOffset, Space.Self);
        transform.Rotate(preferedRotOnFurn, Space.Self);
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
        foreach (var rend in renderers)
        {
            rend.material.color = newColor;
        }
    }
}