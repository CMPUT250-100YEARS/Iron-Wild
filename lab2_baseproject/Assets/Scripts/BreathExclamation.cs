using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathExclamation : MonoBehaviour
{
    public SpriteRenderer spriterender;
    public Sprite sprite1;
    public Sprite sprite2;
    public Sprite sprite3;

    public GameObject target;

    private int urgency;

    private Vector3 baseZoom;
    private Vector3 growZoom;
    private Vector3 regular;

    private float timer;
    private float rotation;
    private float maxrotate;
    private float regular_inc = 0.0012f;
    private float fast_inc = 0.002f;
    private bool wiggling = false;
    private bool shrinkable = true;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(0f, 0f, 0f);
        spriterender.sprite = sprite1;
        urgency = 0;
        rotation = 0;
        growZoom = new Vector3(0.085f, 0.085f, 1f);
        baseZoom = new Vector3(0.05f, 0.05f, 1f);
        regular = new Vector3(regular_inc, regular_inc, 1f);
        //StartCoroutine(sizeup(regular_inc));
    }

    // Update is called once per frame
    void Update()
    {
        //center on the water bar
        if (target != null)
        {
            Vector3 newPosition = new Vector3(target.transform.position.x + 7.4f, target.transform.position.y + 9.25f, 0f);
            transform.position = newPosition;
        }
    }


    public void down_level()
    {
        if (urgency == 3)
        {
            urgency = 2;
            timer = 1.2f;
            spriterender.sprite = sprite2;
        }
        else if (urgency == 2)
        {
            urgency = 1;
            timer = 3;
            spriterender.sprite = sprite1;
        }
        else if (urgency == 1)
        {
            urgency = 0;
            wiggling = false;
            StartCoroutine(shrink());
        }
    }

    public void up_level()
    {
        if (urgency == 0)
        {
            urgency = 1;
            timer = 3;
            spriterender.sprite = sprite1;
            StartCoroutine(wiggle());
            shrinkable = false;
            StartCoroutine(sizeup(fast_inc));
        }
        else if (urgency == 1)
        {
            urgency = 2;
            timer = 1.2f;
            spriterender.sprite = sprite2;
            shrinkable = false;
            StartCoroutine(sizeup(regular_inc));
        }
        else if (urgency == 2)
        {
            urgency = 3;
            timer = 0.25f;
            spriterender.sprite = sprite3;
            shrinkable = false;
            StartCoroutine(sizeup(regular_inc));
        }
    }


    private IEnumerator wiggle()
    {
        wiggling = true;
        while (urgency > 0 && wiggling)
        {
            yield return new WaitForSeconds(timer);

            maxrotate = 25f;
            while (maxrotate > 2)
            {
                while (rotation < maxrotate)
                {
                    rotation += 1;
                    transform.rotation = Quaternion.Euler(0, 0, rotation);
                    yield return null;
                }
                maxrotate /= 1.5f;

                while (rotation > -maxrotate)
                {
                    rotation -= 1;
                    transform.rotation = Quaternion.Euler(0, 0, rotation);
                    yield return null;
                }
                maxrotate /= 1.5f;
            }
            transform.rotation = Quaternion.Euler(0, 0, 0);
            rotation = 0;

            yield return new WaitForSeconds(timer);
            if (wiggling) StartCoroutine(sizeup(regular_inc));
        }
        wiggling = false;
    }

    private IEnumerator sizeup(float increment)
    {
        shrinkable = true;
        Vector3 incr_amount = new Vector3(increment, increment, 0f);

        while (transform.localScale.x < growZoom.x && shrinkable)
        {
            transform.localScale += incr_amount;
            yield return null;
        }

        while (transform.localScale.x > baseZoom.x && shrinkable)
        {
            transform.localScale -= regular;
            yield return null;
        }

        transform.localScale = baseZoom;
    }


    private IEnumerator shrink()
    {
        while (transform.localScale.x > 0f)
        {
            transform.localScale -= regular;
            yield return null;
        }

        transform.localScale = new Vector3(0, 0, 0);
    }


    public void Restart()
    {
        // Reset volume
        urgency = 0;
        timer = 3;
        wiggling = false;
    }


}