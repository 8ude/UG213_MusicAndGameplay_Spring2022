using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beat;
using DG.Tweening;

//Similar behavior to the luminaria in electroplankton
public class HopperMove : MonoBehaviour
{
    public PositionToPitch posToPitch;

    //we're going to spawn audio prefabs instead of just playing the audio source
    //this is so we get the decay on our notes, and we don't change the pitch of a note while moving

    public GameObject audioPrefab;

    //our base interval (quarter note, eigth note, etc)
    public TickValue intervalBase;

    public double intervalCount = 1d;

    //intervalCount * intervalBase;
    public double intervalValue;

    double timeOfNextNote;

    public HopperDirector nextDirector;
    public HopperDirector prevDirector;


    public float distanceThreshold = 0.1f;

    float speed; //set after each hop;

    public Vector3 moveDirection;

    public bool hasStarted = false;

    [HideInInspector] public float xBoundLow, xBoundHigh;
    [HideInInspector] public float yBoundLow, yBoundHigh;


    public void InitHopperMove()
    {
        intervalValue = intervalCount * Clock.Instance.LengthOfD(intervalBase);

        timeOfNextNote = Clock.Instance.AtNext(intervalBase);

        nextDirector = prevDirector.targetObject.GetComponent<HopperDirector>();

        moveDirection = Vector3.Normalize(nextDirector.targetPosition - transform.position);

        speed = Vector3.Distance(nextDirector.targetPosition, transform.position) / Clock.Instance.LengthOf(intervalBase);
    }

    private void Start()
    {
        //when dealing with rhythmic timing, it's easy to run into "race conditions" - trying to access values that haven't yet been set.
        //so I've implemented this version to have a built-in 0.5 second start delay. This is a "magic number" variable which is bad coding practice; 
        //ideally we would know how much we need to wait from the clock script, or we would wait for the user to press a button
        StartCoroutine(StartDelay());
    }

    // Update is called once per frame
    void Update()
    {
         
        if (!hasStarted) return;

        transform.position += moveDirection * speed * Time.deltaTime;
        transform.position = CheckBoundary();

        //update our interval value in case it was changed manually
        intervalValue = Clock.Instance.LengthOfD(intervalBase) * intervalCount;

        if (AudioSettings.dspTime >= timeOfNextNote)
        {
            transform.position = nextDirector.transform.position;

            transform.DOPunchScale(new Vector3(1f, 1f, 1f), 0.2f);

            SetNewTarget();
            //this is a bit weird. once we hit a node, we're immediately going to spawn an audio prefab at our new target, and set it to play at the next interval.

            GameObject soundObject = Instantiate(audioPrefab, nextDirector.transform.position, Quaternion.identity);

            AudioSource soundSource = soundObject.GetComponent<AudioSource>();
            //this is because "PlayScheduled" is meant to be done ahead of time; we don't necessarily want to do it right as we arrive, which is what would happen if we connected it to our movement code.

            float nextPitch = posToPitch.PitchMapping(soundObject.transform.position);

            soundSource.pitch = nextPitch;

            timeOfNextNote += intervalValue;
            soundSource.PlayScheduled(timeOfNextNote);

            //contingency for pitch value of 0
            if (soundSource.pitch == 0f) soundSource.pitch = 1f;

            Destroy(soundObject, (float)intervalValue + (soundSource.clip.length / Mathf.Abs(soundSource.pitch)) + 0.2f); 
        }
    }


    void SetNewTarget()
    {
        prevDirector = nextDirector;
        
        moveDirection = Vector3.Normalize(prevDirector.targetPosition - transform.position);

        //speed is distance/time
        //we get the distance from our current position to the target, and we get our time from the clock script
        speed = Vector3.Distance(
            prevDirector.targetPosition, transform.position) 
            / 
            (float)intervalValue;

        nextDirector = prevDirector.targetObject.GetComponent<HopperDirector>();

        

    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(0.5f);
        {
            InitHopperMove();
        }
        hasStarted = true;
    }

    /// <summary>
    /// Use this for screen wrapping, put it just after the position update
    /// </summary>
    /// <returns></returns>
    Vector3 CheckBoundary()
    {
        Vector3 newPosition = transform.position;
        if (transform.position.x > xBoundHigh)
        {
            newPosition.x = xBoundLow;
        }
        if (transform.position.x < xBoundLow)
        {
            newPosition.x = xBoundHigh;
        }
        if (transform.position.y > yBoundHigh)
        {
            newPosition.y = yBoundLow;
        }
        if(transform.position.y < yBoundLow)
        {
            newPosition.y = yBoundHigh;
        }
        return newPosition;
    }
}
