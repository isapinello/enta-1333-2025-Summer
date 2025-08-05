using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager instance;
    public GameObject LoadingSceenObject;
    public Slider ProgressBar;
    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void SwitchToScene(string SceneSelection)
    {
        LoadingSceenObject.SetActive(true);
        ProgressBar.value = 0;
        StartCoroutine(SwitchToSceneAsync(SceneSelection));
    }
    IEnumerator SwitchToSceneAsync(string SceneSelection)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");
        while (!asyncLoad.isDone)
        {
            ProgressBar.value = asyncLoad.progress;
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        LoadingSceenObject.SetActive(false);
    }
}
