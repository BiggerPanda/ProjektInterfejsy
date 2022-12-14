using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStatusPanel : MonoBehaviour
{

    [SerializeField] private TMP_Text turnText = null;
    [SerializeField] private TMP_Text BlackDeadAmount = null;
    [SerializeField] private TMP_Text WhiteDeadAmount = null;

    private void Start()
    {
        UpdateText();
        Chessboard.Instance.onPieceMove += UpdateText;
    }

    public void UpdateText()
    {
        turnText.text = "Turn: " + (Chessboard.Instance.IsWhiteTurn ? TeamColor.White.ToString() : TeamColor.Black.ToString());
        BlackDeadAmount.text = "Black Dead: " + Chessboard.Instance.BlackDeadAmount.ToString();
        WhiteDeadAmount.text = "White Dead: " + Chessboard.Instance.WhiteDeadAmount.ToString();
    }
}
