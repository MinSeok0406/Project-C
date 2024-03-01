using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    public Text dialogText;

    // Start is called before the first frame update
    void Start()
    {
        dialogText.text = "";
        string sampleText = "안녕하세요, 오늘은 무엇을 사러 오셨나요?";
        StartCoroutine(Typing(sampleText));
    }

    IEnumerator Typing(string text)
    {
        foreach (char letter in text.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
