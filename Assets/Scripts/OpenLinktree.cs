using UnityEngine;

public class OpenLinktree : MonoBehaviour // Must inherit MonoBehaviour
{
    public string linktreeUrl = "https://linktr.ee/untitled.asd";

    public void OpenLinktreelink()
    {
        Application.OpenURL(linktreeUrl);
    }
}