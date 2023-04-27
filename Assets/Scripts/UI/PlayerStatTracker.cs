using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatTracker : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI percentageText;
    [SerializeField] private TextMeshProUGUI deathText;

    public void UpdatePercentage(float percentage) {
        percentageText.text = ((int) percentage).ToString() + "%";
    }

    public void UpdateDeath(int deathCount) {
        deathText.text = deathCount.ToString();
    }
    
    
}
