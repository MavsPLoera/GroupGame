using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Light_Controller : MonoBehaviour
{
    // Creates light flickering.
    // SRC https://discussions.unity.com/t/how-to-randomly-change-the-intensity-of-a-point-light-with-a-script/12987/4

    public Light2D light;
    public float minIntensity;
    public float maxIntensity;
    public float speed;

    private float _random;

    void Start()
    {
        light = GetComponent<Light2D>();
        _random = Random.Range(0f, 65535f);
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(_random, Time.time * speed);
        light.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}
