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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && targetedPlayer == null) {
            targetedPlayer = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (targetedPlayer != null)
        {
            if (collision.gameObject == targetedPlayer)
            {
                targetedPlayer = null;
            }
        }
    }




}
