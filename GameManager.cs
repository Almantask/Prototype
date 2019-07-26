using System.Collections;
using System.Collections.Generic;
using Project.Core.Battle;
using Project.Core.Grid;
using Project.Core.Inputs;
using Project.Core.UI;
using Project.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameSettings Settings { get; private set; }

    public static BattleManager BattleManager { get; private set; }
    public static GridManager GridManager { get; private set; }
    public static InputManager InputManager { get; private set; }
    public static InterfaceManager InterfaceManager { get; private set; }

    private void Start() {
        Launch();
    }

    private void Launch() {
        DontDestroyOnLoad(this);
        Settings = Resources.Load<GameSettings>("Settings/Settings");
        Initialize();
    }

    private void Initialize() {
        LoadManagers();
    }

    private void LoadManagers() {
        InputManager = new GameObject("Input Manager").AddComponent<InputManager>();
        DontDestroyOnLoad(InputManager);
        GridManager = new GameObject("Grid Manager").AddComponent<GridManager>();
        DontDestroyOnLoad(GridManager);
        BattleManager = new GameObject("Battle Manager").AddComponent<BattleManager>();
        DontDestroyOnLoad(BattleManager);
        InterfaceManager = new GameObject("Interface Manager").AddComponent<InterfaceManager>();
        DontDestroyOnLoad(InterfaceManager);
    }

    public void LoadBattle() {
        SceneManager.LoadScene(1);
    }
}
