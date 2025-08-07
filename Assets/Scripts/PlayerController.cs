using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;                      // Velocidad de movimiento del jugador
    public GameObject bulletPrefab;              // Prefab de la bala que dispara el jugador
    public Transform firePoint;                  // Punto desde donde se dispara la bala

    private float screenLimitX;                  // Limite horizontal para que el jugador no salga de la pantalla

    public float fireRate = 0.3f;                // Tiempo minimo entre disparos
    private float nextFireTime = 0f;             // Momento en el que puede volver a disparar

    public GameManager gameManager;              // Referencia al GameManager para verificar si el juego esta en pausa

    void Start()
    {
        // Calcula el limite horizontal segun el tamaño del jugador y la camara
        float halfPlayerWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;
        screenLimitX = camWidth - halfPlayerWidth;
    }

    void Update()
    {
        // Solo permite moverse y disparar si el juego no esta en pausa
        if (!gameManager.isPause)
        {
            Move();
            Shoot();
        }
    }

    // Metodo que mueve al jugador de izquierda a derecha
    void Move()
    {
        // Entrada del InputManager para el movimiento
        float input = Input.GetAxisRaw("Horizontal");
        Vector3 newPosition = transform.position + Vector3.right * input * speed * Time.deltaTime;

        // Limita el movimiento para que no salga de la pantalla
        newPosition.x = Mathf.Clamp(newPosition.x, -screenLimitX, screenLimitX);

        transform.position = newPosition;
    }

    // Metodo que instancia una bala cuando el jugador presiona espacio
    void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextFireTime)
        {
            Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            nextFireTime = Time.time + fireRate;
        }
    }
}

