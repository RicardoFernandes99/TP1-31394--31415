using UnityEngine;

public class LevarTiro : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("tiro"))
        {
            Destroy(collision.gameObject); // destrói o tiro
            Destroy(gameObject); // destrói este objeto
        }
    }
}
