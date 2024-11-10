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
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        cmvirtualcamera = GetComponent<CinemachineVirtualCamera>();

    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log("function update called!");
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            cmvirtualcamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain =
            Mathf.Lerp(startingIntensity, 0f, (1 - (shakeTimer / shakeDuration)));
        }
    }
    public void ShakeCamera(float intensity, float time)
    {
        Debug.Log("function called!");
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cmvirtualcamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        shakeTimer = time;
        shakeDuration = time;
    }
}