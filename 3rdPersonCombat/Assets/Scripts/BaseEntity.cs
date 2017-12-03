using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThirdPerson.Entity
{
    /// <summary>
    /// Base class for all entities
    /// </summary>
    public abstract class BaseEntity : MonoBehaviour
    {
        protected abstract void Move ();
        protected abstract void Attack ();

        public abstract void ChangeHealh ( int amount );
        public abstract void Kill ();
    }
}