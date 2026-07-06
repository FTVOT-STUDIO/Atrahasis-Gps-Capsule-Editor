using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    //public static GameManager instance;

    [Header("----------------[ Core ]")]
    public bool isOver;
    public int score;
    public int maxLevel;
    public int mergeCount = 0;
    public float spawnTime;
    public float dragTime;
    public bool isGameStart = false;
    public int nextCapsule;
    int randomLevel;
    public bool isvibration;
    [Header("----------------[ Object Pooling ]")]
    public GameObject capsulePrefab;
    public Transform capsuleGroup;
    public List<Capsule> capsulePool;
    public GameObject[] effectPrefab;
    public Transform effectGroup;
    public List<ParticleSystem> effectPool;
    [Range(1, 1000)]
    public int poolSize;
    public int poolCursor;
    public Capsule lastCapsule;

    [Header("----------------[ Audio ]")]
    public AudioSource bgmPlayer;
    public AudioClip[] bgmClips;
    public AudioSource LobbyBgmPlayer;
    public AudioSource gameOverBgmPlayer;
    public AudioSource[] sfxPlayer;
    public AudioClip[] sfxClip;
    public enum Sfx { LevelUp, Next, Attach,
        Button, Over, Explode, Warning, Combo, AtraHasis, comboVoice, TimeOver, Aracde
    , Skin, TimeOver2, Arcade2 };
    int sfxCursor;

    [Header("----------------[ UI ]")]
    public GameObject startGroup;
    public GameObject endGroup;
    public Text scoreText;
    public Text maxScoreText;
    public Text subScoreText;
    public GameObject scoreBar;
    public GameObject maxScoreBar;
    public GameObject GameStopBar;
    public GameObject Controllor;
    public GameObject WarningBar;
    public GameObject TouchPad;
    public List<GameObject> BgmButtons;
    public List<GameObject> SfxButtons;

    public Sprite bgmOnSprite; // BGM 켜진 상태 스프라이트
    public Sprite bgmOffSprite; // BGM 꺼진 상태 스프라이트

    public Sprite sfxOnSprite; // Sfx 켜진 상태 스프라이트
    public Sprite sfxOffSprite; // Sfx 꺼진 상태 스프라이트

    public GameObject pausePopup; // 일시 정지 창을 표시하는 오브젝트
    private bool isPaused = false; // 일시 정지 상태를 나타내는 변수
    public Button resumeButton; // 계속하기 버튼을 나타내는 변수
    public GameObject nextCapsuleImage;

    public Text modText;

    public Toggle[] VaibrateToggle;

    public Slider dragSlider;
    public float sliderValue;

    [Header("----------------[ ETC ]")]
    public GameObject line;
    public GameObject Warningline;
    public GameObject infinteLine;
    public GameObject bottom;
    private bool isSfxMuted = false; // Sfx 음소거 상태를 나타내는 변수
    public bool isBgmMuted = false; // BGM 음소거 상태를 나타내는 변수
    public GameObject backGround;
    public Sprite[] backGroundSprites;

    private const string VibrationKey = "Vibration";

    [Header("---------------[Capsule Info]")]
    public int capusleID;
    public int modID;

    [Header("----------------[ Combo ]")]
    public int comboCount;
    public float comboTimer;
    public int maxCombo;
    public Text comboText; // UI Text 요소
    public int baseComboScore = 10; // 기본 콤보 점수
    public int comboScoreMultiplier = 30; // 콤보 점수 배수
    //private bool isGameStarted = false;

    [Header("----------------[ Time Attack]")]
    public Text timeText; // 생존 시간
    public Text subTimeText;
    public Text bestTimeText;
    public Text subBestTimeText;
    public float surviveTime; // 게임 시작후 살아남은 시간
    public GameObject TimeOverGroup;
    private float bestTime;

    void Awake()
    {
        sliderValue = PlayerPrefs.GetFloat("SliderValue", 0.5f);
        dragSlider.value = sliderValue;

        dragSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });

        Application.targetFrameRate = 60; // 게임 프레임 설정

        isvibration = PlayerPrefs.GetInt(VibrationKey, 1) == 1;

        foreach (Toggle toggle in VaibrateToggle)
        {
            toggle.isOn = isvibration;
            toggle.onValueChanged.AddListener(OnvibrationToggleChange);
        }

        SetVibration(isvibration);

        capsulePool = new List<Capsule>(); // 캡슐Pool 초기화
        effectPool = new List<ParticleSystem>(); // 이펙트 Pool 초기화
        for (int index = 0; index < poolSize; index++)
        {
            MakeCapsule();
        }


        if (!PlayerPrefs.HasKey("MaxScore"))
        {
            PlayerPrefs.SetInt("MaxScore", 0);
        }
        maxScoreText.text = PlayerPrefs.GetInt("MaxScore").ToString(); // 최고 점수 텍스트 초기화



        //comboText = GameObject.Find("ComboText").GetComponent<Text>(); // 콤보 텍스트 초기화

        // PlayerPrefs에서 음소거 설정 로드
        isBgmMuted = PlayerPrefs.GetInt("BgmMuted", 0) == 1;
        isSfxMuted = PlayerPrefs.GetInt("SfxMuted", 0) == 1;

        capusleID = PlayerPrefs.GetInt("CapsuleID", capusleID); // 기본값 0 설정
        //Debug.Log("Capsule ID loaded: " + capusleID);

        // 음소거 설정에 따라 UI 버튼 이미지 변경
        foreach (var button in BgmButtons)
        {
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.sprite = isBgmMuted ? bgmOffSprite : bgmOnSprite;
        }

        foreach (var button in SfxButtons)
        {
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.sprite = isSfxMuted ? sfxOffSprite : sfxOnSprite;
        }

        bestTime = PlayerPrefs.GetFloat("BestTime", 0);

        subBestTimeText.text = "최고기록:" + (int)bestTime + "초";



        // BGM 및 SFX 음소거 설정 적용
        ApplyMuteSettings();
    }

    public void BackGrounds()
    {

        switch (modID)
        {
            case 0:
                backGround.GetComponent<SpriteRenderer>().sprite = backGroundSprites[0];
               // Debug.Log("Sprite" + modID);
                break;
            case 1:
                backGround.GetComponent<SpriteRenderer>().sprite = backGroundSprites[1];
                //Debug.Log("Sprite" + modID);
                break;
            case 2:
                backGround.GetComponent<SpriteRenderer>().sprite = backGroundSprites[2];
               // Debug.Log("Sprite" + modID);
                break;
            case 3:
                backGround.GetComponent<SpriteRenderer>().sprite = backGroundSprites[3];
              //  Debug.Log("Sprite" + modID);
                break;
            case 4:
                backGround.GetComponent<SpriteRenderer>().sprite = backGroundSprites[4];
               // Debug.Log("Sprite" + modID);
                break;
            case 5:
                backGround.GetComponent<SpriteRenderer>().sprite = backGroundSprites[5];
               // Debug.Log("Sprite" + modID);
                break;
            case 6:
                backGround.GetComponent<SpriteRenderer>().sprite = backGroundSprites[6];
               // Debug.Log("Sprite" + modID);
                break;
        }
    }

    public void PlayBGM(int bgmIndex)
    {
        if (bgmIndex >= 0 && bgmIndex < bgmClips.Length)
        {
            if (bgmPlayer.isPlaying)
            {
                bgmPlayer.Stop();
            }
            bgmPlayer.clip = bgmClips[bgmIndex];
            bgmPlayer.Play();
        }
    }

    public void ChangeMode(int mode)
    {
        modID = mode;
        // modID 값에 따라 다른 BGM 재생
        switch (modID)
        {
            case 0:
                PlayBGM(0); // 클래식 모드 BGM 재생
                break;
            case 1:
                PlayBGM(1); // 더 퍼스트 모드 BGM 재생
                break;
        }
    }

    public void GameStart() //게임 시작
    {
        OnPlayVibrate();

        surviveTime = 0;

        isOver = false;

        isGameStart = true;

        line.SetActive(true);
        Warningline.SetActive(true);
        bottom.SetActive(true);
        scoreText.gameObject.SetActive(true);
        maxScoreText.gameObject.SetActive(true);
        scoreBar.gameObject.SetActive(true);
        maxScoreBar.gameObject.SetActive(true);
        GameStopBar.gameObject.SetActive(true);
        Controllor.SetActive(true);
        startGroup.SetActive(false);
        modText.gameObject.SetActive(true);
        LobbyBgmPlayer.Stop();
        nextCapsuleImage.SetActive(true);

        bgmPlayer.Play();
        SfxPlay(Sfx.Button);
        SfxPlay(Sfx.AtraHasis);
        LobbyBgmPlayer.Stop();
        if (!isBgmMuted) // BGM 음소거가 해제된 경우 BGM 재생
        {
            bgmPlayer.Play();
        }

        Invoke("NextCapsule", 0.7f);
    }

    public void SetCapsuleID(int id)
    {
        capusleID = id;
        PlayerPrefs.SetInt("CapsuleID", capusleID); // 캡슐 ID 저장
        PlayerPrefs.Save();
       // Debug.Log("Capsule ID set to: " + capusleID);
       // Debug.Log("effectPrefab length: " + effectPrefab.Length);
    }
    public void SetModID(int id)
    {
        modID = id;
       // Debug.Log("Mod ID Is:" + modID);
    }

    Capsule MakeCapsule()
    {
        // 캡슐 생성
        GameObject instantCapsuleObj = Instantiate(capsulePrefab, capsuleGroup);
        instantCapsuleObj.name = "Capsule" + capsulePool.Count;
        Capsule instantCapsule = instantCapsuleObj.GetComponent<Capsule>();
        instantCapsule.manager = this;

        // 캡슐 이펙트 생성
        GameObject instantEffectObj = Instantiate(effectPrefab[capusleID], effectGroup);
        instantEffectObj.name = "Effect" + effectPool.Count;
        ParticleSystem instantEffect = instantEffectObj.GetComponent<ParticleSystem>();
        effectPool.Add(instantEffect);

        // 캡슐에 이펙트 설정
        instantCapsule.effect = instantEffect;
        capsulePool.Add(instantCapsule);

        return instantCapsule;
    }

    Capsule GetCapsule()  //Pool에서 사용 가능한 캡슐을 찾아 반환
    {
        for (int index = 0; index < capsulePool.Count; index++)
        {
            poolCursor = (poolCursor + 1) % capsulePool.Count;
            if (!capsulePool[poolCursor].gameObject.activeSelf)
            {
                return capsulePool[poolCursor];
            }
        }
        return MakeCapsule(); // 사용가능한 캡슐이 없다면 새로운 캡슐 생성후 반환
    }

    //다음 캡슐 생성
    void NextCapsule()
    {
        if (isOver)
        {
            return;
        }

        lastCapsule = GetCapsule();

        switch (modID)
        {
            case 0:
                // 클래식 모드
                modText.text = "클래식 모드";
                spawnTime = 0.3f;
                dragTime = 0.15f;
                SpawnCapsule();
                break;
            // 더 퍼스트 모드
            case 1:
                modText.text = "더 퍼스트 모드";
                spawnTime = 0.2f;
                dragTime = 0.15f;
                randomLevel = 0; // 0레벨만 생성
                lastCapsule.level = nextCapsule;
                nextCapsule = randomLevel;


              //  Debug.Log("다음 캡슐레벨:" + nextCapsule);
                break;
            // 일루전 모드
            case 2:
                modText.text = "일루전 모드";
                spawnTime = 0.3f;
                dragTime = 0.15f;
                randomLevel = Random.Range(0, 9);

                lastCapsule.level = nextCapsule;
                nextCapsule = randomLevel;


                //Debug.Log("다음 캡슐레벨:" + nextCapsule);
                break;
            // 슈퍼 패스트 모드
            case 3:
                modText.text = "슈퍼 패스트 모드";
                SpawnCapsule();
                spawnTime = 0f;
                dragTime = 0f;
                break;
            // 하드코어 모드
            case 4:
                modText.text = "하드코어 모드";
                spawnTime = 0.3f;
                dragTime = 0.15f;
                SpawnCapsule();
                break;
            case 5:
                modText.text = "타임 어택 모드";

                SpawnCapsule();

                spawnTime = 0.3f;
                dragTime = 0.15f;

                break;
            case 6:
                modText.text = "무한 모드";
                spawnTime = 0.3f;
                dragTime = 0.15f;
                SpawnCapsule();
                break;
        }

        lastCapsule.gameObject.SetActive(true);

        SfxPlay(Sfx.Next);
        StartCoroutine("WaitNext");
    }

    public void LineMove()
    {
        StartCoroutine(MoveLineRoutine());
    }

    IEnumerator MoveLineRoutine()
    {
        while (true)
        {


            Vector3 currentPosition = line.transform.position;

            currentPosition.y -= 0.2f;

            line.transform.position = currentPosition;


            yield return new WaitForSeconds(0.5f);
        }
    }

    public void UpLine()
    {
        Vector3 currentPosition = line.transform.position;

        currentPosition.y = Mathf.Min(currentPosition.y + 0.5f, 9);
        line.transform.position = currentPosition;
    }

    void SpawnCapsule()
    {

        if (maxLevel < 1) //맥스레벨이 1보다 작으면
        {
            randomLevel = Random.Range(0, 1); // 0레벨만 생성
        }
        else if (maxLevel == 1) // 1랩과 같다면
        {
            randomLevel = Random.Range(0, 2);  //0 ~ 1레벨 생성
        }
        else if (maxLevel == 2) // 2랩과 같다면
        {
            randomLevel = Random.Range(0, 3); // 0 ~ 2레벨 생성
        }
        else if (maxLevel == 3) // 3랩과 같다면
        {
            randomLevel = Random.Range(0, 4); // 0 ~ 3레벨 생성
        }
        else if (maxLevel == 4) // 4랩과 같다면
        {
            randomLevel = Random.Range(0, 4); // 0 ~ 3레벨 생성
        }
        else if (maxLevel == 5) // 5랩과 같다면
        {
            randomLevel = Random.Range(0, 5); // 0 ~ 4레벨 생성
        }
        else
            randomLevel = Random.Range(0, 5); // 아니면 0 ~ 4레벨 생성

        lastCapsule.level = nextCapsule;
        nextCapsule = randomLevel;


        //Debug.Log("다음 캡슐레벨:" + nextCapsule);
    }

    IEnumerator WaitNext() //캡슐 생성 대기시간
    {
        while (lastCapsule != null)
        {
            yield return null;
        }

        yield return new WaitForSeconds(spawnTime); // 0.5초후에 생성

        NextCapsule();

    }

    // 캡슐 이동 관리
    public void TouchDown()
    {
        if (lastCapsule == null)
            return;

        lastCapsule.Drag();
    }

    public void TouchUp()
    {
        if (lastCapsule == null)
            return;

        lastCapsule.Drop();
        lastCapsule = null;
    }

    public void GameOver() // 게임오버 관리 
    {
        if (isOver)
        {
            return;
        }

        isOver = true;

        StartCoroutine("GameOverRoutine");

    }

    IEnumerator GameOverRoutine()
    {
        // 1. 캡슐 배열 찾기
        Capsule[] capsules = FindObjectsOfType<Capsule>();
        SfxPlay(Sfx.Explode);
        // 2. 찾은 캡슐 물리 시뮬레이티드 비활성화
        for (int index = 0; index < capsules.Length; index++)
        {
            capsules[index].rigid.simulated = false;

        }


        // 3. 1초 간격으로 찾은 캡슐을 비활성화하고 이펙트 출력
        for (int index = 0; index < capsules.Length; index++)
        {
            capsules[index].Hide(Vector3.up * 100);
            yield return new WaitForSeconds(0f);

            WarningBar.SetActive(false);
            line.SetActive(false);
            infinteLine.SetActive(false);
            comboText.gameObject.SetActive(false);
        }


        yield return new WaitForSeconds(1f); //1초 간격

        // 최고점수 저장
        int maxScore = Mathf.Max(score, PlayerPrefs.GetInt("MaxScore"));
        PlayerPrefs.SetInt("MaxScore", maxScore);
        // 최고점수 출력
        subScoreText.text = "Score :" + scoreText.text;

        endGroup.SetActive(true);


        bgmPlayer.Stop();
        WarningBar.SetActive(false);
        comboText.gameObject.SetActive(false);


        // 게임오버시 BGM출력 여부 확인
        if (gameOverBgmPlayer != null)
        {
            if (!isBgmMuted && gameOverBgmPlayer.isActiveAndEnabled)
            {
                gameOverBgmPlayer.Play();
            }
            else
            {
                gameOverBgmPlayer.Stop();
            }
        }

        if (isBgmMuted)
        {
            LobbyBgmPlayer.Play();

        }
    }

    public void TimeOver()
    {
        if (isOver)
        {
            return;
        }

        isOver = true;

        SfxPlay(Sfx.TimeOver);
        SfxPlay(Sfx.TimeOver2);
        StartCoroutine("TimeOverRoutine");
    }

    IEnumerator TimeOverRoutine()
    {
        // 1. 캡슐 배열 찾기
        Capsule[] capsules = FindObjectsOfType<Capsule>();
        SfxPlay(Sfx.Explode);
        // 2. 찾은 캡슐 물리 시뮬레이티드 비활성화
        for (int index = 0; index < capsules.Length; index++)
        {

            capsules[index].rigid.simulated = false;
        }


        // 3. 1초 간격으로 찾은 캡슐을 비활성화하고 이펙트 출력
        for (int index = 0; index < capsules.Length; index++)
        {
            capsules[index].Hide(Vector3.up * 100);
            yield return new WaitForSeconds(0f);

            WarningBar.SetActive(false);
            line.SetActive(false);
            infinteLine.SetActive(false);
            comboText.gameObject.SetActive(false);
        }

        //SfxPlay(Sfx.Explode);
        float curruntTime = surviveTime;

        subTimeText.text = (int)curruntTime + "초";

        TimeOverGroup.SetActive(true);

        bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);

        bestTimeText.text = "최고기록:" + (int)bestTime + "초";

        if (surviveTime < bestTime)
        {
            bestTime = surviveTime;

            PlayerPrefs.SetFloat("BestTime", bestTime);

            bestTimeText.text = "최고기록:" + (int)bestTime + "초";
        }

        bgmPlayer.Stop();


        yield return new WaitForSeconds(1f); //1초 간격
    }

    public void Reset() // 돌아기기 버튼
    {
        SfxPlay(Sfx.Button);
        StartCoroutine("ResetCoroutine");
    }
    IEnumerator ResetCoroutine()
    {
        yield return new WaitForSeconds(0.5f); // 1초 대기
        SceneManager.LoadScene("Main");
    }

    public void SfxPlay(Sfx type) //SFX 관리
    {
        switch (type)
        {
            case Sfx.LevelUp:
                sfxPlayer[sfxCursor].clip = sfxClip[Random.Range(0, 3)];
                break;
            case Sfx.Next:
                sfxPlayer[sfxCursor].clip = sfxClip[4];
                break;
            case Sfx.Attach:
                sfxPlayer[sfxCursor].clip = sfxClip[5];
                break;
            case Sfx.Button:
                sfxPlayer[sfxCursor].clip = sfxClip[6];
                break;
            case Sfx.Over:
                sfxPlayer[sfxCursor].clip = sfxClip[7];
                break;
            case Sfx.Explode:
                sfxPlayer[sfxCursor].clip = sfxClip[Random.Range(8, 10)];
                break;
            case Sfx.Warning:
                sfxPlayer[sfxCursor].clip = sfxClip[10];
                break;
            case Sfx.Combo:
                sfxPlayer[sfxCursor].clip = sfxClip[Random.Range(11, 13)];
                break;
            case Sfx.AtraHasis:
                sfxPlayer[sfxCursor].clip = sfxClip[13];
                break;
            case Sfx.comboVoice:
                sfxPlayer[sfxCursor].clip = sfxClip[14];
                break;
            case Sfx.TimeOver:
                sfxPlayer[sfxCursor].clip = sfxClip[15];
                break;
            case Sfx.Aracde:
                sfxPlayer[sfxCursor].clip = sfxClip[16];
                break;
            case Sfx.Skin:
                sfxPlayer[sfxCursor].clip = sfxClip[17];
                break;
            case Sfx.TimeOver2:
                sfxPlayer[sfxCursor].clip = sfxClip[18];
                break;
            case Sfx.Arcade2:
                sfxPlayer[sfxCursor].clip = sfxClip[19];
                break;

        }

        sfxPlayer[sfxCursor].Play();
        sfxCursor = (sfxCursor + 1) % sfxPlayer.Length;
    }
    void Update()
    {
        if (Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
        {
            TouchUp();
        }


        if (isGameStart && !isOver)
        {
            surviveTime += Time.deltaTime;

            timeText.text = (int)surviveTime + "초";
        }


        Screen.sleepTimeout = SleepTimeout.NeverSleep;

    }
    void LateUpdate()
    {
        scoreText.text = score.ToString();
        UpdateComboText();
    }

    public void GameQuit() // 게임 종료
    {
        SfxPlay(Sfx.Button);
        Application.Quit();
    }

    public void TogglePause()
    {
        SfxPlay(Sfx.Button);
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0;
            if (pausePopup != null)
            {
                pausePopup.SetActive(true); // 팝업창 활성화
            }

            if (resumeButton != null)
            {
                resumeButton.interactable = true; // 버튼 상호작용 활성화
            }

        }
        else
        {
            Time.timeScale = 1;
            if (pausePopup != null)
            {
                pausePopup.SetActive(false); //팝업창 비활성화
            }

            if (resumeButton != null)
            {
                resumeButton.interactable = false; // 버튼 상호작용 비활성화
            }
        }
    }

    public void ResumeGame() // 계속 하기 버튼
    {
        TogglePause();
        SfxPlay(Sfx.Button);
    }

    public void ReturnToMenu() // 메뉴 화면으로 돌아가기
    {
        Time.timeScale = 1f; // 게임 실행
        SceneManager.LoadScene("Main"); // 씬 불러오기
        SfxPlay(Sfx.Button);
    }

    public void BgmSlience()
    {
        // 음소거 상태 변경
        isBgmMuted = !isBgmMuted;

        // PlayerPrefs에 음소거 설정 저장
        PlayerPrefs.SetInt("BgmMuted", isBgmMuted ? 1 : 0);
        PlayerPrefs.Save();

        // 음소거 설정에 따라 UI 버튼 이미지 변경
        foreach (var button in BgmButtons)
        {
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.sprite = isBgmMuted ? bgmOffSprite : bgmOnSprite;
        }

        ApplyMuteSettings();
        // BGM 음소거 설정 적용
        bgmPlayer.mute = isBgmMuted;

        SfxPlay(Sfx.Button);
    }

    public void SfxSlience()
    {
        // 음소거 상태 변경
        isSfxMuted = !isSfxMuted;

        // PlayerPrefs에 음소거 설정 저장
        PlayerPrefs.SetInt("SfxMuted", isSfxMuted ? 1 : 0);
        PlayerPrefs.Save();

        // 음소거 설정에 따라 UI 버튼 이미지 변경
        foreach (var button in SfxButtons)
        {
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.sprite = isSfxMuted ? sfxOffSprite : sfxOnSprite;
        }

        // SFX 음소거 설정 적용
        foreach (var player in sfxPlayer)
        {
            player.mute = isSfxMuted;
        }

        SfxPlay(Sfx.Button);
    }
    private Coroutine comboCoroutine;

    IEnumerator ComboTimerRoutine()
    {
        comboTimer = 1.5f; // 콤보 활성화 시간 설정

        while (comboTimer > 0 && comboCount > 0)
        {
            comboTimer -= Time.deltaTime;
            yield return null;
        }

        // Reset combo
        comboCount = 0;
        comboText.gameObject.SetActive(false);
        comboCoroutine = null; // 코루틴 종료 후 null로 초기화
    }

    public void IncreaseCombo()
    {

        comboCount++;

        // Update UI
        if (comboText != null)
        {
            comboText.text = comboCount > 1 ? comboCount + " Combo!" : "";
            comboText.gameObject.SetActive(true);
        }

        // Calculate score
        int comboBonus = baseComboScore + (comboCount - 1) * comboScoreMultiplier;
        score += comboBonus;

        // Restart or extend combo timer
        if (comboCoroutine != null)
        {
            StopCoroutine(comboCoroutine);
        }
        comboCoroutine = StartCoroutine(ComboTimerRoutine());

        if (comboTimer > 0)
        {
            SfxPlay(Sfx.comboVoice);
        }
        //StartCoroutine(ComboTextRoutine());
        SfxPlay(Sfx.Combo);
    }

    void ApplyMuteSettings()
    {
        // BGM 음소거 설정 적용
        bgmPlayer.mute = isBgmMuted;
        LobbyBgmPlayer.mute = isBgmMuted;

        // SFX 음소거 설정 적용
        foreach (var player in sfxPlayer)
        {
            player.mute = isSfxMuted;
        }

    }

    void UpdateComboText() //팝업창 실행시에 콤보 텍스트 숨기기
    {

        if (pausePopup != null && pausePopup.activeSelf)
        {
            if (comboText != null && comboText.gameObject.activeSelf)
            {
                comboText.gameObject.SetActive(false);
            }
            return;
        }

        if (comboText != null) //콤보 텍스트 출력
        {
            comboText.text = comboCount > 0 ? comboCount + " 콤보!!" : "";
        }
    }

    public void ToggleVibration()
    {
        bool isOn = VaibrateToggle[0].isOn;

        foreach (Toggle toggle in VaibrateToggle)
        {
            toggle.isOn = isOn;
        }

        PlayerPrefs.SetInt(VibrationKey, isOn ? 1 : 0);
        PlayerPrefs.Save();

        SetVibration(isOn);

        SetVibration(isvibration);
    }
    public void OnToggleValueChanged(bool isOn)
    {
        // Set vibration based on the specific toggle that was changed
        SetVibration(isOn);
    }
    private void OnvibrationToggleChange(bool isOn)
    {

        foreach (Toggle toggle in VaibrateToggle)
        {
            toggle.isOn = isOn;
        }

        PlayerPrefs.SetInt(VibrationKey, isOn ? 1 : 0);
        PlayerPrefs.Save();

        isvibration = isOn;
        SetVibration(isOn);
    }

    private void SetVibration(bool isOn)
    {
        if (isOn)
        {
            // pc 빌드 때문에 진동 끔
           // Handheld.Vibrate();
            //Debug.Log("진동 켜짐");
        }
        else
        {
            //Debug.Log("진동 꺼짐");
            
        }
    }

    public void OnPlayVibrate()
    {
        if (isvibration)
        {
            // pc 빌드 때문에 진동 끔
            //   Handheld.Vibrate();
        }
    }

    private void OnSliderValueChanged()
    {
        // Update floatValue based on slider value
        sliderValue = dragSlider.value;

        PlayerPrefs.SetFloat("SliderValue", sliderValue);
        PlayerPrefs.Save();
    }

    public void ExitInfiniteMod()
    {
        int maxScore = Mathf.Max(score, PlayerPrefs.GetInt("MaxScore"));
        PlayerPrefs.SetInt("MaxScore", maxScore);

        SceneManager.LoadScene("Main");
    }

    public void PlayButtonSfx()
    {
        SfxPlay(Sfx.Button);
    }

    public void PlayPickButtonSfx()
    {
        SfxPlay(Sfx.AtraHasis);
    }

    public void PlayArcade()
    {
        SfxPlay(Sfx.Aracde);
        //SfxPlay(Sfx.Arcade2);
    }

    public void PlaySkin()
    {
        SfxPlay(Sfx.Skin);
    }
}