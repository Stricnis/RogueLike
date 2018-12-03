using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    public int wallDamaged = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartlevelDelay = 1f;
    private Animator animator;
    private int food;
    public Text foodText;
    public bool HasMovedThisTurn { get; private set; }

    // Use this for initialization
    protected override void Start ()
    {
        animator = GetComponent<Animator>();
        foodText = GameObject.Find("FoodText").GetComponent<Text>();
        food = GameManager.instance.playerFoodPoints;
        foodText.text = "Food: " + food.ToString();

        base.Start();
	}

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }

    // Update is called once per frame
    void Update ()
    {
        if (!GameManager.instance.playerTurn)
            return;

        int horz = (int)Input.GetAxisRaw("Horizontal");
        int vert = (int)Input.GetAxisRaw("Vertical");

        if (horz != 0)
            vert = 0;

        if (horz != 0 || vert != 0)
            AttemptMove<Wall>(horz, vert);
	}

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food --;
        foodText.text = "Food: " + food.ToString();

        base.AttemptMove<T>(xDir, yDir);

        HasMovedThisTurn = true;
        CheckIfGameOver();
        GameManager.instance.playerTurn = false;
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
            GameManager.instance.GameOver();
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamaged);
        animator.SetTrigger("playerChop");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Exit":
                Invoke("Restart", restartlevelDelay);
                enabled = false;
                HasMovedThisTurn = false;
                break;

            case "Food":
                food += pointsPerFood;
                foodText.text = "+" + pointsPerFood.ToString() + "!!! Food: " + food.ToString();
                collision.gameObject.SetActive(false);
                break;

            case "Soda":
                food += pointsPerSoda;
                foodText.text = "+" + pointsPerSoda.ToString() + "!!! Food: " + food.ToString();
                collision.gameObject.SetActive(false);
                break;
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "Ouch! -" + loss.ToString() + "!!! Food: " + food.ToString();
        CheckIfGameOver();
    }
}
