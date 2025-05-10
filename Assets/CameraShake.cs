using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;   // acesso global rápido

    Vector3 posOriginal;

    void Awake()
    {
        Instance = this;
        posOriginal = transform.localPosition;
    }

    public void Shake(float dur, float mag)
    {
        StartCoroutine(ShakeRoutine(dur, mag));
    }

    IEnumerator ShakeRoutine(float dur, float mag)
    {
        float t = 0f;
        while (t < dur)
        {
            float x = Random.Range(-1f, 1f) * mag;
            float y = Random.Range(-1f, 1f) * mag;
            transform.localPosition = posOriginal + new Vector3(x, y, 0);

            t += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = posOriginal;
    }
}
