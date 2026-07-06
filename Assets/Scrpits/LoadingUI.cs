using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;


public class LoadingUI : MonoBehaviour
{
    private static LoadingUI instance;

    public static LoadingUI Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = FindObjectOfType<LoadingUI>();
                if (obj != null)
                {
                    instance = obj;
                }
                else 
                {
                    instance = Create();
                }
            }
            return instance;
        }
    }

    private static LoadingUI Create()
    {
        return Instantiate(Resources.Load <LoadingUI>("LoadingUI"));
    }
    void Awake()
    {
        if (Instance != this) 
        { 
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public CanvasGroup CanvasGroup;

    public Image progressbar;

    string loadSceneName;

    public void loadScene(string sceneName)
    {
        gameObject.SetActive(true);
        SceneManager.sceneLoaded += OnSceneLoaded;
        loadSceneName = sceneName;
        StartCoroutine(LoadSceneProcess());
    }

    IEnumerator LoadSceneProcess()
    {
        
        progressbar.fillAmount = 0f;
        yield return StartCoroutine(Fade(true));

        AsyncOperation op = SceneManager.LoadSceneAsync(loadSceneName);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;
            if (op.progress < 0.9f)
            {
                progressbar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressbar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);
                if (progressbar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(arg0.name == loadSceneName)
        {
            StartCoroutine(Fade(false));
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
    }

    IEnumerator Fade(bool isFadeIn)
    {
        float timer = 0f;
        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 3f;
            CanvasGroup.alpha = isFadeIn ? Mathf.Lerp(0f, 1f, timer) : Mathf.Lerp(1f, 0f, timer);
        }

        if (!isFadeIn)
        {
            gameObject.SetActive (false);
        }
    }
}
