using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MessageDisplayer : MonoBehaviour
{
    static MessageDisplayer instance;

    [SerializeField]
    Text generalMsg;
    [SerializeField]
    float displayTime;
    Color originalColor;


    void Awake()
    {
        instance = this;
        originalColor = generalMsg.color;
    }


    public static void DisplayMessage(string msg)
    {
        instance.generalMsg.text = msg;
        instance.StopAllCoroutines();
        instance.StartCoroutine(nameof(CountDown), instance.displayTime);
    }

    IEnumerator CountDown(float time)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        time = 1;
        while (time > 0)
        {
            time -= Time.deltaTime;
            generalMsg.color = new Color(originalColor.r, originalColor.g, originalColor.b, time);
            yield return null;
        }
        generalMsg.color = originalColor;
        generalMsg.text = "";
    }
}