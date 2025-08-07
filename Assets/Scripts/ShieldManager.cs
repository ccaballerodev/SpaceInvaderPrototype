using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este script se encarga de instanciar y colocar los escudos en la escena
public class ShieldManager : MonoBehaviour
{
    [Header("Configuración de los escudos")]
    public GameObject shieldPrefab;       // Prefab del escudo que se va a instanciar
    public int numberOfShields = 4;       // Cantidad total de escudos que se van a crear
    public float spacing = 2.5f;          // Distancia horizontal entre cada escudo
    public float shieldY = -2.5f;         // Posicion vertical fija donde se colocan los escudos

    void Start()
    {
        // Al iniciar la escena se crean los escudos
        SpawnShields();
    }

    // Este metodo se encarga de crear e instanciar los escudos en la escena
    void SpawnShields()
    {
        // Se calcula el ancho total ocupado por todos los escudos con el espaciado
        float totalWidth = (numberOfShields - 1) * spacing;

        // Se determina desde donde empezar a colocar los escudos (alineados al centro)
        float startX = -totalWidth / 2f;

        // Se instancia cada escudo con su posicion correspondiente
        for (int i = 0; i < numberOfShields; i++)
        {
            Vector3 position = new Vector3(startX + i * spacing, shieldY, 0f);
            Instantiate(shieldPrefab, position, Quaternion.identity, transform);
        }
    }
}
