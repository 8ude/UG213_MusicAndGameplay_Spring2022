using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//switches the audio clip based on the xy position of the object
public class PositionToClip : MonoBehaviour
{

    public float minX = -10;
    public float maxX = 10;
    public float minY = -10;
    public float maxY = 10;

    [Tooltip("recommend to keep this set to true")]
    public bool preventClipChangeWhilePlaying = true;

    public AudioSource aSource;

    // Start is called before the first frame update
    void Start()
    {
        if (minX > maxX || minY > maxY)
        {
            Debug.LogError("ERROR - min bounds are greater than max bounds!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (aSource.isPlaying) return;

        //find min 


    }
}
