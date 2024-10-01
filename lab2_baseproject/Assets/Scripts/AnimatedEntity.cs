using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedEntity : MonoBehaviour
{
    //Sprite List as right and Left for 
    public List<Sprite> DefaultAnimationCycleRight; //when moving right
    public List<Sprite> DefaultAnimationCycleLeft; //when moving left
    public Sprite IdleSprite; //when not moving
    public float Framerate = 12f;//frames per second
    public SpriteRenderer SpriteRenderer;//spriteRenderer

    //private animation stuff
    private float animationTimer;//current number of seconds since last animation frame update
    private float animationTimerMax;//max number of seconds for each frame, defined by Framerate
    private int index;//current index in the DefaultAnimationCycle
    

    //interrupt animation info
    private bool interruptFlag;
    private List<Sprite> interruptAnimation;
    private bool isMovingRight; // tracking direction
    private bool isMoving; //check if object is moving


    //Set up logic for animation stuff
    protected void AnimationSetup(){
        animationTimerMax = 1.0f/((float)(Framerate));
        index = 0;
        isMovingRight = true; // default direction
        isMoving = false; //object not moving initially
    }

    //Default animation update
    protected void AnimationUpdate(){
        animationTimer+=Time.deltaTime;

        if (isMoving) //when object starts moving
        {
            if (animationTimer > animationTimerMax)
            {
                animationTimer = 0;
                index++;

                // what script to show
                List<Sprite> currentAnimationCycle;
                if (isMovingRight) //if object moving right use RightSprite List
                {
                    currentAnimationCycle = DefaultAnimationCycleRight;
                }
                else
                {
                    currentAnimationCycle = DefaultAnimationCycleLeft;
                }

                if (!interruptFlag)
                {
                    if (currentAnimationCycle.Count == 0 || index >= currentAnimationCycle.Count)
                    {
                        index = 0;
                    }
                    if (currentAnimationCycle.Count > 0)
                    {
                        SpriteRenderer.sprite = currentAnimationCycle[index];
                    }
                }
                else
                {
                    if (interruptAnimation == null || index >= interruptAnimation.Count)
                    {
                        index = 0;
                        interruptFlag = false;
                        interruptAnimation = null;//clear interrupt animation
                    }
                    else
                    {
                        SpriteRenderer.sprite = interruptAnimation[index];
                    }
                }
            }
        }
        else //object not moving
        {
            SpriteRenderer.sprite = IdleSprite; //show the idel when not moving
        }
    }

    public void SetMovementDirection( bool movingRight, bool moving)
    {
        //bool values taken about movement and direction
        isMovingRight = movingRight;
        isMoving = moving;
    }

    //Interrupt animation
    protected void Interrupt(List<Sprite> _interruptAnimation){
        interruptFlag = true;
        animationTimer = 0;
        index = 0;
        interruptAnimation = _interruptAnimation;
        SpriteRenderer.sprite = interruptAnimation[index];
    }
    
}
