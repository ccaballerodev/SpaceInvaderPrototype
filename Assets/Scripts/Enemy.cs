using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Prefab de la bala que dispara el enemigo
    public GameObject bulletPrefab;

    // Punto desde donde se dispara la bala
    public Transform firePoint;

    // Bandera para evitar que el enemigo muera dos veces
    private bool isDead = false;

    // Metodo para disparar una bala
    public void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
    }

    // Metodo que se llama cuando el enemigo muere
    public void Death()
    {
        // Marca el enemigo como muerto en el EnemyManager
        GameManager.Instance.enemyManager.MarkEnemyAsDead(gameObject);

        // Si ya esta muerto, no se hace nada
        if (isDead) return;
        isDead = true;

        // Determina los puntos segun el layer del enemigo
        int points = 0;
        int layer = gameObject.layer;

        // Valores de cada enemigo
        if (layer == LayerMask.NameToLayer("Enemy1")) points = 30;        // Primera columna
        else if (layer == LayerMask.NameToLayer("Enemy2")) points = 20;   // Tercera y segunda columna
        else if (layer == LayerMask.NameToLayer("Enemy3")) points = 10;   // Cuarta y quinta columna
        else if (layer == LayerMask.NameToLayer("Enemy4")) points = 100;  // Nave nodriza

        // Agrega los puntos al score del jugador
        GameManager.Instance.AddScore(points);

        // Destruye el objeto del enemigo
        Destroy(gameObject);
    }

    // Metodo que se ejecuta al entrar en contacto con un trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si el enemigo toca el limite de la pantalla, se termina el juego
        if (other.CompareTag("Limit"))
        {
            GameManager.Instance.GameOver();
        }
    }
}