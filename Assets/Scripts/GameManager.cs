using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;
    public float turndelay = .1f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playerTurn = true;

    private Text levelText;
    private GameObject levelImage;
    private bool doingSetup = false;

    private int level = 0;
    private List<Enemy> enemies;
    private bool enemiesMoving;

	// Use this for initialization
	void Awake ()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
	}
	
    void InitGame()
    {
        if (doingSetup)
            return;

        doingSetup = true;

        InitLevelImage();
        InitLevelText();

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    void InitLevelImage()
    {
        levelImage = GameObject.Find("LevelImage");
        if (levelImage != null)
        {
            levelImage.SetActive(true);
            Invoke("HideLevelImage", levelStartDelay);
        }
    }

    void InitLevelText()
    {
        GameObject obj = GameObject.Find("LevelText");
        levelText = obj != null ? obj.GetComponent<Text>() : null;
        if (levelText != null)
            levelText.text = "Day " + level.ToString();
    }

    private void HideLevelImage()
    {
        if (levelImage != null)
        {
            levelImage.SetActive(false);
            doingSetup = false;
        }
    }

    //This is called each time a scene is loaded.
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //Add one to our level number.
        level++;

        //Call InitGame to initialize our level.
        InitGame();
    }

    void OnEnable()
    {
        //Tell our ‘OnLevelFinishedLoading’ function to start listening for a scene change event as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        // Tell our ‘OnLevelFinishedLoading’ function to stop listening for a scene change event as soon as this script is disabled.
        // Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    public void GameOver()
    {
        if (levelText != null)
        {
            levelText.text = string.Format("After {0} days,{1}you pathetically starved.", level.ToString(), Environment.NewLine);
            levelText.fontSize /= 2;
        }

        if (levelImage != null)
            levelImage.SetActive(true);

        enabled = false;
    }

    // Update is called once per frame
    void Update ()
    {
        if (playerTurn || enemiesMoving || doingSetup)
            return;

        StartCoroutine(MoveEnemies());
	}

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turndelay);

        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turndelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(0);
        }

        playerTurn = true;
        enemiesMoving = false;
    }
}
