using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : UIFormBase
{
    //角色面板

    //选择过的Buff

    //设置按钮（呼出设置界面）

    [SerializeField][Header("返回游戏按钮")] private Button resumeButton;
    [SerializeField][Header("结束游戏按钮")] private Button overGameButton;

    protected override void Init()
    {

        resumeButton.onClick.AddListener(OnResume);
        overGameButton.onClick.AddListener(OverGame);
    }

    private void OnResume()
    {
        EventBus.TriggerGameResumed();
    }

    private void OverGame()
    {

    }

}
