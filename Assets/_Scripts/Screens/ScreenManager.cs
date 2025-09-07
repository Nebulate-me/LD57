using UnityEngine.SceneManagement;

namespace _Scripts.Screens
{
    public class ScreenManager : IScreenManager
    {
        private const string TUTORIAL_SCENE_NAME = "TutorialScene";
        private const string GAME_SCENE_NAME = "GameScene";
        private const string HIGH_SCORE_SCENE_NAME = "HighScoreScene";
        private const string MAIN_MENU_SCENE_NAME = "MainMenuScene";

        public void GoToTutorialScreen()
        {
            GoToScreen(TUTORIAL_SCENE_NAME);
        }
        
        public void GoToGameScreen()
        {
            GoToScreen(GAME_SCENE_NAME);
        }
        
        public void GoToHighScoreScreen()
        {
            GoToScreen(HIGH_SCORE_SCENE_NAME);
        }

        public void GoToMainMenuScreen()
        {
            GoToScreen(MAIN_MENU_SCENE_NAME);
        }
        
        private void GoToScreen(string screenName)
        {
            SceneManager.LoadScene(screenName);
        }
    }
}