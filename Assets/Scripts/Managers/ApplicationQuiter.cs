using UnityEngine;

namespace Managers {
    public class ApplicationQuiter : MonoBehaviour {
        public void Quit() {
            //Save data here too
            Application.Quit();
        }
    }
}