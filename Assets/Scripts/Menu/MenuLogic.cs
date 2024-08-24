using PlasticGui.Configuration.CloudEdition.Welcome;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using static MenuLogic;

public enum TabType { MAIN,SETTINGS,LOGIN,REGISTER};

/// <summary>
/// manages a tab stack. opens a specified tab and
/// closes the last tab opened.
/// </summary>
public class MenuLogic : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;

    [SerializeField] private GameObject settingsTab;
    [SerializeField] private GameObject buttonsTab;
    [SerializeField] private GameObject loginTab;
    [SerializeField] private GameObject registerTab;

    /// <summary>
    /// when a tab type is specified, menulogic
    /// needs to know which gameobject to open (i.e. to enable)
    /// </summary>
    Dictionary<TabType, GameObject> tabs;

    private Stack<GameObject> tabStack;

    private void Start()
    {
        tabs = new() {
            {TabType.MAIN,buttonsTab},
            {TabType.SETTINGS,settingsTab},
            {TabType.LOGIN,loginTab},
            {TabType.REGISTER,registerTab}
        };

        CloseAll();
        tabStack = new();
        // default tab is the buttons one
        OpenTab(TabType.MAIN);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && tabStack.Count > 1)
        {
            CloseTab();
        }
    }

    public void CloseAll()
    {
        foreach(var tab in tabs.Values)
        {
            if(tab!=null)
                tab.SetActive(false);
        }
        tabStack = new();
    }

    public void StartPlaying()
    {
        sceneLoader.Load(SceneLoader.playingScene);
    }

    public void ExitToDesktop()
    {
        Application.Quit();
    }

    public void OpenTab(TabType tabType)
    {
        // tab to be opened
        GameObject newTab = tabs[tabType];
        if (tabStack.Count > 0)
        {
            // if a tab is currently visible - set it as inactive
            GameObject oldTab = tabStack.Peek();
            oldTab.SetActive(false);
        }

        // open the new tab
        tabStack.Push(newTab);
        newTab.SetActive(true);
    }

    public void CloseTab()
    {
        // remove top of stack and set the tab as inactive
        GameObject tab = tabStack.Pop();
        tab.SetActive(false);

        if(tabStack.Count > 0)
        {
            // get the current top of the stack
            // and set that as active.
            GameObject newTab = tabStack.Peek();
            newTab.SetActive(true);
        }
    }
}
