using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedEntity : MonoBehaviour
{
    
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
    private bool isMoving; //check if object is moving

    private List<Sprite> currentAnimationCycle;


    //Set up logic for animation stuff
    protected void AnimationSetup(){
        animationTimerMax = 1.0f/((float)(Framerate));
        index = 0;
        isMoving = false; //object not moving initially
    }

    //Player or Enemy pass the animationCycle
    public void SetCurrentAnimationCycle(List<Sprite> animationCycle)
    {
        currentAnimationCycle = animationCycle;
    }

    //Default animation update
    protected void AnimationUpdate(){
        animationTimer += Time.deltaTime;

        if (isMoving) //when object starts moving
        {
            if (animationTimer > animationTimerMax)
            {
                animationTimer = 0;
                index++;

                // check current animation cycle is not empty         
                if (!interruptFlag && currentAnimationCycle != null) 
                {
                    if (currentAnimationCycle.Count == 0 || index >= currentAnimationCycle.Count) 
                    {
                        index = 0; //start back to the beginning 
                    }

                    if (currentAnimationCycle.Count > 0)
                    {
                        SpriteRenderer.sprite = currentAnimationCycle[index]; //update the sprite
                    }
                }
                else if (interruptFlag) //interupt animation
                {
                    if (interruptAnimation == null || index >= interruptAnimation.Count)
                    {
                        index = 0;
                        interruptFlag = false;
                        interruptAnimation = null;
                    }
                    else
                    {
                        SpriteRenderer.sprite = interruptAnimation[index]; //update interrupt sprite
                    }
                }                                           
            }
        }
        else //object not moving
        {
            SpriteRenderer.sprite = IdleSprite; //show the idel when not moving
        }
    }

    public void SetMovementDirection(bool moving)
    {
        //bool values taken about movement
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
