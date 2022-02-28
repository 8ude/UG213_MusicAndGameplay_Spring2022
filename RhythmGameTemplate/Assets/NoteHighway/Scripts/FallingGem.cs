using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingGem : MonoBehaviour
{

    [HideInInspector] public double OkWindowStart;
    [HideInInspector] public double GoodWindowStart, PerfectWindowStart, PerfectWindowEnd, GoodWindowEnd, OkWindowEnd;
    [HideInInspector] public float crossingTime;

    public NoteHighwayWwiseSync wwiseSync;


    public enum CueState { Early = 0, OK = 1, Good = 2, Perfect = 3, Late = 4, AlreadyScored = 5}
    public CueState gemCueState;

    public Vector3 destination;

    private Vector3 velocity;

    //may be needed to make the game feel more fair
    public float crossPositionOffset;

    //debugging crossing sync issues
    private bool _gemCrossed = false;

    public string playerInput;


    public enum SustainType { none, start, end }
    [HideInInspector] public SustainType sustainType;

    Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;

        destination = GameObject.FindGameObjectWithTag("NowCrossing").transform.position;

        //we want to stay in the lane, so the destination will have the same x and y coordinates as the start.
        destination.x = transform.position.x;
        destination.y = transform.position.y;
        destination.z -= crossPositionOffset;

        //velocity = distance/time -- we want to make sure that the cue crosses our destination on beat
        velocity = (destination - transform.position) / (float)(0.001f*(crossingTime - wwiseSync.GetMusicTimeInMS()));

        gemCueState = CueState.Early;

        
    }

   
    void Update()
    {
        transform.Translate(velocity * Time.deltaTime);
        UpdateWindow();
    }

    public void UpdateWindow()
    {
        //check our cue state against Wwise event

        //for this case (more-or-less typical japanese rhythm game style), our detection windows are 
        // early - ok - good - perfect - good - ok - late

        switch (gemCueState)
        {

            case CueState.Early:
                //check to see if we've gotten to "ok"
                if (wwiseSync.GetMusicTimeInMS() > OkWindowStart)
                {
                    gemCueState = CueState.OK;
                }
                break;
            case CueState.OK:
                //check to see if we've gotten to "good"...
                if (wwiseSync.GetMusicTimeInMS() > GoodWindowStart && wwiseSync.GetMusicTimeInMS() < PerfectWindowStart)
                {
                    gemCueState = CueState.Good;

                }
                //... or maybe we're at the end of the last "ok" window
                else if (wwiseSync.GetMusicTimeInMS() > OkWindowEnd)
                {
                    gemCueState = CueState.Late;

                    //right now, our scoring system is checking every gem that's still in existence, so it's a good idea to render this inactive once it's crossed the "late" threshold
                }
                break;
            case CueState.Good:
                //check to see if we've gotten to "perfect"
                if (wwiseSync.GetMusicTimeInMS() > PerfectWindowStart && wwiseSync.GetMusicTimeInMS() < PerfectWindowEnd)
                {
                    gemCueState = CueState.Perfect;
                }
                //
                else if (wwiseSync.GetMusicTimeInMS() > GoodWindowEnd)
                {
                    gemCueState = CueState.OK;
                }
                break;
            case CueState.Perfect:
                if (wwiseSync.GetMusicTimeInMS() > PerfectWindowEnd)
                {
                    gemCueState = CueState.Good;
                }
                break;
            default:
                //if we're "late" or "already scored" there are no more potential state changes
                break;


        }
    }

    //remove late objects in a couple of seconds
    public void RemoveLateGem()
    {
        gemCueState = CueState.AlreadyScored;
        Destroy(gameObject, 2f);
    }

    public void GemScored()
    {
        gemCueState = CueState.AlreadyScored;
        //instantiate particles

        //destroy immediately
        Destroy(gameObject);
    }

    
}
