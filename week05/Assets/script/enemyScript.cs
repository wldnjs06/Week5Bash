using UnityEngine;

public class enemyScript : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D enemyCollider;
    public int nextMove;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<CapsuleCollider2D>();

        Invoke("Think",5); //Think라는 이름의 함수를 5초 뒤 실행
    }

    void FixedUpdate()
    {
        //Move
        rigid.linearVelocity = new Vector2(nextMove, rigid.linearVelocity.y);
        
        //지형체크
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove*0.2f,rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0 ,1,0)); 

        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("platform"));

        if(rayHit.collider == null)
        {
            //Debug.Log("앞에 절벽이다~");
            Turn();
        }
    }

    void Think()
    {
        nextMove = Random.Range(-1, 2);

        //에니메이션
        anim.SetInteger("walkspeed",nextMove);
        
        //방향 설정
        if (nextMove != 0)
        {
            spriteRenderer.flipX = nextMove == 1;
        }

        float nextThinkTime = Random.Range(2f,5f);
        Invoke("Think",nextThinkTime);
        
    }

    void Turn()
    {
        nextMove *= -1; 
        spriteRenderer.flipX = nextMove == 1;
        CancelInvoke();
        Invoke("Think",5);
    }

    public void OnDamaged()
    {
        spriteRenderer.color = new Color(1,1,1,0.4f);

        spriteRenderer.flipY = true;

        enemyCollider.enabled = false;

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        Invoke("DeActive", 5);

    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
