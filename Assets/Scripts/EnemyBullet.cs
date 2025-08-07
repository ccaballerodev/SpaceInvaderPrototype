using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    // Velocidad de la bala enemiga
    public float speed = 5f;

    void Update()
    {
        // Mueve la bala hacia abajo en cada frame segun la velocidad establecida
        transform.Translate(Vector2.down * speed * Time.deltaTime);

        // Si la bala sale por la parte inferior de la pantalla, se destruye
        if (transform.position.y < -Camera.main.orthographicSize - 1f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Si colisiona con el jugador
        if (other.CompareTag("Player"))
        {
            // Se llama al metodo para perder una vida
            GameManager.Instance.LoseLife();
            // Se destruye la bala
            Destroy(gameObject);
        }
        // Si colisiona con un escudo
        else if (other.CompareTag("Shield"))
        {
            // Intenta obtener el componente DestructibleShield del objeto con el que colisiono
            if (other.TryGetComponent(out DestructibleShield shield))
            {
                // Se aplica el daño al escudo en la posicion de impacto
                shield.DamageAt(transform.position);
            }

            // Se destruye la bala
            Destroy(gameObject);
        }
        // Si colisiona con otra bala (jugador)
        else if (other.CompareTag("Bullet"))
        {
            // Se destruye la bala enemiga
            Destroy(gameObject);
        }
    }
}