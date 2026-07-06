using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.BasicApi.Events;

public class GpgsBuilder : MonoBehaviour
{
    /*
    public GameObject loginButton;  // 로그인 버튼 GameObject 이름을 수정해야 합니다.
    public GameObject logoutButton; // 로그아웃 버튼 GameObject 이름을 수정해야 합니다.


    private void Start()
    {
        // Google Play Games 초기화
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        AuthenticateUser();
        // 초기 상태 설정
        UpdateButtonState();
    }

    private void AuthenticateUser()
    {
        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate((bool success) =>
            {
                UpdateButtonState();
            });
        }
        else
        {
            UpdateButtonState();
        }
    }

   
    public void OnClickGoogleLoginButton()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success) //로그인에 성공하면
            {
                Debug.Log(Social.localUser.userName + "님 환영합니다");
                Debug.Log("당신의 ID는 " + Social.localUser.id + "입니다");
                UpdateButtonState(); // 버튼 상태 업데이트
            }
            else //로그인에 실패하면
            {
                Debug.Log("로그인에 실패했습니다");
            }
        });
    }

   
    public void OnClickGoogleLogoutButton()
    {
        PlayGamesPlatform.Instance.SignOut();
        UpdateButtonState(); // 버튼 상태 업데이트
    }


    private void UpdateButtonState()
    {
        bool isLoggedIn = Social.localUser.authenticated;

        // 로그인 상태에 따라 버튼 활성화/비활성화 처리
       

        if (isLoggedIn)
        {
            loginButton.SetActive(false); // 로그인 버튼 비활성화
            logoutButton.SetActive(true); // 로그아웃 버튼 활성화
        }
        else
        {
            loginButton.SetActive(true); // 로그인 버튼 활성화
            logoutButton.SetActive(false); // 로그아웃 버튼 비활성화
        }
    }
    */
}
