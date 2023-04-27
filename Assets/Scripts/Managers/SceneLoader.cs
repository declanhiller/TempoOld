using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers {
    public class SceneLoader : PersistentManager<SceneLoader> {

        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Slider progressBar;
        private string sceneToLoad;
        [SerializeField] private CanvasGroup canvasGroup;

        public UnityEvent DoneLoading;

        public void LoadScene(string sceneToLoad) {
            this.sceneToLoad = sceneToLoad;
            StartCoroutine(StartLoad());
        }

        IEnumerator StartLoad() {
            loadingScreen.SetActive(true);
            yield return StartCoroutine(FadeLoadingScreen(1, 1));
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);


            yield return StartCoroutine(WaitForLoad(operation));

            yield return StartCoroutine(FadeLoadingScreen(0, 1));
            loadingScreen.SetActive(false);
        }

        private IEnumerator WaitForLoad(AsyncOperation operation) {
            while (!operation.isDone) {
                progressBar.value = Mathf.Clamp01(operation.progress / 0.9f);
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator FadeLoadingScreen(float targetValue, float duration) {
            float startValue = canvasGroup.alpha;
            float time = 0;
            while (time < duration) {
                canvasGroup.alpha = Mathf.Lerp(startValue, targetValue, time / duration);
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            canvasGroup.alpha = targetValue;
            
            DoneLoading.Invoke();
            
        }
        
    }
}