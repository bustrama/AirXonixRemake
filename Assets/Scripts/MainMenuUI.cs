using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuUI : MonoBehaviour {
    void Start() {
        SetupUI();
    }

    void SetupUI() {
        var canvasObj = new GameObject("MenuCanvas");
        canvasObj.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        if (FindObjectOfType<EventSystem>() == null) {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        var panelObj = new GameObject("Panel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        var panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0,0,0,0.6f);
        var rect = panelObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        CreateButton(panelObj.transform, "Start", new Vector2(0.5f,0.6f), () => SceneManager.LoadScene("LevelSelect"));
        CreateButton(panelObj.transform, "Exit", new Vector2(0.5f,0.4f), () => Application.Quit());
    }

    void CreateButton(Transform parent, string text, Vector2 anchor, UnityEngine.Events.UnityAction callback) {
        var buttonObj = new GameObject(text+"Button");
        buttonObj.transform.SetParent(parent, false);
        var image = buttonObj.AddComponent<Image>();
        image.color = Color.white;
        var button = buttonObj.AddComponent<Button>();
        var rect = buttonObj.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(160,40);

        var textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        var btnText = textObj.AddComponent<Text>();
        btnText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        btnText.text = text;
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.color = Color.black;
        var textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        button.onClick.AddListener(callback);
    }
}
