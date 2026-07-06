using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Capsule : MonoBehaviour
{
    public GameManager manager;
    public ParticleSystem effect;
    public int level;
    public bool isDrag;
    public bool isMerge;
    public bool isAttach;
    public AudioSource warningAudio;
    public RuntimeAnimatorController[] capsuleCon;

   

    public LineRenderer lineRenderer; // 추가된 LineRenderer

    public Rigidbody2D rigid;
    CircleCollider2D circle;
    Animator anim;
    SpriteRenderer spriteRenderer;

    float deadTime;
    float warningTime;
    float infiTime;

    void Awake()
    {
        
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        circle = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        manager = FindObjectOfType<GameManager>();
        lineRenderer = GetComponent<LineRenderer>(); // LineRenderer 컴포넌트 가져오기
        BroadcastMessage("SetupCapsule", manager.capusleID, SendMessageOptions.DontRequireReceiver);
    }

    void OnEnable()
    {
        anim.SetInteger("Level", level);
        lineRenderer.enabled = true; // 캡슐이 활성화되면 LineRenderer 활성화
        lineRenderer.startWidth = 0.6f; // 라인 두께 설정
        lineRenderer.endWidth = 0.6f; // 라인 두께 설정
        UpdateLineRenderer(transform.position); // 초기 위치 설정

        anim.runtimeAnimatorController = capsuleCon[manager.capusleID];

        manager.TouchPad.gameObject.SetActive(false);
        StartCoroutine(EnableDragAfterDelay(manager.dragTime));

        if (manager.modID == 3)
        {
            rigid.gravityScale = 6;
        }

        if (manager.effectPrefab.Length > manager.capusleID)
        {
            GameObject effectObject = Instantiate(manager.effectPrefab[manager.capusleID], manager.effectGroup);
            effect = effectObject.GetComponent<ParticleSystem>();
        }

        

    }


    void SetupCapsule(int capsuleID)
    {
        anim.runtimeAnimatorController = capsuleCon[capsuleID];
    }


    IEnumerator EnableDragAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 일정 시간이 지난 후에 드래그 활성화
        manager.TouchPad.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        //캡슐 속성 초기화
        level = 0;
        isDrag = false;
        isMerge = false;
        isAttach = false;
        spriteRenderer.sortingOrder = 0;
        // 캡슐 트랜스폼 초기화
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.zero;
        // 캡슐 물리 초기화
        rigid.simulated = false;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        circle.enabled = true;
        lineRenderer.enabled = false; // 캡슐이 비활성화되면 LineRenderer 비활성화
        anim.enabled = true;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    void Update()
    {
        if (isDrag)
        {
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) * manager.sliderValue * 3f;
            // X축 경계 설정
            float leftBorder = -4.3f + transform.localScale.x / 2f;
            float rightBorder = 4.3f - transform.localScale.x / 2f;

            if (MousePos.x < leftBorder)
            {
                MousePos.x = leftBorder;
            }
            else if (MousePos.x > rightBorder)
            {
                MousePos.x = rightBorder;

            }
            MousePos.y = 8;
            MousePos.z = 0;
            transform.position = Vector3.Lerp(transform.position, MousePos, 1f);

            // LineRenderer 업데이트
            UpdateLineRenderer(MousePos);
        }

    }

    void UpdateLineRenderer(Vector3 MousePos)
    {
        // 선의 시작점은 캡슐의 위치
        lineRenderer.SetPosition(0, transform.position);
        // 선의 끝점은 화면 아래쪽
        Vector3 lineEnd = new Vector3(transform.position.x, -10f, transform.position.z);
        lineRenderer.SetPosition(1, lineEnd);
  

    }

    public void Drag()
    {
        isDrag = true;
    }

    public void Drop()
    {
        isDrag = false;
        rigid.simulated = true;
        lineRenderer.enabled = false; // 드래그를 놓으면 LineRenderer 비활성화
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine("AttachRoutine");
    }

    IEnumerator AttachRoutine()
    {
        if (isAttach)
        {
            yield break;
        }

        isAttach = true;

        
        
        manager.SfxPlay(GameManager.Sfx.Attach);

        yield return new WaitForSeconds(2.5f);

        rigid.velocity = Vector2.zero;

        isAttach = false;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Capsule")
        {
            Capsule other = collision.gameObject.GetComponent<Capsule>();

            if (level == other.level && !isMerge && !other.isMerge && level < 9)
            {
                //나와 상대편 위치 가져오기
                float meX = transform.position.x;
                float meY = transform.position.y;
                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;
                // 1.내가 아래에 있을 때 
                // 2. 동일한 높이일 때, 내가 오른쪽에 있을 때
                if (meY < otherY || (meY == otherY && meX > otherX))
                {
                    //상대방은 숨기기
                    other.Hide(transform.position);
                    //나는 레벨업
                    LevelUp();
                    manager.mergeCount++;
                    
                }
            }
        }
    }

    public void Hide(Vector3 targetPos)
    {
        isMerge = true;

        spriteRenderer.sortingOrder = -1;
        rigid.simulated = false;
        circle.enabled = false;
        spriteRenderer.color = new Color(1,1,1,0);
        anim.enabled = false;
        if (targetPos == Vector3.up * 100)
        {
            EffectPlay();
        }

        

        StartCoroutine(HideRoutine(targetPos));
    }

    IEnumerator HideRoutine(Vector3 targetPos)
    {
        int frameCount = 0;
  
        while (frameCount < 20)
        {
            frameCount++;
            if (targetPos != Vector3.up * 100)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, 100f);
            }
            else if (targetPos == Vector3.up * 100)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 100f);
            }
            
            yield return null;
        }
      
        manager.score += (int)Mathf.Pow(2, level);

        isMerge = false;
        gameObject.SetActive(false);
    }

    void LevelUp()
    {
        isMerge = true;

        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;

        StartCoroutine(LevelUpRoutine());

        if (manager.modID == 4)
        {
            manager.UpLine();
        }

        
        

    }

    IEnumerator LevelUpRoutine()
    {
        yield return new WaitForSeconds(0f);

        if (manager.modID == 2)
        {
            int newLevel = Random.Range(0, 9); // 0부터 9까지의 레벨을 무작위로 선택
            anim.SetInteger("Level", newLevel);
            level = newLevel;
            EffectPlay();
            manager.SfxPlay(GameManager.Sfx.LevelUp);
        }
        else
        {
            anim.SetInteger("Level", level + 1);
            EffectPlay();
            manager.SfxPlay(GameManager.Sfx.LevelUp);
            level++;
        }

        if (manager.modID == 5 && level ==9)
        {
           // Debug.Log("Game Over");
            manager.TimeOver();

        }

        yield return new WaitForSeconds(0f);

        manager.OnPlayVibrate();

        manager.maxLevel = Mathf.Max(level, manager.maxLevel);

        isMerge = false;

        // 6. 합쳐질 때 콤보 증가
        //manager.IncreaseCombo();
        if (manager.mergeCount >= 2)
        {
            manager.IncreaseCombo();
        }
    }

    void OnTriggerStay2D(Collider2D collision) //게임 오버 시간
    {
        if (collision.tag == "Finish")
        {
            deadTime += Time.deltaTime;

            if (deadTime > 1f)
            {
                manager.GameOver();
            }
        }
        if (collision.tag == "Warning")
        {
            warningTime += Time.deltaTime;

            if (warningTime > 2)
            {
                if (!manager.WarningBar.activeSelf)
                {
                    manager.WarningBar.SetActive(true);
                    manager.SfxPlay(GameManager.Sfx.Warning);
                }
            }
        }

        if (collision.CompareTag("Infinite"))
        {
            infiTime += Time.deltaTime;
            Capsule[] capsules = FindObjectsOfType<Capsule>();
            if (infiTime > 1)
            {
                
                for (int index = 0; index < capsules.Length; index++)
                {
                    capsules[index].Hide(Vector3.up * 100);
                    
                }
                manager.SfxPlay(GameManager.Sfx.Explode);
                manager.TouchUp();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            deadTime = 0;
        }
            if (collision.tag == "Warning")
        {
            manager.WarningBar.SetActive(false);
            warningTime = 0;
        }
        if (collision.CompareTag("Infinite"))
        {
            infiTime = 0;
        }
       
    }

    void EffectPlay()
    {
        effect.transform.position = transform.position;
        effect.transform.localScale = transform.localScale;
        effect.Play();
    }
}
