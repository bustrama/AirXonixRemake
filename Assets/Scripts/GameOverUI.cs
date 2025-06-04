using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameOverUI : MonoBehaviour {
    private GameObject canvasObj;

    void Awake() {
        SetupUI();
        canvasObj.SetActive(false);
    }

    void SetupUI() {
        // Canvas
        canvasObj = new GameObject("GameOverCanvas");
        canvasObj.transform.SetParent(transform, false);
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Ensure there is an EventSystem so the button receives clicks
        if (FindObjectOfType<EventSystem>() == null) {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        // Panel background
        var panelObj = new GameObject("Panel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        var panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.6f);
        var panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // GameOver Text
        var textObj = new GameObject("GameOverText");
        textObj.transform.SetParent(panelObj.transform, false);
        var text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.text = "Game Over";
        text.fontSize = 40;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        var textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.7f);
        textRect.anchorMax = new Vector2(0.5f, 0.7f);
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(400, 100);

        // Restart button
        var buttonObj = new GameObject("RestartButton");
        buttonObj.transform.SetParent(panelObj.transform, false);
        var buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = Color.white;
        var button = buttonObj.AddComponent<Button>();
        var buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.3f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.3f);
        buttonRect.anchoredPosition = Vector2.zero;
        buttonRect.sizeDelta = new Vector2(160, 40);

        var buttonTextObj = new GameObject("Text");
        buttonTextObj.transform.SetParent(buttonObj.transform, false);
        var buttonText = buttonTextObj.AddComponent<Text>();
        buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        buttonText.text = "Restart";
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.black;
        var buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;

        button.onClick.AddListener(RestartGame);
    }

    public void Show() {
        canvasObj.SetActive(true);
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
