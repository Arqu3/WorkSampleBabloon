using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ThirdPerson.Enemy;

namespace ThirdPerson.Game
{
    /// <summary>
    /// Handles the state of the game
    /// </summary>
    public class GameController : MonoBehaviour
    {
        #region Exposed fields

        [Header ("Spawnable Prefabs")]
        [SerializeField]
        GameObject m_EnemyPrefab;

        [Header ("Enemy spawnpoints")]
        [SerializeField]
        Transform[] m_Spawnpoints;

        [Header ("Game over variables")]
        [SerializeField]
        Text[] m_Texts;

        public static GameController Instance
        {
            get { return m_Instance; }
            private set
            {
                if (m_Instance != null)
                {
                    Debug.LogError ("An assign attempt to override the current instance of the gamecontroller was made, this should not be possible!");
                    return;
                }
                m_Instance = value;
            }
        }

        #endregion

        #region Component fields

        #endregion

        #region Private fields

        static GameController m_Instance = null;

        List<GameObject> m_Enemies = new List<GameObject> ();

        #endregion

        void Awake ()
        {
            Instance = this;

            for (int i = 0 ; i < 10 ; ++i )
            {
                GameObject enemy = Instantiate (m_EnemyPrefab);
                m_Enemies.Add (enemy);
                enemy.SetActive (false);
            }
        }

        void Start ()
        {
            Toolbox.Instance.m_GameOverEvent.AddListener (GameOver);

            for (int i = 0 ; i < 2 ; ++i )
            {
                StartCoroutine (SpawnEnemy (2.0f, 5.0f));
            }
        }

        IEnumerator SpawnEnemy(float minTime, float maxTime)
        {
            float time = Random.Range (minTime, maxTime);
            yield return new WaitForSeconds (time);

            BaseEnemy enemy = GetEnemyObject ().GetComponent<BaseEnemy> ();
            enemy.transform.position = m_Spawnpoints[Random.Range (0, m_Spawnpoints.Length)].position;

            StartCoroutine (SpawnEnemy (minTime, maxTime));
        }

        GameObject GetEnemyObject()
        {
            for (int i = 0 ; i < m_Enemies.Count ; ++i )
            {
                if (!m_Enemies[i].activeSelf)
                {
                    m_Enemies[i].SetActive (true);
                    return m_Enemies[i];
                }
            }

            GameObject enemy = Instantiate (m_EnemyPrefab);
            m_Enemies.Add (enemy);
            return enemy;
        }

        void GameOver()
        {
            m_Texts[0].transform.parent.gameObject.SetActive (true);

            m_Texts[0].text = "You survived: " + Toolbox.Instance.m_GameTime.ToString ("F2") + " seconds.";
            m_Texts[1].text = "You killed: " + Toolbox.Instance.m_EnemiesKilled + " enemies.";
            m_Texts[2].text = "Press ESC to quit or RETURN to play again.";
        }

        void Update ()
        {
            if ( Input.GetKeyDown (KeyCode.Escape) ) Application.Quit ();
        }
    }
}