using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuController : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject mainMenu = null;
    [SerializeField] private GameObject settings = null;

    [Header("Fade Properties")]
    [SerializeField] private Image fadeInOut = null;
    [SerializeField] private Animator fadeAnim = null;

    // Start is called before the first frame update
    void Start()
    {
        fadeInOut.gameObject.SetActive(true); //used to clean the screen in the editor
        fadeAnim.SetTrigger("fadeIn");

        MainMenu();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    /// <summary>
    /// Function that starts the game
    /// Used by the Start button
    /// </summary>
    public void StartGame()
    {
        StartCoroutine(Fading()); //Start the fade out before change sceane
    }

    // Active the main menu screen and deactive the others
    public void MainMenu()
    {
        mainMenu.SetActive(true);
        settings.SetActive(false);
    }

    //active the Setting Screen and deactive the others
    public void Settings()
    {
        mainMenu.SetActive(false);
        settings.SetActive(true);
    }

    //Function that playes the fade out in paralel
    //The Animation needs to be in the update mode = Unscale time
    private IEnumerator Fading()
    {
        fadeAnim.SetTrigger("fadeOut");

        yield return new WaitUntil(() => fadeInOut.color.a == 1);
        SceneManager.LoadScene(1);
    }
}
