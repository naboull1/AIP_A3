using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerPlaneController player;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI messageText;

    private void Start()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (player == null || scoreText == null) return;

        scoreText.text = "Score: " + player.CurrentScore;
    }

    public void ShowMessage(string msg)
    {
        if (messageText == null) return;

        messageText.text = msg;
        messageText.gameObject.SetActive(true);
    }

    public void HideMessage()
    {
        if (messageText == null) return;

        messageText.gameObject.SetActive(false);
    }
}
