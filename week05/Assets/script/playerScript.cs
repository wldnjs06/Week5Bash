using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class playerScript : MonoBehaviour
{   
    public float maxSpeeed;
    public float jumpPower;
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
        // 점프
        if(Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up*jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }


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

        //Landing Platform
        if(rigid.linearVelocity.y < 0) 
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0 ,1,0)); 

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("platform"));

            if(rayHit.collider != null)
            {
                if(rayHit.distance < 0.5f)
                {
                    //Debug.Log(rayHit.collider.name);
                    anim.SetBool("isJumping", false);
                }
                
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        Debug.Log("충돌 이벤트 발생!");

        Debug.Log(collision.gameObject.name);

        if(collision.gameObject.tag == "enemy")
        {
            OnDamaged(collision.transform.position);
        }


        
    }
    void OnDamaged(Vector2 tergetPos)
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1,1,1,0.4f);

        int dirc = transform.position.x - tergetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc,1)*7, ForceMode2D.Impulse);

        anim.SetTrigger("damaged");

        Invoke("OffDamaged", 3);
    }

    void OffDamaged()
    {
        gameObject.layer = 9;
        spriteRenderer.color = new Color(1,1,1,1);
    }
    
}

