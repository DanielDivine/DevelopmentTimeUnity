using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class DebugInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_FPSMeterText;
    [SerializeField] private TextMeshProUGUI m_GraphicsAPIText;

    [SerializeField] private bool m_UseFpsMeter = true;

    private float m_deltaTime = 0.0f;

    private void Start()
    {
        GraphicsDeviceType currentAPI = SystemInfo.graphicsDeviceType;

        m_GraphicsAPIText.text = "GraphicsAPI: " + currentAPI.ToString();
    }
    private void Update()
    {
        m_deltaTime += (Time.unscaledDeltaTime - m_deltaTime) * 0.1f;

        float fps = 1.0f / m_deltaTime;
        if(m_FPSMeterText != null )
        {
            m_FPSMeterText.text = "FPS: " + Mathf.Ceil(fps).ToString();
        }
    }
}
