using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script for collectible objects:
public class Collectable : Colliderable
{
   protected bool collected = false;

 protected override void OnCollide(Collider2D coll)
 {
     if (coll.name == "Player")
         OnCollect();
 }

 protected virtual void OnCollect()
 {
     collected = true;
 }  
}

