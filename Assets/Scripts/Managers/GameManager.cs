using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers {
    public class GameManager : PersistentManager<GameManager> {
        

        public void GoToStartScreen() {
            SceneManager.LoadScene("StartScreen");
        }

        public void Exit() {
            Application.Quit();
        }

        public void GoToMapSelect() {
            // SceneLoader.INSTANCE.LoadScene("TowerOfHeaven");
            // SceneLoader.INSTANCE.LoadScene("MapSelectScene");
            SceneManager.LoadScene("MapSelectScene");
        }

        public void GoToCharacterSelect() {
        }

        public void GoToTowerOfHeaven() {
            SceneLoader.INSTANCE.LoadScene("TowerOfHeaven");
        }
        
    }
}