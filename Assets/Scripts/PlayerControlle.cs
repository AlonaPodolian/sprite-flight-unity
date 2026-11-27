using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerControlle : MonoBehaviour
{

    private float elapsedTime = 0f;
    private float score = 0f;
    public float scoreMultiplier = 10f;
    public float thrustForce = 1f;
    Rigidbody2D rb;
    public UIDocument uiDocument;
    private Label scoreText;
    private Label highScoreText;
    private Button restartButton;
    public GameObject explosionEffect;
    public GameObject boosterFlame;
    public InputAction moveForward;
    public InputAction lookPosition;
    private bool gameOver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveForward.Enable();
        lookPosition.Enable();
        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreLabel");
        highScoreText = uiDocument.rootVisualElement.Q<Label>("HighScoreLabel");
        highScoreText.style.display = DisplayStyle.None;
        restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        restartButton.style.display = DisplayStyle.None;
        restartButton.clicked += ReloadScene;
        boosterFlame.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();
        MovePlayer();
    }


    void UpdateScore()
        {
            elapsedTime += Time.deltaTime;
            score = Mathf.FloorToInt(elapsedTime * scoreMultiplier);
            Debug.Log("Score: " + score);
            scoreText.text = "Score: " + score;
        }

    void MovePlayer()
    {
        bool isThrusting = moveForward.IsPressed();
        boosterFlame.SetActive(isThrusting);

        if (isThrusting)
        {
            Vector2 pointerPosition =
                lookPosition.ReadValue<Vector2>();

            Vector3 pointerWorldPosition = Camera.main.ScreenToWorldPoint(
                new Vector3(pointerPosition.x, pointerPosition.y, 0f)
            );

            Vector2 direction =
                (pointerWorldPosition - transform.position).normalized;

            transform.up = direction;
            rb.AddForce(direction * thrustForce);
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameOver) return;
        gameOver = true;

        GameObject obstaclesObject = GameObject.Find("Obstacles");

        if (obstaclesObject != null)
        {
            Rigidbody2D[] obstacleBodies =
                obstaclesObject.GetComponentsInChildren<Rigidbody2D>();

            foreach (Rigidbody2D obstacleBody in obstacleBodies)
            {
                obstacleBody.linearVelocity = Vector2.zero;
                obstacleBody.angularVelocity = 0f;
                obstacleBody.bodyType = RigidbodyType2D.Static;
            }
        }

        GameObject bordersObject = GameObject.Find("Borders");

        if (bordersObject != null)
        {
            bordersObject.SetActive(false);
        }

        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        int currentScore = Mathf.FloorToInt(score);

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        highScoreText.text = "High Score: " + highScore;
        highScoreText.style.display = DisplayStyle.Flex;

        Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(gameObject);

        restartButton.style.display = DisplayStyle.Flex;
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
