using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [Tooltip("Destruir a bala ao fim deste tempo caso não colida com nada")]
    [SerializeField] private float lifeTime = 5f;

    void Start()
    {
        // auto-destruição para não ficar no cenário para sempre
        Destroy(gameObject, lifeTime);
    }

    // quando a bala bate em qualquer coisa com Collider
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("tiro"))
        {
            // se a bala colidir com outra bala, ignora
            return;
        }
        Destroy(gameObject);
    }
}
