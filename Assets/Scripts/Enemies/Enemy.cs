using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Enemy : MonoBehaviour
{

    protected int hitPoints = 1;
    protected int? contactDamage; //Set this if there's contact damage
    protected bool canSeePlayer = false; //Can this monster see the player?
    protected GameObject targetedPlayer; //The player this monster is targeting

    [SerializeField] protected GameObject detectionObject; //This is an invisible object attached to the monster that detects players on collision
    [SerializeField] protected Transform detectionObjectTransform; //This is the transform of said object.
    [SerializeField] protected Vector3 detectionObjectScale; //This is the scale of said object.

    abstract protected void Movement();
    //abstract protected void DetectPlayer();

}
