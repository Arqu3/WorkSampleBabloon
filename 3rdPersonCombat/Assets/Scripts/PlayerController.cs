using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThirdPerson.Entity;

namespace ThirdPerson.Player
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class PlayerController : BaseEntity
    {
        #region Exposed fields

        [Header ("Stat variables")]
        [SerializeField]
        [Range (0, 200)]
        int m_BaseHealth = 100;
        [SerializeField]
        [Range (0, 100)]
        int m_BaseDamage = 30;

        [Header ("Movement variables")]
        [SerializeField]
        [Range (0.0f, 50.0f)]
        float m_MovementSpeed = 15.0f;
        [SerializeField]
        [Range (0.0f, 50.0f)]
        float m_JumpForce = 15.0f;

        [Header ("Raycast variables")]
        [SerializeField]
        LayerMask m_GroundMask = Physics.AllLayers;

        #endregion

        #region Component fields

        Rigidbody m_Body;
        Collider m_Collider;

        #endregion

        #region Private fields

        int m_CurrentHealth = 0;
        int m_CurrentDamage = 0;
        bool m_JumpCooldown = false;

        #endregion

        void Awake ()
        {
            m_Body = GetComponent<Rigidbody> ();
            m_Collider = GetComponent<Collider> ();
        }

        void Start ()
        {
            m_CurrentDamage = m_BaseDamage;
            m_CurrentHealth = m_BaseHealth;
        }

        void Update ()
        {
            Move ();
        }

        public override void ChangeHealh ( int amount )
        {
            m_CurrentHealth = Mathf.Clamp (m_CurrentHealth + amount, 0, m_BaseHealth);
            if ( m_CurrentHealth <= 0 ) Kill ();
        }

        public override void Kill ()
        {
            
        }

        protected override void Attack ()
        {
        }

        protected override void Move ()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if ( Grounded ) Jump (Vector3.up * m_JumpForce);
            }
        }

        bool Grounded
        {
            get
            {
                return Physics.Raycast (m_Collider.bounds.center, Vector3.down, m_Collider.bounds.size.y / 2.0f * 1.1f, m_GroundMask, QueryTriggerInteraction.Ignore);
            }
        }

        void Jump(Vector3 force)
        {
            if ( m_JumpCooldown ) return;
            StartCoroutine (JumpCooldown (0.5f));
            m_Body.AddForce (force, ForceMode.Impulse);
        }

        IEnumerator JumpCooldown(float time)
        {
            m_JumpCooldown = true;
            yield return new WaitForSeconds (time);
            m_JumpCooldown = false;
        }
    }
}