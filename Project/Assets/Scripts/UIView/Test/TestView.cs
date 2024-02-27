using UnityEngine.Rendering;
using UnityEngine.UI;

public class TestView : UIBase
{
    public Button LoginButton;
    public Button NoteButton;
    public Button ShouCangButton;
    public override void Initialize()
    {
        base.Initialize();
        LoginButton = Root.transform.Find("top/Login").GetComponent<Button>();
        NoteButton = Root.transform.Find("top/Note").GetComponent<Button>();
        ShouCangButton = Root.transform.Find("top/ShouCang").GetComponent<Button>();
        LoginButton.onClick.AddListener(OnLoginButtonClick);
        NoteButton.onClick.AddListener(OnNoteButtonClick);
        ShouCangButton.onClick.AddListener(OnShouCangButtonClick);
    }

    private void OnShouCangButtonClick()
    {
        UIManager.Instance.ShowView(ViewID.WebView,WebState.Collect);
    }

    private void OnNoteButtonClick()
    {
        UIManager.Instance.ShowView(ViewID.WebView,WebState.Note);
    }

    private void OnLoginButtonClick()
    {
        UIManager.Instance.ShowView(ViewID.WebView,WebState.Login);
    }

    public override void OnShow(params object[] args)
    {
        base.OnShow(args);
    }

    public override void UpdateView(params object[] args)
    {
        base.UpdateView(args);
    }

    public override void Update(float time)
    {
        base.Update(time);
    }

    public override void OnApplicationFocus(bool hasFocus)
    {
        base.OnApplicationFocus(hasFocus);
    }

    public override void OnApplicationPause(bool pauseStatus)
    {
        base.OnApplicationPause(pauseStatus);
    }

    public override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnEnterMutex()
    {
        base.OnEnterMutex();
    }

    public override void OnExitMutex()
    {
        base.OnExitMutex();
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}