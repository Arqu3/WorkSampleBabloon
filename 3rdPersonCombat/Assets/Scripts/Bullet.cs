using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThirdPerson.Player;

namespace ThirdPerson.Damage
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        #region Exposed fields

        #endregion

        #region Component fields

        Rigidbody m_Body;

        #endregion

        #region Private fields

        int m_Damage = 0;

        #endregion

        void Awake ()
        {
            m_Body = GetComponent<Rigidbody> ();
        }

        public int Damage
        {
            get { return m_Damage; }
            set { m_Damage = value; }
        }

        public void AddForce(Vector3 force)
        {
            StartCoroutine (DisableAfterTime (3f));
            m_Body.velocity = Vector3.zero;
            m_Body.AddForce (force, ForceMode.Impulse);
        }

        IEnumerator DisableAfterTime(float time)
        {
            yield return new WaitForSeconds (time);

            gameObject.SetActive (false);
        }

        private void OnTriggerEnter ( Collider other )
        {
            if ( !gameObject.activeSelf ) return;
            if ( other.isTrigger ) return;
            if ( other.gameObject.layer == gameObject.layer ) return;

            PlayerController player = other.GetComponent<PlayerController> ();
            if ( player ) player.Damage (m_Damage);
            gameObject.SetActive (false);
        }
    }
}