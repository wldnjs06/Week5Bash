using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class playerScript : MonoBehaviour
{   
    public float maxSpeeed;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    void Awake() 
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    void Update() 
    {
        //stop speed
        if(Input.GetButtonUp("Horizontal"))
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.normalized.x * 0.5f, rigid.linearVelocity.y);
        }

        //방향 전환 
        if (Input.GetButtonDown("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }
        
        // 에니메이션 변환(걷기 <-> 대기)
        if(rigid.linearVelocity.normalized.x == 0)
        {
            anim.SetBool("isWalking", false);
        }
        else
        {
            anim.SetBool("isWalking", true);
        }
    }
    void FixedUpdate() 
    {
        //Move by Key Control
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right*h, ForceMode2D.Impulse);

        if(rigid.linearVelocity.x > maxSpeeed) //오른쪽 최대 속력
        {
            rigid.linearVelocity = new Vector2(maxSpeeed, rigid.linearVelocity.y);
        }
        else if(rigid.linearVelocity.x < maxSpeeed*(-1)) //왼쪽 최대 속력
        {
            rigid.linearVelocity = new Vector2(maxSpeeed*(-1), rigid.linearVelocity.y);
        }
    }
}

