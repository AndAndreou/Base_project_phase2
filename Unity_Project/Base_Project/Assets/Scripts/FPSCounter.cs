using UnityEngine;
using System.Collections;

public class FPSCounter : MonoBehaviour 
{
    const float UPDATE_INTERVAL = 0.5f;

    int   m_frameCount;
    float m_timeLeft;
    float m_accumulatedFPS;

	void Start()
    {
        if (!GetComponent<GUIText>()) 
        {
            enabled = false;
            return;
        }
        
        m_timeLeft = UPDATE_INTERVAL; 
	}
	
	void Update() 
    {
        m_timeLeft -= Time.deltaTime;
        
        if (m_timeLeft <= 0.0f) 
        {
            GetComponent<GUIText>().text = (m_accumulatedFPS / m_frameCount).ToString("f2") + " FPS";
            m_timeLeft = UPDATE_INTERVAL;
            m_frameCount = 0;
            m_accumulatedFPS = 0.0f;
        } 
        else 
        {
            ++m_frameCount;
            m_accumulatedFPS += Time.timeScale / Time.deltaTime;
        }
	}
}
