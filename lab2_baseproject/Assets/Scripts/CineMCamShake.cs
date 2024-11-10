using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CineMCamShake : MonoBehaviour
{
    public static CineMCamShake Instance { get; private set; }
    private CinemachineVirtualCamera cmvirtualcamera;
    private float shakeTimer;
    private float startingIntensity;
    private float shakeDuration;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate instances
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        cmvirtualcamera = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeTimer > 0)
        {
            Debug.Log("In here!");
            shakeTimer -= Time.deltaTime;
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                cmvirtualcamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain =
                Mathf.Lerp(startingIntensity, 0f, (1 - (shakeTimer / shakeDuration))*1.20f);
        }
    }

    public void ShakeCamera(float intensity, float time)
    {
        Debug.Log("Shake function called with " + intensity + " " + time);
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            cmvirtualcamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

        shakeTimer = time;
        shakeDuration = time;
        startingIntensity = intensity;
    }
}
