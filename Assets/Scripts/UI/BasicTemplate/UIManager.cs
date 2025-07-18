using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// UI 管理器（单例）
/// 负责动态加载、注册、显示、隐藏、回收 UI 面板
/// </summary>
public class UIManager : Singleton<UIManager>
{
    #region 字段

    /// <summary> 路径映射模板（Key = 面板名，Value = Resources 路径） </summary>
    public Dictionary<string, GameObject> formPrefabs = new();

    /// <summary> 当前已注册的实例面板 </summary>
    public Dictionary<string, UIFormBase> forms = new();

    /// <summary> 当前正在显示的面板 </summary>
    public List<UIFormBase> showForms = new();

    /// <summary> 面板显示堆栈（用于顺序关闭） </summary>
    public Stack<UIFormBase> showFormStack = new();

    /// <summary> UI 根节点（场景中应存在名为 UIRoot 的对象） </summary>
    public Transform uiRoot => GameObject.Find("UIRoot").transform;

    // 动态面板分组字典
    public Dictionary<string, List<UIFormBase>> dynamicFormGroups = new Dictionary<string, List<UIFormBase>>();

    #endregion

    #region 注册接口

    /// <summary>
    /// 普通面板注册接口
    /// </summary>
    /// <param name="uIForm"></param>
    public void RegisterForm(IUIForm uIForm)
    {
        var form = uIForm.GetUIFormBase();
        if (form.IsDynamicForm)
        {
            Debug.LogError($"Cannot register dynamic form {form.name} using RegisterForm, use RegisterDynamicForm instead");
            return;
        }

        string key = form.GetType().Name;
        RegisterFormInternal(key, form);
    }

    /// <summary>
    /// 动态面板注册接口
    /// </summary>
    /// <param name="uIForm"></param>
    /// <param name="groupID"></param>
    public void RegisterDynamicForm(IUIForm uIForm, string groupID)
    {
        var form = uIForm.GetUIFormBase();
        if (!form.IsDynamicForm)
        {
            Debug.LogError($"Cannot register non-dynamic form {form.name} using RegisterDynamicForm");
            return;
        }

        string key = $"{groupID}_{Guid.NewGuid()}";
        form.InitializeAsDynamic(groupID);
        RegisterFormInternal(key, form);
    }

    /// <summary>
    /// 内部注册方法
    /// </summary>
    /// <param name="key"></param>
    /// <param name="form"></param>
    private void RegisterFormInternal(string key, UIFormBase form)
    {
        if (!forms.ContainsKey(key))
        {
            forms.Add(key, form);

            // 动态面板分组处理
            if (form.IsDynamicForm && !string.IsNullOrEmpty(form.DynamicGroupID))
            {
                if (!dynamicFormGroups.ContainsKey(form.DynamicGroupID))
                {
                    dynamicFormGroups.Add(form.DynamicGroupID, new List<UIFormBase>());
                }
                dynamicFormGroups[form.DynamicGroupID].Add(form);
            }

            form.Close(); // 默认关闭
        }
    }

    public void UnRegisterForm(IUIForm uIForm)
    {
        var form = uIForm.GetUIFormBase();
        string key = form.GetType().Name;

        // 如果是动态面板，需要从分组中移除
        if (form.IsDynamicForm && !string.IsNullOrEmpty(form.DynamicGroupID))
        {
            if (dynamicFormGroups.TryGetValue(form.DynamicGroupID, out var group))
            {
                group.Remove(form);
            }
        }

        if (forms.ContainsKey(key))
        {
            showForms.Remove(form);

            var tmpStack = new Stack<UIFormBase>();
            while (showFormStack.Count > 0)
            {
                var top = showFormStack.Pop();
                if (top != form) tmpStack.Push(top);
            }
            showFormStack = new Stack<UIFormBase>(tmpStack);

            forms.Remove(key);
        }
    }

    /// <summary>
    /// 手动注册已有面板实例（用于场景中已有面板）
    /// </summary>
    public void RegisterFormInstance(UIFormBase formInstance)
    {
        if (formInstance.IsDynamicForm)
        {
            Debug.LogError($"Cannot register dynamic form instance {formInstance.name} using RegisterFormInstance");
            return;
        }

        string key = formInstance.GetType().Name;
        if (!forms.ContainsKey(key))
        {
            forms.Add(key, formInstance);
            formInstance.Close(); // 默认关闭
        }
    }

    #endregion

    #region 显示与隐藏

    public void ShowUIForm(string name)
    {
        if (!forms.ContainsKey(name))
        {
            CreateForm(name);
            if (!forms.ContainsKey(name)) return;
        }

        var form = forms[name];
        if (form != null && !showForms.Contains(form))
        {
            form.Open(this);
            showForms.Add(form);
            showFormStack.Push(form);
        }
    }

    public void ShowUIForm<T>() where T : UIFormBase => ShowUIForm(typeof(T).Name);

    public void HideUIForm(string name)
    {
        var form = GetForm(name);
        if (form != null && showForms.Contains(form))
        {
            showForms.Remove(form);
            form.Close();
        }
    }

    public void HideUIForm<T>() where T : UIFormBase => HideUIForm(typeof(T).Name);

    public void HideUIFormTurn()
    {
        if (showFormStack.Count > 0)
        {
            var form = showFormStack.Pop();
            HideUIForm(form.name);
        }
    }

    public void HideAllUIForm()
    {
        foreach (var form in showForms)
            form.Close();

        showForms.Clear();
        showFormStack.Clear();
    }

    public bool HasActiveForm() => showForms.Count > 0;

    private void CreateForm(string name)
    {
        if (formPrefabs.ContainsKey(name))
        {
            var formObj = GameObject.Instantiate(formPrefabs[name], uiRoot);
            formObj.name = name;
        }
        else
        {
            Debug.LogError($"[UIManager] CreateForm Failed: no prefab named {name}");
        }
    }

    #endregion

    #region AB包预加载

    /// <summary>
    /// AB包预加载
    /// </summary>
    /// <param name="label"></param>
    /// <returns></returns>
    public async Task PreloadFormsByLabelAsync(string label)
    {
        try
        {
            var loadHandle = Addressables.LoadAssetsAsync<GameObject>(label, null);
            await loadHandle.Task;

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var prefab in loadHandle.Result)
                {
                    if (!formPrefabs.ContainsKey(prefab.name))
                    {
                        formPrefabs.Add(prefab.name, prefab);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"AB包标签加载失败: {label}, 错误: {e.Message}");
        }
    }

    /// <summary>
    /// 通用预加载方法
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public async Task PreloadAllFormsAsync(UIResourceConfigSO config)
    {
        if (config == null) return;

        // 1. 加载标签化的UI
        foreach (var tag in config.uiTags)
        {
            if (tag.preloadOnStart)
            {
                await PreloadFormsByLabelAsync(tag.labelName);
            }
        }

        // 2. 手动注册的UI
        PreLoadForms(config.manualUIForms);

        // 3. 额外预加载的UI（角色卡片、Buff卡片等）
        PreLoadForms(config.additionalPreloadForms); // 统一加载，不区分具体类型
    }

    public void PreLoadForm(GameObject prefab)
    {
        if (prefab == null) return;
        if (!formPrefabs.ContainsKey(prefab.name))
        {
            formPrefabs.Add(prefab.name, prefab);
        }
    }

    public void PreLoadForms(GameObject[] prefabs)
    {
        foreach (var prefab in prefabs)
        {
            PreLoadForm(prefab);
        }
    }

    #endregion

    #region 快捷访问

    public UIFormBase GetForm(string name) => forms.TryGetValue(name, out var f) ? f : null;

    public T GetForm<T>() where T : UIFormBase => GetForm(typeof(T).Name) as T;

    public bool IsShown(string name) => GetForm(name)?.IsOpen ?? false;

    public UIFormBase TryShowForm(string name)
    {
        ShowUIForm(name);
        return GetForm(name);
    }

    #endregion

    #region 动态生成面板扩展


    /// <summary>
    /// 动态面板创建方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="prefab"></param>
    /// <param name="groupID"></param>
    /// <param name="parent"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public T CreateDynamicForm<T>(GameObject prefab, string groupID, Transform parent = null,
        UIFormConfigSO config = null) where T : UIFormBase
    {
        if (prefab == null) return null;

        var formObj = UnityEngine.Object.Instantiate(prefab, parent ?? uiRoot);
        var form = formObj.GetComponent<T>();

        if (form == null)
        {
            UnityEngine.Object.Destroy(formObj);
            Debug.LogError($"Prefab does not contain {typeof(T)} component!");
            return null;
        }

        form.InitializeAsDynamic(groupID, config);
        RegisterDynamicForm(form, groupID);
        return form;
    }

    /// <summary>
    /// 获取动态面板组的方法
    /// </summary>
    /// <param name="groupID"></param>
    /// <returns></returns>
    public List<UIFormBase> GetDynamicFormsInGroup(string groupID)
    {
        if (dynamicFormGroups.TryGetValue(groupID, out var forms))
        {
            return forms;
        }
        return new List<UIFormBase>();
    }

    /// <summary>
    /// 清除动态面板组的方法
    /// </summary>
    /// <param name="groupID"></param>
    /// <param name="immediate"></param>
    public void ClearDynamicFormsInGroup(string groupID, bool immediate = false)
    {
        if (!dynamicFormGroups.ContainsKey(groupID)) return;

        foreach (var form in dynamicFormGroups[groupID].ToArray())
        {
            if (form == null) continue;

            if (immediate)
            {
                UnRegisterForm(form);
                UnityEngine.Object.Destroy(form.gameObject);
            }
            else
            {
                form.Close();
            }
        }

        dynamicFormGroups[groupID].Clear();
    }

    #endregion
}


public interface IUIForm
{
    void RegisterForm() => UIManager.Instance.RegisterForm(this);
    void UnRegisterForm() => UIManager.Instance.UnRegisterForm(this);
    UIFormBase GetUIFormBase();
}

public enum FormAnimType
{
    None,
    Fade,
    Zoom,
    Pop,
    SlideLeft,
    SlideRight,
    SlideUp,
    SlideDown,
    FadeSlide,

}


