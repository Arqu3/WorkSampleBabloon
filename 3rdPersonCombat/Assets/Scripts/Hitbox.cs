using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThirdPerson.Entity;

namespace ThirdPerson.Damage
{
    /// <summary>
    /// Logical representation of a melee hitbox
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Hitbox : MonoBehaviour
    {
        #region Exposed fields

        #endregion

        #region Component fields

        #endregion

        #region Private fields

        GameObject m_Parent;
        bool m_Active = false;
        int m_Damage = 0;

        #endregion

        void Awake ()
        {
            m_Parent = transform.parent.gameObject;
        }

        public int Damage
        {
            set { m_Damage = value; }
        }

        public void Activate(float time)
        {
            StartCoroutine (DisableAfterTime (time));
        }

        IEnumerator DisableAfterTime(float time)
        {
            m_Active = true;
            yield return new WaitForSeconds (time);
            m_Active = false;
        }

        void OnTriggerEnter ( Collider other )
        {
            if ( other.gameObject == m_Parent || other.gameObject.layer == m_Parent.layer || !m_Active ) return;

            BaseEntity e = other.GetComponent<BaseEntity> ();
            if ( e ) e.ChangeHealh (-m_Damage);
        }
    }
}
