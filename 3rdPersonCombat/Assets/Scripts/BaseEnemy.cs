using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThirdPerson.Entity;
using UnityEngine.AI;
using ThirdPerson.Damage;

namespace ThirdPerson.Enemy
{
    /// <summary>
    /// Basic enemy
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent), typeof(Collider), typeof(Rigidbody))]
    public class BaseEnemy : BaseEntity
    {
        #region Exposed fields

        [Header ("Movement variables")]
        [SerializeField]
        [Range (0.0f, 50.0f)]
        float m_MovementSpeed = 10.0f;

        [Header("Attack variables")]
        [SerializeField]
        [Range (0.0f, 50.0f)]
        float m_AttackRange = 20.0f;
        [SerializeField]
        [Range (0, 50)]
        int m_Damage = 10;

        [Header ("Spawnable prefabs")]
        [SerializeField]
        GameObject m_BulletPrefab;

        #endregion

        #region Component fields

        Collider m_Collider;
        NavMeshAgent m_Agent;
        Rigidbody m_Body;

        #endregion

        #region Private fields

        Transform m_PlayerTransform;

        List<GameObject> m_Bullets = new List<GameObject> ();

        float m_FireTime = 1.0f;
        float m_FireTimer = 0.0f;

        #endregion

        void Awake ()
        {
            m_Collider = GetComponent<Collider> ();
            m_Agent = GetComponent<NavMeshAgent> ();
            m_Body = GetComponent<Rigidbody> ();

            //Spawn initial pooled bullets
            for (int i = 0 ; i < 5 ; ++i )
            {
                GameObject bullet = Instantiate (m_BulletPrefab);
                AddBullet (bullet);
                bullet.SetActive (false);
            }
        }

        void Start ()
        {
            m_PlayerTransform = Toolbox.Instance.PlayerTransform;
            m_Agent.speed = m_MovementSpeed;
            m_Agent.stoppingDistance = m_AttackRange;
        }

        void Update ()
        {
            Vector3 dir = m_PlayerTransform.position - transform.position;
            if ( dir.sqrMagnitude < m_AttackRange * m_AttackRange )
            {
                Attack ();
                transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation (dir, Vector3.up), 10f * Time.deltaTime);
            }
            else Move ();
        }

        public override void ChangeHealh ( int amount )
        {
            Kill ();
        }

        public override void Kill ()
        {
            Toolbox.Instance.m_EnemyDiedEvent.Invoke ();
            gameObject.SetActive (false);
        }

        protected override void Attack ()
        {
            m_FireTimer += Time.deltaTime;
            if (m_FireTimer >= m_FireTime)
            {
                m_FireTimer = 0.0f;
                Shoot ();
            }
        }

        void Shoot()
        {
            Bullet bullet = GetBulletObject ().GetComponent<Bullet> ();
            bullet.gameObject.transform.position = transform.position;
            bullet.AddForce((m_PlayerTransform.position - transform.position).normalized * 20.0f);
        }

        GameObject GetBulletObject()
        {
            for (int i = 0 ; i < m_Bullets.Count ; ++i )
            {
                if (!m_Bullets[i].gameObject.activeSelf)
                {
                    m_Bullets[i].gameObject.SetActive (true);
                    return m_Bullets[i];
                }
            }

            GameObject bullet = Instantiate (m_BulletPrefab);
            AddBullet (bullet);
            return bullet;
        }

        void AddBullet(GameObject gObj)
        {
            Bullet bullet = gObj.GetComponent<Bullet> ();
            bullet.Damage = m_Damage;
            m_Bullets.Add (gObj);
        }

        protected override void Move ()
        {
            NavMeshHit hit;
            if ( NavMesh.SamplePosition (m_PlayerTransform.position, out hit, 10.0f, NavMesh.AllAreas) ) m_Agent.SetDestination (hit.position);
        }
    }
}