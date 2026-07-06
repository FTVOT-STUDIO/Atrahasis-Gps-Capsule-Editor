
using UnityEngine;


public class NextCapsule : MonoBehaviour
{
    public GameManager manager;
    Animator anim;

    public RuntimeAnimatorController[] controller;

    void Awake()
    {
        anim = GetComponent<Animator>();
        manager = FindObjectOfType<GameManager>();
    }
    public void SetNextSkin()
    {
        anim.runtimeAnimatorController = controller[manager.capusleID];
    }
    
    void Update()
    {
        int nextCapsuleLevel = manager.nextCapsule;
      
        anim.SetInteger("Level", nextCapsuleLevel);
        SetNextSkin();

    }

}
