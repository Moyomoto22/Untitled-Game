using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VCamManager : MonoBehaviour
{
    public CinemachineInputProvider InputProvider;
    public bool isInput;
    void Update()
    {
        InputProvider.enabled = isInput;
    }
}