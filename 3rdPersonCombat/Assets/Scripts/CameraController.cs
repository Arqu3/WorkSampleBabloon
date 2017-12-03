using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThirdPerson.CameraControll
{
    /// <summary>
    /// Controlls camera movement
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        #region Exposed fields

        [Header ("Offset variables")]
        [SerializeField]
        Vector3 m_Offset = Vector3.zero;

        [Header ("Input variables")]
        [SerializeField]
        Vector2 m_Sensitivity = new Vector2 (5.0f, 5.0f);
        [SerializeField]
        [Range (0.0f, 180.0f)]
        float m_ClampY = 80.0f;

        #endregion

        #region Private fields

        //Rotation variables
        float m_AbsoluteX = 0.0f;
        float m_AbsoluteY = 45.0f;

        //Position variables
        Transform m_PlayerTransform;

        #endregion

        void Start ()
        {
            m_PlayerTransform = Toolbox.Instance.PlayerTransform;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update ()
        {
            m_AbsoluteX += Input.GetAxis ("Mouse X") * m_Sensitivity.x * Time.deltaTime;
            if ( m_AbsoluteX > 360.0f ) m_AbsoluteX -= 360.0f;
            else if ( m_AbsoluteX < -360.0f ) m_AbsoluteX += 360.0f;

            m_AbsoluteY = Mathf.Clamp (m_AbsoluteY + Input.GetAxis ("Mouse Y") * -1 * m_Sensitivity.y * Time.deltaTime, -m_ClampY / 5.0f, m_ClampY);

            Quaternion rot = Quaternion.Euler (m_AbsoluteY, m_AbsoluteX, 0.0f);
            transform.position = m_PlayerTransform.position - ( rot * m_Offset );
            transform.LookAt (m_PlayerTransform.position + rot * new Vector3 (m_Offset.x, m_Offset.y, 0.0f));
        }
    }
}