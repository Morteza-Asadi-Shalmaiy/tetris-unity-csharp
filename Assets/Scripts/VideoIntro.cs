using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(VideoPlayer))]
public class VideoIntro : MonoBehaviour
{
    [SerializeField] string nextSceneName = "MainMenu";
    private VideoPlayer videoPlayer;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        videoPlayer = GetComponent<VideoPlayer>();
        
        ConfigureVideoPlayer();
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();
    }

    void ConfigureVideoPlayer()
    {
        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        videoPlayer.targetCamera = mainCamera;
        videoPlayer.aspectRatio = VideoAspectRatio.FitVertically;
        
        float videoAspect = (float)videoPlayer.width / videoPlayer.height;
        float screenAspect = (float)Screen.width / Screen.height;
        
        if (videoAspect > screenAspect)
        {
            mainCamera.orthographicSize = (float)videoPlayer.width / screenAspect / 200f;
        }
        else
        {
            mainCamera.orthographicSize = (float)videoPlayer.height / 200f;
        }
    }

    void OnVideoEnd(VideoPlayer vp) => SceneManager.LoadScene(nextSceneName);

    void Update()
    {
        if (Input.anyKeyDown) SceneManager.LoadScene(nextSceneName);
    }

    void ConfigureCamera()
    {
        float videoAspect = (float)videoPlayer.width / videoPlayer.height;
        float screenAspect = (float)Screen.width / Screen.height;
        float fov = Mathf.Atan(Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * Mathf.Rad2Deg * 2f);
    
        if (videoAspect > screenAspect)
        {
            mainCamera.fieldOfView = fov * (screenAspect / videoAspect);
        }
    }
}
