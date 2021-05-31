using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MulliganPanel : MonoBehaviour
{
    public Button startGameBtn;
    public Button keepCardsBtn;
    public Button mulliganCardsBtn;
    public TextMeshProUGUI waitingPlayerText;

    public Button GetStartGameButton()
    {
        return startGameBtn;
    }

    public Button GetMulliganButton()
    {
        return mulliganCardsBtn;
    }

    public Button GetKeepButton()
    {
        return keepCardsBtn;
    }

    public TextMeshProUGUI GetPlayerText()
    {
        return waitingPlayerText;
    }
}
