using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypingTextEffect : MonoBehaviour
{
    public InputField nickname;
    private Text effectText;
    private string message = "WELCOME to BeanRice Heaven";
    
    // Start is called before the first frame update
    void Start()
    {
        effectText = GetComponent<Text>();
        StartCoroutine(TypingText());
        message = nickname.text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator TypingText()
    {
        yield return new WaitForSeconds(0.8f);
        for (int i = 0; i <= message.Length; ++i)
        {
            effectText.text = message.Substring(0, i);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
