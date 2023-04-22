using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEnemy : Enemy
{

    private bool inAttackAnimation = false;

    private void Awake()
    {
        hitPoints = 2;
        contactDamage = 1;
        detectionObjectTransform.transform.localScale = detectionObjectScale;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //DetectPlayer();
        Movement();
    }

    protected override void Movement()
    {
        if(targetedPlayer && !inAttackAnimation)
        {
            //play the attack animation
            //at the end move the slime towards the player
        }
    }

    //It will always detect itself as being inside the detection layer since the detection layer is including the slime hitbox
    //I need to basically check to see if the detection is including the slime instead of the detection layer probably
    //Otherwise it works.

    private void OnTriggerEnter2D(Collider2D collision)
    {

        LayerMask mask = LayerMask.GetMask("Detection");
        if(collision.IsTouchingLayers(mask))
        {
            Debug.Log("Touching Detection Layer!");
        }



        Debug.Log("Trigger Entered");

        if(collision.gameObject.CompareTag("Player") && targetedPlayer == null) {
            targetedPlayer = collision.gameObject;
            Debug.Log("Slime collided with the player");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (targetedPlayer != null)
        {
            if (collision.gameObject == targetedPlayer)
            {
                targetedPlayer = null;
                Debug.Log("Slime is done colliding with the player");
            }
        }
    }




}
