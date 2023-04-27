using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : PersistentManager<PlayerManager> {
    
    [NonSerialized] public List<PlayerBase> players = new List<PlayerBase>();
    
    [SerializeField] private string actionMap;

    [SerializeField] private GameObject popUpMessage;

    private string leave = "left";
    private string join = "joined";


    public string ActionMap { get; private set; }

    private void Start() {
        ActionMap = actionMap;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnPlayerJoin(PlayerInput input) {
        input.SwitchCurrentActionMap(actionMap);
        PlayerBase player = input.gameObject.GetComponent<PlayerBase>();
        players.Add(player);
        GameObject msg = Instantiate(popUpMessage, GameObject.FindGameObjectWithTag("Canvas").transform);
        PopupMessage popupMessage = msg.GetComponent<PopupMessage>();
        popupMessage.SetMessage("Player " + players.Count + " joined");
        for (int i = 0; i < players.Count; i++) {
            players[i].playerNumber = i;
        }
    }

    public void OnPlayerLeave(PlayerInput input) {
        PlayerBase player = input.gameObject.GetComponent<PlayerBase>();
        players.Remove(player);
        GameObject msg = Instantiate(popUpMessage, GameObject.FindGameObjectWithTag("Canvas").transform);
        PopupMessage popupMessage = msg.GetComponent<PopupMessage>();
        popupMessage.SetMessage("Player " + (players.Count + 1) + " left");
        for (int i = 0; i < players.Count; i++) {
            players[i].playerNumber = i;
        }
    }
    

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        
    }

    protected override void LoadNewData(PlayerManager newInstance) {
        ActionMap = newInstance.ActionMap;
    }
}
