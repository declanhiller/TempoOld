using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class ReallyShittyPlayButton : MonoBehaviour
{
    public void Play() {
        GameManager.INSTANCE.GoToMapSelect();
    }
}
