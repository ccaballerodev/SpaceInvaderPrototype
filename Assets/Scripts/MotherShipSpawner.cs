using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherShipSpawner : MonoBehaviour
{
    [Header("Prefab Mother Ship")]
    public GameObject mothershipPrefab;        // Prefab que se va a instanciar como nave nodriza

    [Header("Interval of occurrence")]
    public float minSpawnTime = 15f;           // Tiempo minimo de espera entre cada aparicion de la nave
    public float maxSpawnTime = 30f;           // Tiempo maximo de espera entre apariciones

    void Start()
    {
        // Inicia la corrutina que controla las apariciones repetidas de la nave nodriza
        StartCoroutine(SpawnLoop());
    }

    // Corrutina que ejecuta un bucle infinito para instanciar la nave nodriza cada cierto tiempo
    IEnumerator SpawnLoop()
    {
        while (true)
        {
            // Espera un tiempo aleatorio entre el minimo y maximo antes de instanciar
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            // Crea una nueva nave nodriza en la escena
            Instantiate(mothershipPrefab);
        }
    }
}

