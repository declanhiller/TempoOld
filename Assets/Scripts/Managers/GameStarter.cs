using System;
using System.Collections.Generic;
using AudioFramework;
using Characters;
using Characters.TamborineTurtle;
using Player;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers {
    public class GameStarter : MonoBehaviour {

        private List<MonoBehaviour> thingsToEnable;

        [SerializeField] private GameStartCountdown countdown;
        [SerializeField] private Metronome metronome;

        [SerializeField] private PlayerStatTracker[] playerStatTrackers = new PlayerStatTracker[4];
        
        [SerializeField] private GameObject tamboTurtlePrefab;
        [SerializeField] private Transform respawnPoint;
        [SerializeField] private DynamicCamera camera;
        [SerializeField] private Transform[] spawnPositions = new Transform[4];
        [SerializeField] private GameObject playerNumberContainer;
        [SerializeField] private GameObject playerNumber;

        [SerializeField] private GameTimer gameTimer;

        [SerializeField] private WinMessage winMessage;
        
        private List<GameObject> objectsToDisable;
        private List<HurtboxController> hurtboxes;
        private List<GameObject> numbers;

        [SerializeField] private GameObject mapBoundary;
        
        
        public void Start() {
            thingsToEnable = new List<MonoBehaviour>();
            objectsToDisable = new List<GameObject>();
            hurtboxes = new List<HurtboxController>();
            numbers = new List<GameObject>();
            foreach (PlayerBase player in PlayerManager.INSTANCE.players) {
                player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
                Transform characterParent = player.GetComponentInChildren<PlayerCharacterContainer>().transform;
                GameObject turtleObj = Instantiate(tamboTurtlePrefab, characterParent);
                objectsToDisable.Add(turtleObj);
                TamborineTurtle tamborineTurtle = turtleObj.GetComponent<TamborineTurtle>();
                PlayerMovement playerMovement = turtleObj.GetComponent<PlayerMovement>();
                tamborineTurtle.metronome = metronome;
                tamborineTurtle.enabled = false;
                playerMovement.enabled = false;
                thingsToEnable.Add(tamborineTurtle);
                thingsToEnable.Add(playerMovement);
                turtleObj.GetComponent<CharacterRespawner>().respawnPoint = respawnPoint;
                camera.AddTarget(tamborineTurtle.GetComponent<DynamicCameraTarget>());
                tamborineTurtle.transform.position = spawnPositions[player.playerNumber].position;

                PlayerStatTracker playerStatTracker = playerStatTrackers[player.playerNumber];
                playerStatTracker.gameObject.SetActive(true);
                HurtboxController hurtbox = turtleObj.GetComponentInChildren<HurtboxController>();
                hurtbox.StatTracker = playerStatTracker;
                hurtbox.dynamicCamera = camera;
                hurtboxes.Add(hurtbox);
                

                GameObject numberObj = Instantiate(playerNumber, playerNumberContainer.transform);
                numbers.Add(numberObj);
                PlayerNumber num = numberObj.GetComponent<PlayerNumber>();
                Color color = Color.black;
                int number = player.playerNumber + 1;
                switch (number) {
                    case 1:
                        color = Color.red;
                        break;
                    case 2:
                        color = Color.blue;
                        break;
                    case 3:
                        color = Color.green;
                        break;
                    case 4:
                        color = Color.magenta;
                        break;
                }
                
                num.InitPlayerNumber(tamborineTurtle.playerNumberTarget, "P" + number, color);
            }

            gameTimer.SetupTimer(metronome.gameObject.GetComponent<AudioSource>().clip.length);
            // gameTimer.SetupTimer(10);

            countdown.OnComplete.AddListener(StartGame);
            
            SceneLoader.INSTANCE.DoneLoading.AddListener(DoneLoading);
            
        }

        public void DoneLoading() {
            metronome.Startup();
            gameTimer.ActivateTimer();
            SceneLoader.INSTANCE.DoneLoading.RemoveListener(DoneLoading);
        }

        private void StartGame() {
            foreach (MonoBehaviour god in thingsToEnable) {
                god.enabled = true;
            }
            countdown.OnComplete.RemoveListener(StartGame);
        }

        public void EndGame() {

            mapBoundary.GetComponent<MapBoundary>().isActive = false;

            int leastAmtOfDeaths = Int32.MaxValue;
            int playerNumber = -1;
            
            foreach (HurtboxController hurtbox in hurtboxes) {
                if (hurtbox.deathTotal < leastAmtOfDeaths) {
                    Debug.Log(hurtbox.GetComponentInParent<PlayerBase>().playerNumber + ": " + hurtbox.deathTotal);
                    leastAmtOfDeaths = hurtbox.deathTotal;
                    playerNumber = hurtbox.GetComponentInParent<PlayerBase>().playerNumber;
                }
            }

            foreach (PlayerBase player in PlayerManager.INSTANCE.players) {
                player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
            }
            
            
            winMessage.gameObject.SetActive(true);
            winMessage.Trigger(playerNumber + 1);

            UIGlobalManager.INSTANCE.currentlySelectedUI = winMessage;

            foreach (GameObject shit in numbers) {
                shit.SetActive(false);
            }
            
            metronome.gameObject.SetActive(false);
            foreach (GameObject player in objectsToDisable) {
                player.GetComponent<PlayerMovement>().enabled = false;
                player.GetComponent<CharacterBase>().enabled = false;
                player.GetComponentInChildren<HurtboxController>().enabled = false;
                player.GetComponent<SpriteRenderer>().enabled = false;
                player.GetComponent<Rigidbody2D>().isKinematic = true;
            }
        }
    }
}