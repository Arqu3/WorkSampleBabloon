using UnityEngine;
using ThirdPerson.Player;

/// <summary>
/// Toolbox singleton-class, use this to set and get any public variables needed,
/// note that no more than 1 instance of this class should be present at any time!
/// </summary>
public class Toolbox : Singleton<Toolbox>
{
    //Make sure constructor cannot be used, true singleton
    protected Toolbox(){}

    //Gamestate variables
    bool m_Paused = false;

    //Player variables
    Transform m_PlayerTransform;

    public bool Paused
    {
        get
        {
           return m_Paused;
        }
    }

    public Transform PlayerTransform
    {
        get
        {
            return m_PlayerTransform ? m_PlayerTransform : m_PlayerTransform = FindObjectOfType<PlayerController> ().transform;
        }
    }

    //Toggle paused variable and set timescale depending on new value
    public void TogglePaused()
    {
        m_Paused = !m_Paused;
        Cursor.visible = m_Paused;
        Cursor.lockState = m_Paused ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = m_Paused ? 0.0f : 1.0f;
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    static public T REGISTERCOMPONENT<T>() where T : Component
    {
        return Instance.GetOrAddComponent<T>();
    }
}
