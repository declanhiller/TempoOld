using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour {
    
    [SerializeField] private CanvasGroup canvasGroup;
    public string sceneToLoad { get; set; }
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void StartLoading() {
        
    }
    
}
