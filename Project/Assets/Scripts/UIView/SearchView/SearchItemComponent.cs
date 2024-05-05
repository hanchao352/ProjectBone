using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SearchItemComponent : ComponentBase
{
    public Bone bone;
    public TextMeshProUGUI TitleText;
    public Button GotoBtn;
    public override void Initialize()
    {
        base.Initialize();
        TitleText = Root.transform.Find("BackgroundImage/Text (TMP)").GetComponent<TextMeshProUGUI>();
        GotoBtn = Root.transform.Find("jump_btn").GetComponent<Button>();
        GotoBtn.onClick.AddListener(OnGotoBtnClick);
    }

    private void OnGotoBtnClick()
    {
        if (bone == null)
        {
            return;
        }
        UIManager.Instance.HideView(ViewID.SearchView);
        GameObjectManager.Instance.BodyVisible = true;
        BoneMod.Instance.CurrentBoneId = bone.Id;
        SkeletonInfo selectedBone = GameObjectManager.Instance.GetSkeletonInfo(bone.Id);
        if (selectedBone != null)
        {
            GameObject body = GameObjectManager.Instance.Body;
            GameObjectManager.Instance.Body.transform.localScale = Vector3.one*3;
            GameObject target = selectedBone.boneGameObject;
           Camera mainCamera = UIManager.Instance.ModelCamera;
           //相机移动到目标位置位置前方,看向目标位置
           float distance = 0.5f;
           // 计算目标前方的位置
           Vector3 targetFrontPosition = target.transform.position + body.transform.forward * distance;

            // 设置相机的位置
           mainCamera.transform.position = targetFrontPosition;

        // 使相机朝向目标
           mainCamera.transform.LookAt(target.transform.position);
           EventManager.Instance.TriggerEvent(EventDefine.BoneClickEvent ,selectedBone.boneId);
        }
    }

    public override void OnShow(params object[] args)
    {
        base.OnShow(args);
        UpdateUI();
    }

    public void SetData(Bone bone)
    {
        this.bone = bone;
    }

    public void UpdateUI()
    {
        if (bone == null)
        {
            return;
        }
        TitleText.text = bone.Name;
    }

    public override void UpdateView(params object[] args)
    {
        base.UpdateView(args);
        UpdateUI();
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