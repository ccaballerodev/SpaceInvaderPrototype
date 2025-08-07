using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;

    void Update()
    {
        // hago que la bala se mueva hacia arriba todo el tiempo
        transform.Translate(Vector2.up * speed * Time.deltaTime);

        // si la bala se sale de la pantalla, la destruyo
        if (transform.position.y > Camera.main.orthographicSize + 1f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // si choca con un enemigo, le activo la animacion de muerte
        if (other.CompareTag("Enemy"))
        {
            Animator anim = other.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("Dead");
            }

            Destroy(gameObject); // destruyo la bala al chocar
        }
        // si choca con un muro, le hago daño
        else if (other.CompareTag("Shield"))
        {
            if (other.TryGetComponent(out DestructibleShield shield))
            {
                shield.DamageAt(transform.position);
            }

            Destroy(gameObject); // destruyo la bala al impactar el muro
        }
        // si choca con otra bala, tambien la destruyo
        else if (other.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }
}