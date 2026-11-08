using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class playerScript : MonoBehaviour
{   
    public GameManager gameManager;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    
    public float maxSpeeed;
    public float jumpPower;
    public BoxCollider2D playerCollider;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    AudioSource audioSource;
    void Awake() 
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        playerCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }
    void PlaySound(string action)
    {
        switch(action)
        {
            case "Jump":
                 audioSource.clip = audioJump;
                 break;
            case "Attack":
                 audioSource.clip = audioAttack;
                 break;   
            case "Damaged":
                 audioSource.clip = audioDamaged;
                 break;
            case "Item":
                 audioSource.clip = audioItem;
                 break;  
            case "Die":
                 audioSource.clip = audioDie;
                 break;  
            case "Finish":
                 audioSource.clip = audioFinish;
                 break;  
        }
    }

    void Update() 
    {
        // 점프
        if(Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up*jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            PlaySound("Jump");

        }

        //stop speed
        if(Input.GetButtonUp("Horizontal"))
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.normalized.x * 0.5f, rigid.linearVelocity.y);
        }

        //방향 전환 
        if (Input.GetButton("Horizontal"))
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
        //충돌 확인
        // Debug.Log("충돌 이벤트 발생!");
        // Debug.Log(collision.gameObject.name);

        if(collision.gameObject.tag == "enemy")
        {
            //Atack
            if(rigid.linearVelocity.y < 0 && 
            transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);

            }
            else
            {
                OnDamaged(collision.transform.position);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //아이템 획득 트리거
        if(collision.gameObject.tag == "item")
        {
            //포인트 추가
            bool isBronze = collision.gameObject.name.Contains("bronze");
            bool isSilver = collision.gameObject.name.Contains("silver");
            bool isGold = collision.gameObject.name.Contains("gold");

            if(isBronze)
            {
                gameManager.stagePoint += 50;
            }
            else if(isSilver)
            {
                gameManager.stagePoint += 100;
            }
            else if(isGold)
            {
                gameManager.stagePoint += 300;
            }
            // 동전 액티브 
            PlaySound("Item");
            collision.gameObject.SetActive(false);
        }
        //finish 트리거
        else if(collision.gameObject.tag == "Finish")
        {
            //Debug.Log("깃밯에 닿았다");
            //다음 스테이지
            PlaySound("Finish");
            gameManager.NextStage();
        }
    }
    void OnDamaged(Vector2 tergetPos)
    {
        //피깎임
        PlaySound("Damaged");
        gameManager.healthDown();
        
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
    
    void OnAttack(Transform enemy)
    {
        PlaySound("Attack");
        gameManager.stagePoint += 100;

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //enemyScript 에서 enemyScript 클래스 불러오기
        enemyScript enemyMove = enemy.GetComponent<enemyScript>();
        enemyMove.OnDamaged();
    }

    public void OnDie()
    {
        spriteRenderer.color = new Color(1,1,1,0.4f);

        spriteRenderer.flipY = true;

        playerCollider.enabled = false;

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    public void VelocityZero()
    {
        rigid.linearVelocity = Vector2.zero;
    }
}

