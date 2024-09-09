using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealthEntity 
{
   int MaxHealth { get; set; }

   int CurHealth {  get; set; }   

   bool _canIncrease {  get; set; }

   bool _canDecrease { get; set; }

   bool _isDead { get; set; }

   void TakeDamage(int damage); 

   void Healing(int heal);

    void Die();

}
