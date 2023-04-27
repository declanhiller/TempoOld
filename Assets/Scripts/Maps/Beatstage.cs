using UnityEngine;
using UnityEngine.SceneManagement;

namespace Maps {
    [CreateAssetMenu(fileName = "Map", menuName = "Tempo", order = 0)]
    public class Beatstage : ScriptableObject {
        //This will eventually be used to create a map in a template scene but now just to display data to player
        [SerializeField] private string name;
        [SerializeField] private Texture2D mapBackground;
        [SerializeField] private AudioClip song;
        [SerializeField] private float bpm;
        [SerializeField, Range(1, 5)] private int difficulty;
        //TODO: We will use scenes for now to simplify map loading but in the future we want to create
        //TODO: a template scene and load songs/backgrounds/spawnpoints/colliders/other map data... onto that scene
        [SerializeField] private string sceneName;
    }
}