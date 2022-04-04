using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerElevationToWwise : MonoBehaviour
{
    public AK.Wwise.RTPC playerElevationParam;

    // Update is called once per frame
    void Update()
    {
        playerElevationParam.SetGlobalValue(transform.position.y);
    }
}
