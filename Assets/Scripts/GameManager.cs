using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // uso este singleton para acceder desde otros scripts

    public int playerLives = 3; // cantidad de vidas que tiene el jugador al inicio
    public int currentLevel = 1; // nivel actual
    public EnemyManager enemyManager; // referencia al controlador de enemigos
    public int score = 0; // puntuacion del jugador
    public TMPro.TextMeshProUGUI scoreText; // texto que muestra el puntaje
    public TMPro.TextMeshProUGUI livesText; // texto que muestra las vidas
    public TMPro.TextMeshProUGUI levelText; // texto que muestra el nivel actual

    [Header("Init Menu")]
    public GameObject initMenu; // panel de inicio del juego
    public bool isPause = true; // para pausar el juego cuando arranca

    [Header("Loss Menu")]
    public GameObject LossMenu; // panel que aparece al perder o terminar
    public TextMeshProUGUI txtTitle; // titulo grande del panel de derrota o victoria
    public TextMeshProUGUI txtButtonMessage; // mensaje del boton
    public Button buttonAgain; // boton para volver a jugar

    public Animator PlayerAnimator; // animador del jugador, para mostrar daño

    private float RemainingTime = 5f; // cuenta regresiva antes de iniciar un nivel
    private bool isCountdownActive = false; // si la cuenta regresiva esta activa

    private void Awake()
    {
        // me aseguro de que solo exista una instancia del GameManager
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // destruyo duplicado
    }

    void Start()
    {
        // detengo el tiempo al inicio del juego
        Time.timeScale = 0f;
        initMenu.SetActive(true); // muestro el menu de inicio

        // actualizo los textos de vidas y nivel
        livesText.text = playerLives.ToString();
        levelText.text = "<" + currentLevel.ToString() + ">";

        LossMenu.SetActive(false); // oculto el menu de derrota
    }

    void Update()
    {
        // actualizo constantemente el texto del nivel
        levelText.text = "<" + currentLevel.ToString() + ">";

        // si la cuenta regresiva esta activa, la actualizo
        if (isCountdownActive)
        {
            if (RemainingTime > 0f)
            {
                RemainingTime -= Time.unscaledDeltaTime; // le resto tiempo sin afectar Time.timeScale

                int tiempoEntero = Mathf.CeilToInt(RemainingTime); // redondeo hacia arriba

                if (currentLevel > 1)
                {
                    // muestro mensaje de inicio de siguiente nivel
                    txtButtonMessage.text = "LEVEL " + currentLevel.ToString() + "\nStarting in " + tiempoEntero;
                }
                else
                {
                    // mensaje para el primer nivel
                    txtTitle.text = "Starting in";
                    txtButtonMessage.text = tiempoEntero.ToString();
                }
            }
            else
            {
                // cuando termina la cuenta regresiva, inicio el juego
                isCountdownActive = false;
                isPause = false;
                LossMenu.SetActive(false);
                Time.timeScale = 1f;
            }
        }

        // actualizo el texto de las vidas
        livesText.text = playerLives.ToString();
    }

    public void PlayAgain()
    {
        // reinicio la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlayGame()
    {
        // empiezo el juego con una cuenta regresiva
        isCountdownActive = true;
        RemainingTime = 5f;
        isPause = false;
        initMenu.SetActive(false);
        LossMenu.SetActive(true);

        StartLevel(); // reinicio enemigos
    }

    public void LoseLife()
    {
        // quito una vida al jugador
        playerLives--;

        // muestro animacion de daño
        PlayerAnimator.SetTrigger("Damage");

        // si ya no tiene vidas, muestro pantalla de game over
        if (playerLives <= 0)
        {
            GameOver();
        }
    }

    public void AddLife()
    {
        // agrego una vida al jugador (por recompensa)
        playerLives++;
    }

    public void WinLevel()
    {
        // subo de nivel
        currentLevel++;

        if (currentLevel > 10)
        {
            // si ya pasaron los 10 niveles, muestro victoria final
            Victory();
        }
        else
        {
            // si no, me preparo para el siguiente nivel
            RemainingTime = 5f;
            isCountdownActive = true;
            playerLives++; // doy una vida como recompensa
            enemyManager.IncreaseDifficulty(); // subo la dificultad

            // actualizo los textos del panel
            txtTitle.text = "VICTORY";
            txtButtonMessage.text = "LEVEL " + currentLevel.ToString();
            buttonAgain.interactable = false; // desactivo el boton para evitar que reinicien antes de tiempo
            isPause = true;
            Time.timeScale = 0f;
            LossMenu.SetActive(true);
            RestartLevel(); // reinicio enemigos para el nuevo nivel
        }
    }

    void RestartLevel()
    {
        // reinicio los enemigos para el siguiente nivel
        enemyManager.RestartEnemies();
    }

    void StartLevel()
    {
        // reinicio enemigos para el primer nivel o reintento
        enemyManager.RestartEnemies();
    }

    public void GameOver()
    {
        // muestro mensaje de derrota y doy opcion de reiniciar
        txtTitle.text = "GAME OVER";
        txtButtonMessage.text = "PLAY AGAIN";
        LossMenu.SetActive(true);
        buttonAgain.interactable = true;
        Time.timeScale = 0f;
        isPause = true;
    }

    void Victory()
    {
        // muestro pantalla de victoria definitiva
        txtTitle.text = "VICTORY";
        txtButtonMessage.text = "PLAY AGAIN";
        buttonAgain.interactable = true;
        LossMenu.SetActive(true);
    }

    public void AddScore(int amount)
    {
        // sumo al puntaje actual
        score += amount;

        if (scoreText != null)
        {
            // actualizo el texto en pantalla
            scoreText.text = score.ToString();
        }

        // si ya no hay enemigos, doy por ganado el nivel
        if (enemyManager != null && enemyManager.AreAllEnemiesDead())
        {
            WinLevel();
        }
    }
}
