using System.Collections;
using System.Collections.Generic;
using Player.Interfaces;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBase : MonoBehaviour, IPlayer {
    
    
    [SerializeField] private PlayerInput playerInput;
    
    public int playerNumber { get; set; }

    [SerializeField] private GameObject popupPrefab;

    [SerializeField] private float connectionLengthAfterDisconnect;
    
    public bool isInGame { get; set; }
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnControllerDisconnect() {
        GameObject msg = Instantiate(popupPrefab, GameObject.FindGameObjectWithTag("Canvas").transform);
        PopupMessage popupMessage = msg.GetComponent<PopupMessage>();

        if (isInGame) {
            //execute code for in game stuff
            //pause game
            popupMessage.SetMessage("Player controller has disconnected");
        } else {
            Destroy(gameObject);
        }
    }

    public void OnControllerReconnect() {
        GameObject msg = Instantiate(popupPrefab, GameObject.FindGameObjectWithTag("Canvas").transform);
        PopupMessage popupMessage = msg.GetComponent<PopupMessage>();
        popupMessage.SetMessage("Player controller has reconnected");
    }

    public void SetActionMaps(InputActionMap actionMap) {
        playerInput.SwitchCurrentActionMap(actionMap.name);
    }
}
