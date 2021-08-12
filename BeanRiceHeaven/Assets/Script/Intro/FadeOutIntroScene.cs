using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOutIntroScene : MonoBehaviour
{
    [Header("- Main")] 
    Animator introAnimator;
    private bool isFadeOutEnd = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        introAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (introAnimator.GetCurrentAnimatorStateInfo(0).IsName("7th Intro Animation") &&
            introAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            isFadeOutEnd = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetKeyDown(KeyCode.Return))
        {
            isFadeOutEnd = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (isFadeOutEnd)
        {
            SceneManager.LoadScene("LobbyScene");
        }
    }
}
