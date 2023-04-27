using System.Collections;
using System.Collections.Generic;
using Managers;
using UI;
using UnityEngine;

public class UIMapSelector : MonoBehaviour, IPressableUI, IBackableUI
{
    public void Press(int playerNumber) {
        GameManager.INSTANCE.GoToTowerOfHeaven();
    }

    public void Back(int playerNumber) {
        GameManager.INSTANCE.GoToStartScreen();
    }
}
