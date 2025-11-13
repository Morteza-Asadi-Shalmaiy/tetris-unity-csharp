using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoringLog : MonoBehaviour
{
    public TextMeshProUGUI scoringLogText;
    private string log = "";
    public void AddLogMessage(string message)
    {
        log = message + "------------------------------------------\n" + log;
        UpdateLogUI();
    }
    private void UpdateLogUI()
    {
        if (scoringLogText != null)
        {
            scoringLogText.text = log;
        }
    }
    public void ClearLog()
    {
        log = "";
        UpdateLogUI();
    }
}