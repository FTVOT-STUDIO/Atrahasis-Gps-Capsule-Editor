using GoogleMobileAds.Api;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AddMobManager : MonoBehaviour
{
    GameManager manager;
    public Canvas[] myCanvas;

    private InterstitialAd interstitial;

    void Awake()
    {
        manager = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        RequestInterstitial();  // 게임 시작 시 광고를 미리 로드합니다.
    }

    private void RequestInterstitial()
    {
        string adUnitId = "ca-app-pub-1946197079320485/6649221443";

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;

        this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        manager.gameOverBgmPlayer.Stop();
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        SceneManager.LoadScene("Main");
        RequestInterstitial();  // 광고를 닫았을 때 다음 광고를 미리 로드합니다.
    }

    public void GameOver()
    {
        if (this.interstitial.IsLoaded())
        {
            if (manager.surviveTime > 30)
            {
                this.interstitial.Show();
                foreach (var canvas in myCanvas)
                {
                    canvas.sortingOrder = -1;
                }
            }
            else
            {
                SceneManager.LoadScene("Main");
            }
        }
        else
        {
            SceneManager.LoadScene("Main");
        }
    }
}
