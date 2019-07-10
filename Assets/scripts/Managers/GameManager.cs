﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject field;
    public GameObject playerObj;
    public GameObject ballObj;
    public enum Bonuses { upSpeed, downSpeed, upSize, downSize, controlBall, bigBall, upScore, bomb };

    private float HP = 1f;
    private int life = 3;
    public int speed = 4;
    public float playerSpeed = 1f;
    public float ballSpeed = 200f;
    public List<GameObject> enemys;
    public bool ready = false;
    public bool controlBall = false;

    public GameObject player;
    public GameObject enemy;
    public GameObject[] Texts;
    public GameObject[] clouds;
    public GameObject bubble;

    public Transform PlayerStartPoint;
    public Transform EnemyStartPoint;
    public Transform PlayerEndPoint;
    public Transform EnemyEndPoint;
    public Transform TextStartPoint;
    public Transform StartSkyPoint;
    public Transform EndSkyPoint;
    public Transform StartBubblePoint;
    public Transform EndBubblePoint;

    private float TimeCloud;
    public float controlTimeCloud = 5f;
    private float TimeBubble;
    public float controlTimeBubble = 7f;

    private void Awake()
    {
        Cursor.visible = false;
        Instance = this;
        Helper.Set2DCameraToObject(field);
    }

    private void Start()
    {
        PrepareGame();
    }

    private void Update()
    {
        if (Instance.speed < 13)
        {
            if (ScoreManager.Instance.score == 5000 || ScoreManager.Instance.score == 10000 || ScoreManager.Instance.score == 20000 || ScoreManager.Instance.score == 30000 || ScoreManager.Instance.score == 40000 || ScoreManager.Instance.score == 50000 || ScoreManager.Instance.score == 100000 || ScoreManager.Instance.score == 250000 || ScoreManager.Instance.score == 500000)
            {
                SetSpeed(1);
                if (Instance.speed == 8 || Instance.speed == 13)
                {
                    ballSpeed += 100f;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

        BackhroundAnim();
    }

    private void PrepareGame()
    {
        playerObj = Instantiate(player, PlayerStartPoint);
        playerObj.GetComponent<StartObj>().EndPoint = PlayerEndPoint;

        var enemyObj = Instantiate(enemy, EnemyStartPoint);
        enemyObj.GetComponent<StartObj>().EndPoint = EnemyEndPoint;
    }

    private void BackhroundAnim()
    {
        TimeCloud += Time.deltaTime;
        if (TimeCloud >= controlTimeCloud)
        {
            Instantiate(clouds[Random.Range(0, 5)], new Vector3(StartSkyPoint.position.x, Random.Range(StartSkyPoint.position.y, EndSkyPoint.position.y), StartSkyPoint.position.z), Quaternion.identity);
            TimeCloud = 0;
        }

        TimeBubble += Time.deltaTime;
        if (TimeBubble >= controlTimeBubble)
        {
            Instantiate(bubble, new Vector3(Random.Range(StartBubblePoint.position.x, EndBubblePoint.position.x), StartBubblePoint.position.y, StartBubblePoint.position.z), Quaternion.identity);
            TimeBubble = 0;
        }
    }

    public static void PlayerDie()
    {
        foreach (GameObject enem in Instance.enemys)
        {
            enem.GetComponent<EnemyBulletScript>().StartDie();
        }
        Instance.playerObj.GetComponent<PlayerScript>().StartDie();
        Instance.life--;
        if (Instance.life >= 0)
        {
            Destroy(UIManager.Instance.lifes[Instance.life], 1f);
            Instance.Restart();
        }
        else Instance.GameOver();
    }

    private void Restart()
    {
        HP = 1f;
        UIManager.Instance.lifeBar.value = 0f;
        Instance.ready = true;
        Instantiate(Texts[0], new Vector3(TextStartPoint.position.x, TextStartPoint.position.y, TextStartPoint.position.z), Quaternion.identity);
        playerObj = Instantiate(player, PlayerStartPoint);
        playerObj.GetComponent<StartObj>().EndPoint = PlayerEndPoint;
    }

    private void GameOver()
    {
        var text = Instantiate(Texts[1], new Vector3(TextStartPoint.position.x, TextStartPoint.position.y, TextStartPoint.position.z), Quaternion.identity);
    }

    public void SetSpeed(int speed)
    {
        Instance.speed += speed;
        UIManager.Instance.speed.text = (Instance.speed - 3).ToString();
    }

    public void GetDamage(float dmg)
    {
        HP -= dmg;
        UIManager.Instance.lifeBar.value = 1 - HP;
        if (HP <= 0)
        {
            Destroy(ballObj);
            PlayerDie();
        }

    }

    public void GetBonus(int bonus)
    {
        Debug.Log(bonus);
        switch (bonus)
        {
            case (int)Bonuses.upSpeed:
                if (Instance.playerSpeed < 2f) Instance.playerSpeed += 0.2f;
                else Instance.playerSpeed = 2f;
                break;
            case (int)Bonuses.downSpeed:
                if (Instance.playerSpeed > 0.4f) Instance.playerSpeed -= 0.6f;
                else Instance.playerSpeed = 0.4f;
                break;
            case (int)Bonuses.upSize:
                playerObj.GetComponent<PlayerScript>().SetSprite(1);
                break;
            case (int)Bonuses.downSize:
                playerObj.GetComponent<PlayerScript>().SetSprite(-1);
                break;
            case (int)Bonuses.controlBall:
                Instance.controlBall = !Instance.controlBall;
                break;
            case (int)Bonuses.bigBall:
                ballObj.GetComponent<BallScript>().SetSprite();
                break;
            case (int)Bonuses.upScore:
                ScoreManager.Instance.SetScore(10000);
                break;
            case (int)Bonuses.bomb:
                foreach (GameObject enem in enemys)
                {
                    enem.GetComponent<EnemyBulletScript>().StartDie();
                }
                enemys.Clear();
                break;
        }
    }
}
