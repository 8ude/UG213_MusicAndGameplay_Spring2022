  using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RhythmInput
{
    public KeyCode inputKey;
    public bool held = false;
    //corresponds to Input System button
    public string inputString;
}

/// <summary>
/// The purpose of this class is twofold:
/// - Get the Clock-synchronized timing of the user's input
/// - Check that against the windows of currently existing obstacles
/// </summary>
public class InputEvaluator : MonoBehaviour
{
    
    public List<FallingGem> activeGems;
    public List<RhythmInput> cachedInputs = new List<RhythmInput>();

    public GemGenerator gemGenerator;

    //ideally we'd manage score on a seperate script
    public int gameScore;

    public NoteHighwayWwiseSync wwiseSync;



    void Update()
    {
        //only used for debugging right now
        float wwiseTime = wwiseSync.GetMusicTimeInMS();

        //every frame, we do two things:
        //1: cache all of our inputs, so we know what the player pressed
        //2: evaluate every gem that's in play

        if (Input.GetButtonDown(gemGenerator.fallingGemR.playerInput))
        {
            //we make this RhythmInput class 
            RhythmInput _rhythmInput = new RhythmInput();
            _rhythmInput.inputString = gemGenerator.fallingGemR.playerInput;

            cachedInputs.Add(_rhythmInput);
        }

        if(Input.GetButtonDown(gemGenerator.fallingGemG.playerInput))
        {
            RhythmInput _rhythmInput = new RhythmInput();
            _rhythmInput.inputString = gemGenerator.fallingGemG.playerInput;
            cachedInputs.Add(_rhythmInput);
        }

        if(Input.GetButtonDown(gemGenerator.fallingGemB.playerInput))
        {
            RhythmInput _rhythmInput = new RhythmInput();
            _rhythmInput.inputString = gemGenerator.fallingGemB.playerInput;
            cachedInputs.Add(_rhythmInput);
        }

        //compare inputs to current beatMap windows

        //first find any non-destroyed cues

        FallingGem[] allGems = FindObjectsOfType<FallingGem>();

        activeGems.AddRange(allGems);
        for (int i = 0; i < activeGems.Count; i ++)
        {
            //we're not going to do anything with early inputs
            if (activeGems[i].gemCueState != FallingGem.CueState.Early)
            {
                
                //if player hasn't input anything, don't do anything
                if (cachedInputs.Count == 0)
                    break;
                //go through each of our inputs from this frame, and check them against this gem
                for (int j = 0; j < cachedInputs.Count; j++)
                {
                    if (cachedInputs[j].inputString == activeGems[i].playerInput)
                    {
                        ScoreGem(activeGems[i]);
                    }
                }
            }
        }

        //clear Lists
        activeGems.Clear();
        cachedInputs.Clear();
    }

    void ScoreGem(FallingGem gem)
    {
        switch (gem.gemCueState)
        {
            case FallingGem.CueState.OK:
                gameScore += 1;
                Debug.Log("OK!");
                gem.RemoveLateGem();
                break;
            case FallingGem.CueState.Good:
                gameScore += 2;
                Debug.Log("Good!");
                gem.RemoveLateGem();
                break;
            case FallingGem.CueState.Perfect:
                gameScore += 3;
                Debug.Log("Perfect!");
                gem.RemoveLateGem();
                break;
            case FallingGem.CueState.Late:
                Debug.Log("Missed!");
                gem.RemoveLateGem();
                break;
            case FallingGem.CueState.AlreadyScored:
                //don't do anything once we've scored the gem
                //this prevents a bunch of "misses" for the same gem, or confusing a past gem for a future one
                break;
        }


    }





}
