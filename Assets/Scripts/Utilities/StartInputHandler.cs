using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartInputHandler : MonoBehaviour
{

    [SerializeField] private string firstScene = "1_game";

    [SerializeField] private GameObject carLight;

    [SerializeField] private GameObject rightSmog;
    [SerializeField] private GameObject leftSmog;

    [SerializeField] private GameObject overlay;


    public void OnStartGame()
    {

        SoundManager.Instance.IsPlayingSnapshot();
        StartCoroutine(LoadPlayScene());

    }

    private IEnumerator LoadPlayScene()
    {
        GameObject.Find("Overlay").SetActive(false);
        yield return new WaitForSeconds(0.5f);
        SoundManager.Instance.CarStartSFX();
        carLight.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        rightSmog.SetActive(true);
        leftSmog.SetActive(true);

        yield return new WaitForSeconds(1); 
        
        GameManager.Instance.SetState(GameManager.GameState.Loading);
        yield return new WaitForSeconds(2); 
        AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(firstScene);
        while (!asyncLoadScene.isDone)
        {
            yield return null;
        }

        GameManager.Instance.SetState(GameManager.GameState.Playing);
    }

    public void SetupStartScene()
    {
        overlay.SetActive(true);
        carLight.SetActive(false);
        rightSmog.SetActive(false);
        leftSmog.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
