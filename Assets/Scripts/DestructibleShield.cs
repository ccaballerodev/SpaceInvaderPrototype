using System.Collections.Generic;
using UnityEngine;

// Este script es para el escudo que se puede destruir por partes, pixel por pixel
public class DestructibleShield : MonoBehaviour
{
    private SpriteRenderer sr; // para cambiar la imagen del escudo en tiempo real
    private Sprite shieldSprite; // sprite que vamos a modificar
    private Texture2D textureCopy; // copia editable de la textura del sprite
    private List<BoxCollider2D> pixelColliders = new List<BoxCollider2D>(); // colliders que se adaptan a los pixeles visibles

    void Start()
    {
        sr = GetComponent<SpriteRenderer>(); // tomo el SpriteRenderer

        // se hace una copia de la textura original para poder modificarla
        Texture2D originalTexture = sr.sprite.texture;
        textureCopy = Instantiate(originalTexture);
        textureCopy.filterMode = FilterMode.Point; // para que no se vea borroso
        textureCopy.wrapMode = TextureWrapMode.Clamp; // que no se repita
        textureCopy.Apply();

        // se crea un nuevo sprite con esa textura modificable
        shieldSprite = Sprite.Create(
            textureCopy,
            new Rect(0, 0, textureCopy.width, textureCopy.height),
            new Vector2(0.5f, 0.5f),
            30f
        );
        sr.sprite = shieldSprite;

        // se genera los colliders segun los pixeles visibles
        RegenerarCollidersPorFilas();
    }

    // esta funcion se llama cuando algo golpea el escudo
    public void DamageAt(Vector2 worldHitPoint)
    {
        // se convierte el punto donde se pego al espacio local del objeto
        Vector2 localPoint = transform.InverseTransformPoint(worldHitPoint);
        Bounds bounds = sr.sprite.bounds;

        float spriteWidth = bounds.size.x;
        float spriteHeight = bounds.size.y;

        // se convierte el punto local a coordenadas UV (de 0 a 1)
        float uvX = (localPoint.x - bounds.min.x) / spriteWidth;
        float uvY = (localPoint.y - bounds.min.y) / spriteHeight;

        uvX = Mathf.Clamp01(uvX);
        uvY = Mathf.Clamp01(uvY);

        // se pasa las coordenadas UV a coordenadas de pixel
        int px = Mathf.FloorToInt(uvX * textureCopy.width);
        int py = Mathf.FloorToInt(uvY * textureCopy.height);

        int radius = 10; // radio de destruccion

        // recorre los pixeles dentro del radio
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                int tx = px + x;
                int ty = py + y;

                // Se valida que no se salga de la textura
                if (tx >= 0 && tx < textureCopy.width && ty >= 0 && ty < textureCopy.height)
                {
                    float distance = Mathf.Sqrt(x * x + y * y);
                    if (distance <= radius)
                    {
                        // para que no sea perfectamente circular, le meto algo de aleatoriedad
                        float randomness = Random.Range(0f, 1f);
                        float threshold = Mathf.InverseLerp(radius, 0, distance);

                        if (randomness <= threshold + 0.1f)
                        {
                            // se borra el pixel (lo hacemos transparente)
                            textureCopy.SetPixel(tx, ty, new Color(0, 0, 0, 0));
                        }
                    }
                }
            }
        }

        textureCopy.Apply(); // se aplica los cambios en la textura

        // se crea un nuevo sprite con la textura actualizada
        sr.sprite = Sprite.Create(
            textureCopy,
            new Rect(0, 0, textureCopy.width, textureCopy.height),
            new Vector2(0.5f, 0.5f),
            30f
        );

        // se actualiza los colliders segun la nueva forma del escudo
        RegenerarCollidersPorFilas();

        // si ya no queda ningun pixel visible, se destruye el objeto
        if (IsTextureEmpty())
        {
            Destroy(gameObject);
        }
    }

    // esta funcion revisa la textura y genera colliders solo en las filas donde hay pixeles visibles
    private void RegenerarCollidersPorFilas()
    {
        // se borra los colliders anteriores
        foreach (BoxCollider2D col in pixelColliders)
            Destroy(col);
        pixelColliders.Clear();

        float pixelsPerUnit = 30f;
        float pixelSize = 1f / pixelsPerUnit;

        for (int y = 0; y < textureCopy.height; y++)
        {
            int startX = -1;

            for (int x = 0; x < textureCopy.width; x++)
            {
                Color pixel = textureCopy.GetPixel(x, y);
                bool visible = pixel.a > 0.1f;

                if (visible && startX == -1)
                {
                    // empiezo un nuevo bloque de colision
                    startX = x;
                }
                else if (!visible && startX != -1)
                {
                    // cierro el bloque y creamos un collider
                    CrearColliderDeBloque(startX, x - 1, y, pixelSize);
                    startX = -1;
                }
            }

            // si llega al final de la fila y hay un bloque abierto, lo cerramos
            if (startX != -1)
            {
                CrearColliderDeBloque(startX, textureCopy.width - 1, y, pixelSize);
            }
        }
    }

    // crea un collider que cubre un bloque horizontal de pixeles
    private void CrearColliderDeBloque(int x0, int x1, int y, float pixelSize)
    {
        int ancho = x1 - x0 + 1;
        Vector2 centro = new Vector2((x0 + x1 + 1) / 2f * pixelSize, (y + 0.5f) * pixelSize);
        centro -= new Vector2(textureCopy.width, textureCopy.height) * pixelSize * 0.5f;

        BoxCollider2D col = gameObject.AddComponent<BoxCollider2D>();
        col.offset = centro;
        col.size = new Vector2(ancho * pixelSize, pixelSize);
        pixelColliders.Add(col);
    }

    // este metodo revisa si ya no queda ningun pixel visible
    private bool IsTextureEmpty()
    {
        Color[] pixels = textureCopy.GetPixels();
        foreach (Color pixel in pixels)
        {
            if (pixel.a > 0.01f)
                return false;
        }
        return true;
    }
}