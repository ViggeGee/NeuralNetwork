using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimulationManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float timeMultiplier = 1;
    [SerializeField] GameObject panel;
    [SerializeField] Slider timeSlider;
    [SerializeField] Toggle deadAgentToggle;
    [SerializeField] TextMeshProUGUI timeSliderText;
    [SerializeField] TextMeshProUGUI timeSpentText;
    [SerializeField] TextMeshProUGUI agentNrText;
    [SerializeField] Material deadMaterial;

    Color color;
    void Start()
    {
        timeSlider.onValueChanged.AddListener(SetTimeScale);
        timeSlider.value = 1;
        color = deadMaterial.color;
    }
    void Update()
    {
        int timeSpent = (int)Time.time;
        timeSpentText.text = FormatTime(timeSpent);
        agentNrText.text = Agent.reproductionCount.ToString();
    }

    string FormatTime(int seconds)
    {
        int hh = seconds / 3600;
        int mm = (seconds % 3600) / 60;
        int ss = seconds % 60;
        return string.Format("{0:00}:{1:00}:{2:00}", hh, mm, ss);
    }

    void SetTimeScale(float value)
    {
        if (value < 1)
            value = 1;

        Time.timeScale = value;
        value = (int)value;
        timeSliderText.text = "x" + value.ToString();
    }

    public void ToggleDeadAgents()
    {
        if (deadAgentToggle.isOn)
        {
            color.a = 0.95f;
            deadMaterial.color = color;
        }
        else
        {
            color.a = 0;
            deadMaterial.color = color;
        }
    }

    public void TogglePanel()
    {
        if(panel.gameObject.activeSelf)
            panel.gameObject.SetActive(false);
        else
            panel.gameObject.SetActive(true);
    }
}
