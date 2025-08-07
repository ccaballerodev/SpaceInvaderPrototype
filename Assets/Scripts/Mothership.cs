using System.Collections.Generic;
using UnityEngine;

public class Mothership : MonoBehaviour
{
    public float speed = 5f;            // Velocidad a la que se mueve la nave nodriza
    private Vector2 direction;          // Direccion en la que se movera (izquierda o derecha)

    void Start()
    {
        // Decide aleatoriamente si la nave aparece desde la izquierda o desde la derecha
        if (Random.value < 0.5f)
        {
            // Si el valor aleatorio es menor que 0.5, entra desde la izquierda

            // Posiciona la nave fuera de la pantalla por la izquierda, un poco mas arriba en Y
            transform.position = new Vector3(
                -Camera.main.orthographicSize * Camera.main.aspect - 1f,
                transform.position.y + 3.5f,
                0
            );

            direction = Vector2.right; // Se movera hacia la derecha
        }
        else
        {
            // Si el valor aleatorio es mayor o igual a 0.5, entra desde la derecha

            // Posiciona la nave fuera de la pantalla por la derecha, un poco mas arriba en Y
            transform.position = new Vector3(
                Camera.main.orthographicSize * Camera.main.aspect + 1f,
                transform.position.y + 3.5f,
                0
            );

            direction = Vector2.left; // Se movera hacia la izquierda
        }
    }

    void Update()
    {
        // Mueve la nave en la direccion elegida a la velocidad establecida
        transform.Translate(direction * speed * Time.deltaTime);

        // Si la nave se sale completamente del borde de la pantalla, se destruye sola
        if (Mathf.Abs(transform.position.x) > Camera.main.orthographicSize * Camera.main.aspect + 2f)
        {
            Destroy(gameObject);
        }
    }
}

