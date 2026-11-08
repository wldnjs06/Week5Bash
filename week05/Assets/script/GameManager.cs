using System.Linq.Expressions;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public playerScript player;
    public GameObject[] Stages;

    public UnityEngine.UI.Image[] UIhealth;
    public TextMeshProUGUI UIPoint;
    public TextMeshProUGUI UIStage;
    public GameObject RestartBtn;

    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }
    public void NextStage()
    {
        //스테이지 변환
        if (stageIndex < Stages.Length-1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE" + (stageIndex + 1);
        }
        else // 게임 클리어
        {
            Time.timeScale = 0;
            Debug.Log("게임 클리어");

            if (RestartBtn == null)
            {
                Debug.LogError("RestartBtn이 연결되지 않았습니다.");
                return;
            }

            TextMeshProUGUI btnText = RestartBtn.GetComponentInChildren<TextMeshProUGUI>();

            if (btnText == null)
            {
                Debug.LogError("버튼 내부에서 TextMeshProUGUI를 찾을 수 없습니다.");
                return;
            }

            btnText.text = "Game Clear!";
            RestartBtn.SetActive(true);
        }


        //포인트 계산
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void healthDown()
    {
        if(health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1,0,0,0.4f);
        }
        else
        {
            UIhealth[0].color = new Color(1,0,0,0.4f);
            player.OnDie();
            Debug.Log("죽었습니다!");

            RestartBtn.SetActive(true);
        }
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D collision)
    {
        //맵밖으로 떨어짐
        if(collision.gameObject.tag == "Player")
        {
            healthDown();

            //플레이어 원위치
            if(health > 1)
            {
                PlayerReposition();
            }
            
        }

    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(-1,4,-1);
        player.VelocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
