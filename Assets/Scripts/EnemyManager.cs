using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Types of enemies")]
    public GameObject[] enemyPrefabs; // arreglo que almacena los prefabs de enemigos

    [Header("Grid of enemies")]
    public int rows = 3; // cantidad de filas
    public int columns = 6; // cantidad de columnas
    public float spacingX = 1.5f; // espacio horizontal entre enemigos
    public float spacingY = 1.5f; // espacio vertical entre enemigos

    [Header("Movement")]
    public float moveSpeed = 1f; // velocidad de movimiento lateral
    public float moveDownAmount = 0.3f; // cuanto bajan al llegar al borde
    public float shootInterval = 2f; // intervalo de disparo

    private List<GameObject> enemies = new List<GameObject>(); // lista de enemigos vivos
    private Vector2 direction = Vector2.right; // direccion actual del movimiento
    private float moveTimer = 0f; // temporizador para movimiento
    private float moveRate = 0.5f; // tiempo entre pasos de movimiento
    private float shootTimer = 0f; // temporizador para disparo
    private float screenLimitX; // limite horizontal de la pantalla
    private bool goingDownNextStep = false; // indica si debe bajar en el siguiente paso

    // se llama al iniciar el juego
    void Start()
    {
        float camWidth = Camera.main.orthographicSize * Camera.main.aspect;
        screenLimitX = camWidth - 0.5f;

        SpawnEnemies(); // genera los enemigos
    }

    // se llama cada frame
    void Update()
    {
        MoveEnemies(); // movimiento de enemigos
        HandleShooting(); // disparos aleatorios
        CheckGameOverCondition(); // verifica si algun enemigo bajo demasiado
    }

    // genera todos los enemigos en una grilla
    void SpawnEnemies()
    {
        enemies.Clear(); // limpia lista de enemigos

        float totalWidth = (columns - 1) * spacingY;
        float totalHeight = (rows - 1) * spacingX;
        Vector2 offset = new Vector2(-totalWidth * 1.5f, totalHeight / 1.5f);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector2 pos = new Vector2(col * spacingX, -row * spacingY) + offset;
                GameObject enemy = Instantiate(enemyPrefabs[row % enemyPrefabs.Length], pos, Quaternion.identity, transform);
                enemies.Add(enemy); // agrega el enemigo a la lista
            }
        }
    }

    // controla el movimiento de los enemigos
    void MoveEnemies()
    {
        moveTimer += Time.deltaTime;

        if (moveTimer >= moveRate)
        {
            moveTimer = 0f;

            if (goingDownNextStep)
            {
                transform.Translate(Vector2.down * moveDownAmount);
                goingDownNextStep = false;
                return;
            }

            transform.Translate(direction * moveSpeed);

            foreach (GameObject enemy in enemies)
            {
                if (enemy == null) continue;

                float enemyX = enemy.transform.position.x;

                if (direction == Vector2.right && enemyX > screenLimitX)
                {
                    direction = Vector2.left;
                    goingDownNextStep = true;
                    break;
                }
                else if (direction == Vector2.left && enemyX < -screenLimitX)
                {
                    direction = Vector2.right;
                    goingDownNextStep = true;
                    break;
                }
            }
        }
    }

    // selecciona un enemigo vivo al azar y le ordena disparar
    void HandleShooting()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= shootInterval)
        {
            shootTimer = 0f;

            List<GameObject> alive = enemies.FindAll(e => e != null && e.GetComponent<Enemy>() != null);

            if (alive.Count > 0)
            {
                GameObject shooter = alive[Random.Range(0, alive.Count)];
                shooter.GetComponent<Enemy>().Shoot();
            }
        }
    }

    // verifica si un enemigo ha llegado muy abajo
    void CheckGameOverCondition()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            if (enemy.transform.position.y <= -Camera.main.orthographicSize + 1f)
            {
                GameManager.Instance.LoseLife();
                break;
            }
        }
    }

    // retorna true si todos los enemigos han sido destruidos
    public bool AreAllEnemiesDead()
    {
        int vivos = 0;

        foreach (var e in enemies)
        {
            if (e != null) vivos++;
        }

        return vivos == 0;
    }

    // aumenta la dificultad al reducir los tiempos entre pasos y disparos
    public void IncreaseDifficulty()
    {
        moveRate *= 0.9f;
        shootInterval *= 0.9f;
    }

    // destruye todos los enemigos actuales y reinicia la grilla
    public void RestartEnemies()
    {
        foreach (GameObject e in enemies)
        {
            if (e != null) Destroy(e);
        }

        enemies.Clear();

        SpawnEnemies();
    }

    // marca a un enemigo especifico como muerto en la lista
    public void MarkEnemyAsDead(GameObject enemy)
    {
        int index = enemies.IndexOf(enemy);

        if (index != -1)
        {
            enemies[index] = null;
        }
    }
}
