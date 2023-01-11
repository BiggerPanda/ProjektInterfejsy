using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class AdminPanel : MonoBehaviour
{   
    [SerializeField] private Button WinGameButton;
    [SerializeField] private Button AIMakeMoveButton;
    [SerializeField] private Button LoseGameButton;
    [SerializeField] private Button ResetGameButton;
    [SerializeField] private Button ToggleFly;
    [SerializeField] private DynamicMoveProvider moveProvider;
    private Chessboard chessboard = null;

    // Start is called before the first frame update
    void Start()
    {
        WinGameButton.onClick.AddListener(WinGame);
        AIMakeMoveButton.onClick.AddListener(AIMakeMove);
        LoseGameButton.onClick.AddListener(LoseGame);
        ResetGameButton.onClick.AddListener(ResetGame);
        ToggleFly.onClick.AddListener(ToggleFlyMode);

        chessboard = Chessboard.Instance;
    }

    private void WinGame()
    {
        chessboard.WinGame(chessboard.Player.playerColor);
    }

    private void AIMakeMove()
    {
       StartCoroutine(chessboard.BotPlayer.MakeRandomMove());
    }

    private void LoseGame()
    {
        chessboard.WinGame(chessboard.BotPlayer.playerColor);
    }

    private void ResetGame()
    {
        SceneLoader.instance.LoadScene(1);
    }
    
    private void ToggleFlyMode()
    {   
        moveProvider.enableFly = !moveProvider.enableFly;
    }
    
}
