using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MyUIController : MonoBehaviour
{
    private VisualElement root;
    private Button playButton;
    private Button quitButton;

    void OnEnable()
    {
        // Получите корневой элемент UI Document
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        // Найдите кнопку по имени и добавьте обработчик событий
        playButton = root.Q<Button>("PlayButton");
        playButton.clicked += OnPlayButtonClick;

        quitButton = root.Q<Button>("QuitButton");
        quitButton.clicked += OnQuitButtonClick;
    }

    void OnPlayButtonClick()
    {
        SwitchScene("SampleScene");
        Debug.Log("Button clicked!");
    }

    void OnQuitButtonClick()
    {
        QuitGame();
    }

    void SwitchScene(string sceneName)
    {
        // Проверяет, загружена ли уже сцена
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            // Загружает сцену с указанным именем
            SceneManager.LoadScene(sceneName);
        }
    }
    // Метод для выхода из игры
    public void QuitGame()
    {
        // Для отладки в редакторе Unity
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // Закрытие приложения
        Application.Quit();
        #endif
    }
}
