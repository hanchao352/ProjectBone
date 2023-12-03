public interface  IMod
{
    void Initialize();
    void Update(float time);
    void OnApplicationFocus(bool hasFocus);
    void OnApplicationPause(bool pauseStatus);
    void OnApplicationQuit();
    void Dispose();
}