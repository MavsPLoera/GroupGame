using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Light_Controller : MonoBehaviour
{
    // Light Controller
    // Creates light flickering.
    // SRC https://discussions.unity.com/t/how-to-randomly-change-the-intensity-of-a-point-light-with-a-script/12987/4

    [Header("Light Controller Misc.")]
    public float minIntensity;
    public float maxIntensity;
    public float speed;

    private Light2D _light;
    private float _random;

    private void Start()
    {
        _light = GetComponent<Light2D>();
        _random = Random.Range(0f, 65535f);
    }

    private void Update()
    {
        float noise = Mathf.PerlinNoise(_random, Time.time * speed);
        _light.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }

}
