using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThirdPerson.Entity;
using UnityEngine.SceneManagement;

namespace ThirdPerson.Player
{
    /// <summary>
    /// Central player controller script
    /// </summary>
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
        LayerMask m_WallMask = Physics.AllLayers;

        #endregion

        #region Component fields

        Rigidbody m_Body;
        Collider m_Collider;

        #endregion

        #region Private fields

        //Stat variables
        int m_CurrentHealth = 0;
        int m_CurrentDamage = 0;

        //Air variables
        bool m_JumpCooldown = false;
        bool m_Grounded = false;

        //Rotation variables
        Quaternion m_Rotation;

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
            if ( Input.GetKeyDown (KeyCode.Escape) ) Application.Quit ();

            Move ();

            if ( Input.GetMouseButton (0) ) Attack ();
        }

        public override void ChangeHealh ( int amount )
        {
            m_CurrentHealth = Mathf.Clamp (m_CurrentHealth + amount, 0, m_BaseHealth);
            if ( m_CurrentHealth <= 0 ) Kill ();
        }

        public override void Kill ()
        {
            SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
        }

        protected override void Attack ()
        {
        }

        protected override void Move ()
        {
            m_Grounded = Grounded;
            m_Body.useGravity = !m_Grounded;

            if (Input.GetKey(KeyCode.Space))
            {
                if ( m_Grounded ) Jump (Vector3.up * m_JumpForce);
            }

            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0.0f;
            m_Rotation = Quaternion.LookRotation (forward, Vector3.up);

            if ( InputActive ) transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation (forward, Vector3.up), 10.0f * Time.deltaTime);

            Vector3 input = m_Rotation * new Vector3 (Input.GetAxis ("Horizontal"), 0.0f, Input.GetAxis ("Vertical"));
            if ( WallCheck (input) ) input = Vector3.zero;

            if (Grounded) m_Body.velocity = new Vector3 (input.x * m_MovementSpeed, m_Body.velocity.y, input.z * m_MovementSpeed);
        }

        bool Grounded
        {
            get
            {
                return Physics.Raycast (m_Collider.bounds.center, Vector3.down, m_Collider.bounds.size.y / 2.0f * 1.1f, m_GroundMask, QueryTriggerInteraction.Ignore);
            }
        }

        bool WallCheck(Vector3 direction)
        {
            int numRaycasts = 5;
            Vector3 pos = m_Collider.bounds.center - new Vector3 (0.0f, m_Collider.bounds.size.y / 2.0f, 0.0f);
            float height = m_Collider.bounds.size.y / numRaycasts;
            RaycastHit hit;
            for ( int i = 0 ; i < numRaycasts + 1 ; ++i )
            {
                //Debug.DrawRay (pos + new Vector3 (0.0f, height * i, 0.0f), direction, Color.red);
                if ( Physics.Raycast (pos + new Vector3 (0.0f, height * i, 0.0f), direction, out hit, m_Collider.bounds.extents.x * 1.1f, m_WallMask, QueryTriggerInteraction.Ignore) )
                {
                    if (Vector3.Angle(hit.normal, Vector3.up) >= 89f) return true;
                }
            }
            return false;
        }

        bool InputActive
        {
            get
            {
                return Mathf.Abs (Input.GetAxis ("Horizontal")) > 0.0f || Mathf.Abs (Input.GetAxis ("Vertical")) > 0.0f;
            }
        }

        void Jump(Vector3 force)
        {
            if ( m_JumpCooldown ) return;
            StartCoroutine (JumpCooldown (0.1f));
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