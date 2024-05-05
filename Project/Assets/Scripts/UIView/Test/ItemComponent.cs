using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemComponent : ComponentBase
{
    public Image BGImage;
    public Text text;
    public override void Initialize()
    {
        base.Initialize();
        BGImage = Root.transform.Find("BackgroundImage").GetComponent<Image>();
        text = Root.transform.Find("TitleText").GetComponent<Text>();
        text.raycastTarget = false;
        BGImage.AddComponent<Button>().onClick.AddListener(OnClick);
        Debug.Log(this.GetType()+"Initialize");
    }

    public void OnClick()
    {
        Debug.Log(Root.name);
    }

    public override void OnShow(params object[] args)
    {
        base.OnShow(args);
        Debug.Log(this.GetType()+"OnShow");
    }

    public override void UpdateView(params object[] args)
    {
        base.UpdateView(args);
        Debug.Log(this.GetType()+"UpdateView");
    }

    public override void Update(float time)
    {
        base.Update(time);
        Debug.Log(this.GetType()+"Update");
    }
    public void UpdateUI()
    {
        Debug.Log(this.GetType()+"UpdateUI");
        text.text = "Hello World";


    }
    public override void OnApplicationFocus(bool hasFocus)
    {
        base.OnApplicationFocus(hasFocus);
        Debug.Log(this.GetType()+"OnApplicationFocus");
    }

    public override void OnApplicationPause(bool pauseStatus)
    {
        base.OnApplicationPause(pauseStatus);
        Debug.Log(this.GetType()+"OnApplicationPause");
    }

    public override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        Debug.Log(this.GetType()+"OnApplicationQuit");
    }

    public override void OnHide()
    {
        base.OnHide();
        Debug.Log(this.GetType()+"OnHide");
    }

    public override void OnEnterMutex()
    {
        base.OnEnterMutex();
        Debug.Log(this.GetType()+"OnEnterMutex");
    }

    public override void OnExitMutex()
    {
        base.OnExitMutex();
        Debug.Log(this.GetType()+"OnExitMutex");
    }

    public override void Dispose()
    {
        base.Dispose();
        Debug.Log(this.GetType()+"Dispose");
    }
}