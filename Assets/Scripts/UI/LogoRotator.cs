using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoRotator : MonoBehaviour {

    [SerializeField] private float angularSpeed = 3f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, angularSpeed * Time.deltaTime);
    }
}
