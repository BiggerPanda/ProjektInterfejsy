using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button optionsButton;
    // Start is called before the first frame update
    void Start()
    {
        playButton.onClick.AddListener(() => SceneLoader.instance.LoadScene(1));
        quitButton.onClick.AddListener(() => SceneLoader.instance.QuitGame());
        optionsButton.onClick.AddListener(() => { });
    }

}
