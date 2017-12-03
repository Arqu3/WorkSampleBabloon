using UnityEngine;
using ThirdPerson.Player;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Toolbox singleton-class, use this to set and get any public variables needed,
/// note that no more than 1 instance of this class should be present at any time!
/// </summary>
public class Toolbox : Singleton<Toolbox>
{
    //Make sure constructor cannot be used, true singleton
    protected Toolbox(){}

    //General game variables
    public int m_EnemiesKilled = 0;
    public float m_GameTime = 0.0f;

    //Gamestate variables
    bool m_GameOver = false;
    public UnityEvent m_GameOverEvent;
    public UnityEvent m_EnemyDiedEvent;

    //Player variables
    Transform m_PlayerTransform;

    void Update ()
    {
        m_GameTime += Time.deltaTime;

        if ( m_GameOver && Input.GetKeyDown (KeyCode.Return) )
        {
            SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
            Time.timeScale = 1.0f;
            m_GameOver = false;
        }
    }

    public bool IsGameOver
    {
        get
        {
           return m_GameOver;
        }
    }

    public int EnemiesKilled
    {
        get { return m_EnemiesKilled; }
    }

    public Transform PlayerTransform
    {
        get
        {
            return m_PlayerTransform ? m_PlayerTransform : m_PlayerTransform = FindObjectOfType<PlayerController> ().transform;
        }
    }

    void GameOver()
    {
        m_GameOver = true;
        Cursor.visible = m_GameOver;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0.0f;
    }

    void IncrementKilledEnemies()
    {
        ++m_EnemiesKilled;
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        m_GameOverEvent = new UnityEvent ();
        m_GameOverEvent.AddListener (GameOver);

        m_EnemyDiedEvent = new UnityEvent ();
        m_EnemyDiedEvent.AddListener (IncrementKilledEnemies);
    }

    static public T REGISTERCOMPONENT<T>() where T : Component
    {
        return Instance.GetOrAddComponent<T>();
    }
}
