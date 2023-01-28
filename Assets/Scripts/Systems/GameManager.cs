using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BuildingManager))]
[RequireComponent(typeof(CameraController))]
[RequireComponent(typeof(ResourceManager))]
[RequireComponent(typeof(WaveManager))]

[System.Serializable]
class UIElements
{
    [Header("Menus")]
    public Transform BuildMenu = null;
    public Transform SelectedMenu = null;
    public Transform PauseMenu = null;
    public Transform DefeatMenu = null;
    public Transform VictoryMenu = null;

    [Header("Text elements")]
    public Text WaveText = null;
    public Text Resources = null;
    public Text Health = null;

    public Text PlaceHqBanner = null;
}

public class GameManager : MonoBehaviour
{
    /*
     * Manages game logic
     */

    //Managers
    private BuildingManager buildingManager = null;
    private CameraController cameraController = null;
    private ResourceManager resourceManager = null;
    private WaveManager waveManager = null;


    //player related declarations
    enum PlayerMode { normal, build, selected, waveInProgres, defeat, paused, placeHQ, victory}
    [SerializeField] PlayerMode playerMode;

    [SerializeField] int team = 0;

    Camera mainCam;
    
    Vector3 mousePosNow;
    Vector3 mousePosLastFrame;
    Vector3 mousePosSinceHeld;

    private Entity hq;
    [SerializeField] private GameObject hqPrefab = null;

    private bool waveInProgress = false;
    private bool paused = false;

    //normal mode related declarations
    private GameObject selectedEntity = null;

    //selected mode related declarations
    [SerializeField] private GameObject selectedIndicatorPrefab = null;
    private GameObject selectedIndicator = null;

    //build mode related declarations
    private GameObject selectedBuildable = null;
    private GameObject buildableGhostPrefab = null;
    private GameObject buildableGhostObject = null;

    //UI related declarations
    [SerializeField] UIElements UiElements = null;

    private void Start()
    {
        LoadResources();
        InitManagers();
        InitPlayer();
        InitGUI();


        mousePosNow = Input.mousePosition;
        mousePosLastFrame = Input.mousePosition;
        mousePosSinceHeld = Input.mousePosition;
    }

    #region Main Thread
    private void Update()
    {
        //check if wave started
        waveInProgress = waveManager.IsWaveInProgress();

        //get pause input
        if (Input.GetKeyDown(KeyCode.Escape) && (playerMode != PlayerMode.victory || playerMode != PlayerMode.defeat))
        {
            Pause();
        }
        //check if we are paused
        if (paused)
        {
            playerMode = PlayerMode.paused;
            Time.timeScale = 0;
        }
        else if(!paused && Time.timeScale == 0)
        {
            playerMode = PlayerMode.normal;
            Time.timeScale = 1;
        }

        //check if we lost
        if (!hq && playerMode != PlayerMode.placeHQ)
        {
            Time.timeScale = 1;
            playerMode = PlayerMode.defeat;
        }
        //check if wave in progress
        if(playerMode != PlayerMode.defeat)
        {
            if (waveInProgress)
            {
                playerMode = PlayerMode.waveInProgres;
            }
            else if(playerMode == PlayerMode.waveInProgres)
            {
                playerMode = PlayerMode.normal;
            }
        }
        //check if we won
        if(playerMode != PlayerMode.waveInProgres && waveManager.wavesComplete())
        {
            playerMode = PlayerMode.victory;
        }
        


        //get mouse position for this frame.
        mousePosNow = Input.mousePosition;


        //get mouse pos once hold
        if (Input.GetMouseButtonDown(1))
        {
            mousePosSinceHeld = Input.mousePosition;
        }

        //normal mode
        //______________________________________________________________________________________________________________________________________
        if (playerMode == PlayerMode.normal)
        {
            
            if (selectedBuildable)
            {
                SwitchMode(PlayerMode.build);
            }
            else if (selectedEntity)
            {
                SwitchMode(PlayerMode.selected);
            }

            //select building enabled if normal mode or selected mode
            RaycastHit h;
            Ray r = mainCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(r, out h))
            {
                Entity hoveringOverEntity = h.transform.GetComponent<Entity>();

                if (hoveringOverEntity && hoveringOverEntity.GetTeam() == team)
                {
                    if (Input.GetKeyUp(KeyCode.Mouse0))
                    {
                        selectedEntity = hoveringOverEntity.gameObject;
                    }
                }
            }

        }
        //build mode
        //______________________________________________________________________________________________________________________________________
        else if (playerMode == PlayerMode.build)
        {
            
            if (selectedBuildable == null)
            {
                if (buildableGhostObject)
                {
                    Destroy(buildableGhostObject);
                }

                SwitchMode(PlayerMode.normal);
            }

            //instantiate buildableghost
            if (!buildableGhostObject)
            {
                buildableGhostObject = Instantiate(buildableGhostPrefab);
                buildableGhostObject.transform.localScale = Vector3.one * selectedBuildable.GetComponent<Entity>().GetPlacementSize();
            }

            //get position of mouse on terrain and check if mouse0 is clicked to try and build a Buildable
            if (buildableGhostObject)
            {
                RaycastHit h;
                Ray r = mainCam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(r, out h))
                {
                    Vector3 positionOfMouse3D = GetNearestPointOnGridXZ(h.point, 1f) + Vector3.up * buildableGhostObject.transform.localScale.y / 2;
                    buildableGhostObject.transform.position = positionOfMouse3D;

                    if (Input.GetKeyUp(KeyCode.Mouse0))
                    {
                        TryBuild(positionOfMouse3D, buildableGhostObject.GetComponent<BuildableGhost>(), resourceManager, selectedBuildable.GetComponent<Entity>());
                    }
                }

            }

            //exit buildmode on right click
            if (Input.GetKeyUp(KeyCode.Mouse1) && !HasMouseMovedSinceHeldDown())
            {
                selectedBuildable = null;
            }
        }

        //selected mode
        //___________________________________________________________________________________________________________________________________________
        else if (playerMode == PlayerMode.selected)
        {
            
            if (selectedEntity == null)
            {
                if (selectedIndicator)
                {
                    Destroy(selectedIndicator);
                }
                SwitchMode(PlayerMode.normal);
            }

            else
            {
                if (!selectedIndicator)
                {
                    selectedIndicator = Instantiate(selectedIndicatorPrefab, selectedEntity.transform.position + Vector3.up * 1f, Quaternion.identity);
                }
            }

            //clear selected on right click
            if (Input.GetKeyUp(KeyCode.Mouse1) && !HasMouseMovedSinceHeldDown())
            {
                selectedEntity = null;
            }

            //select building enabled if normal mode or selected mode
            RaycastHit h;
            Ray r = mainCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(r, out h))
            {
                Entity hoveringOverEntity = h.transform.GetComponent<Entity>();

                if (hoveringOverEntity && hoveringOverEntity.GetTeam() == team)
                {
                    if (Input.GetKeyUp(KeyCode.Mouse0))
                    {
                        if (selectedIndicator)
                        {
                            Destroy(selectedIndicator);
                        }
                        selectedEntity = hoveringOverEntity.gameObject;
                    }
                }
            }
        }

        //Wave mode
        //___________________________________________________________________________________________________________________________________________
        else if (playerMode == PlayerMode.waveInProgres)
        {
            if (buildableGhostObject)
            {
                Destroy(buildableGhostObject);
            }
        }

        //defeat mode
        //___________________________________________________________________________________________________________________________________________
        else if (playerMode == PlayerMode.defeat)
        {

        }

        //Victory mode
        //___________________________________________________________________________________________________________________________________________
        else if (playerMode == PlayerMode.victory)
        {

        }

        //place HQ mode
        //___________________________________________________________________________________________________________________________________________
        else if (playerMode == PlayerMode.placeHQ)
        {
            selectedBuildable = hqPrefab;

            Headquarters hqObj = GameObject.FindObjectOfType<Headquarters>();
            if (hqObj)
            {
                hq = hqObj.GetComponent<Entity>();
                if (buildableGhostObject)
                {
                    Destroy(buildableGhostObject);
                }
                selectedBuildable = null;
                SwitchMode(PlayerMode.normal);
            }

            //instantiate buildableghost
            if (!buildableGhostObject)
            {
                buildableGhostObject = Instantiate(buildableGhostPrefab);
                buildableGhostObject.transform.localScale = Vector3.one * selectedBuildable.GetComponent<Entity>().GetPlacementSize();
            }

            //get position of mouse on terrain and check if mouse0 is clicked to try and build a Buildable
            if (buildableGhostObject)
            {
                RaycastHit h;
                Ray r = mainCam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(r, out h))
                {
                    Vector3 positionOfMouse3D = GetNearestPointOnGridXZ(h.point, 1f) + Vector3.up * buildableGhostObject.transform.localScale.y / 2;
                    buildableGhostObject.transform.position = positionOfMouse3D;

                    if (Input.GetKeyUp(KeyCode.Mouse0))
                    {
                        TryBuild(positionOfMouse3D, buildableGhostObject.GetComponent<BuildableGhost>(), resourceManager, selectedBuildable.GetComponent<Entity>());
                    }
                }

            }
            
        }

        //get mouse position for next frame.
        mousePosLastFrame = Input.mousePosition;
    }
    #endregion Main Thread

    #region GUI
    private void OnGUI()
    {

        //update information panel
        if (playerMode != PlayerMode.paused)
        {
            {
                if (UiElements.WaveText)
                {
                    UiElements.WaveText.text = Mathf.Clamp(waveManager.GetCurrentWaveNumber(), 1, waveManager.GetCurrentWaveNumber()) + "/" + waveManager.GetTotalWavesNumber();
                }
                if (UiElements.Resources)
                {
                    UiElements.Resources.text = Mathf.RoundToInt(resourceManager.GetResources()) + "$";
                }
                if (UiElements.Health && hq)
                {
                    UiElements.Health.text = "Health " +  hq.GetHealth().ToString("F00");
                }
            }
        }
        
        UiElements.PauseMenu.gameObject.SetActive(paused);
        UiElements.DefeatMenu.gameObject.SetActive(playerMode == PlayerMode.defeat);

        UiElements.PlaceHqBanner.gameObject.SetActive(playerMode == PlayerMode.placeHQ);

        //Normal mode
        //___________________________________________________________________________________________________________________________________________
        if (playerMode == PlayerMode.normal)
        {
            UiElements.BuildMenu.gameObject.SetActive(true);
            UiElements.SelectedMenu.gameObject.SetActive(false);
        }

        //Build mode
        //___________________________________________________________________________________________________________________________________________
        else if (playerMode == PlayerMode.build)
        {
            UiElements.BuildMenu.gameObject.SetActive(false);
            UiElements.SelectedMenu.gameObject.SetActive(false);
        }

        //Selected mode
        //___________________________________________________________________________________________________________________________________________
        else if (playerMode == PlayerMode.selected)
        {
            UiElements.BuildMenu.gameObject.SetActive(false);
            UiElements.SelectedMenu.gameObject.SetActive(true);
        }

        //Wave mode
        //___________________________________________________________________________________________________________________________________________
        else if (playerMode == PlayerMode.waveInProgres)
        {
            UiElements.BuildMenu.gameObject.SetActive(false);
            UiElements.SelectedMenu.gameObject.SetActive(false);
        }

        //Defeat mode
        //___________________________________________________________________________________________________________________________________________
        else if (playerMode == PlayerMode.defeat)
        {
            UiElements.BuildMenu.gameObject.SetActive(false);
            UiElements.SelectedMenu.gameObject.SetActive(false);
            UiElements.DefeatMenu.gameObject.SetActive(true);
        }

        //Victory mode
        //___________________________________________________________________________________________________________________________________________
        else if (playerMode == PlayerMode.victory)
        {
            UiElements.BuildMenu.gameObject.SetActive(false);
            UiElements.SelectedMenu.gameObject.SetActive(false);
            UiElements.VictoryMenu.gameObject.SetActive(true);
        }

        //Place HQ mode
        //___________________________________________________________________________________________________________________________________________
        else if (playerMode == PlayerMode.placeHQ)
        {
            UiElements.BuildMenu.gameObject.SetActive(false);
            UiElements.SelectedMenu.gameObject.SetActive(false);
            UiElements.DefeatMenu.gameObject.SetActive(false);
        }

        //disable menus when paused
        if (paused)
        {
            UiElements.BuildMenu.gameObject.SetActive(false);
            UiElements.SelectedMenu.gameObject.SetActive(false);
        }
    }
    #endregion GUI

    #region Methods

    /// <summary>
    /// Loads all required resources before starting a game session.
    /// </summary>
    void LoadResources()
    {
        buildableGhostPrefab = Resources.LoadAll("BuildableGhost", typeof(GameObject)).Cast<GameObject>().ToList()[0];
        if(hqPrefab == null)
        {
            Debug.LogError("NO HQ SET IN INSPECTOR IN GAMEMANAGER! ABORTING INITIALIZING SEQUENCE");
            this.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Get Managers before starting game manager.
    /// </summary>
    void InitManagers()
    {
        buildingManager = GetComponent<BuildingManager>();
        cameraController = GetComponent<CameraController>();
        resourceManager = GetComponent<ResourceManager>();
        waveManager = GetComponent<WaveManager>();
    }

    /// <summary>
    /// Initialize player before starting a game session.
    /// </summary>
    void InitPlayer()
    {
        mainCam = Camera.main;
        playerMode = PlayerMode.placeHQ;
    }

    /// <summary>
    /// Initialize GUI elements
    /// </summary>
    void InitGUI()
    {

        //too much hard coding needs to be made modular later

        if (!UiElements.BuildMenu)
        {
            Debug.LogWarning("No build Menu set. Set one in the inspector!");
            return;
        }
        //loads all buildables into pressable buttons
        else
        {
            int i_buildables = 0;
            foreach(Transform t in UiElements.BuildMenu)
            {
                //m_YourThirdButton.onClick.AddListener(() => ButtonClicked(42));
                if(i_buildables < buildingManager.buildables.Count)
                {
                    Button b = t.GetComponent<Button>();
                    Text text = t.GetComponentInChildren<Text>();
                    t.name = i_buildables.ToString();
                    b.onClick.AddListener(() => SelectBuildable(int.Parse(t.name)));
                    text.text = buildingManager.buildables[i_buildables].GetComponent<Entity>().GetAbreviation();

                    i_buildables++;
                }
                else
                {
                    t.gameObject.SetActive(false);
                }

            }
        }
    }

    /// <summary>
    /// Switch playermode
    /// </summary>
    void SwitchMode(PlayerMode p)
    {
        playerMode = p;
    }

    /// <summary>
    /// select buildable and automatically switch to build mode
    /// </summary>
    void SelectBuildable(int buildableIndex)
    {
        selectedBuildable = buildingManager.buildables[buildableIndex];
    }

    /// <summary>
    /// demolish selected entity
    /// </summary>
    public void DemolishSelected()
    {
        if (selectedEntity)
        {
            selectedEntity.GetComponent<Entity>().Demolish(resourceManager);
        }
    }

    /// <summary>
    /// upgrade selected entity
    /// </summary>
    public void UpgradeSelected()
    {
        if (selectedEntity)
        {
            selectedEntity.GetComponent<Entity>().Upgrade();
        }
    }

    /// <summary>
    /// Tries to place a building in the position marked and will check if resources are sufficient and valid position.
    /// </summary>
    bool TryBuild(Vector3 position, BuildableGhost bGhost, ResourceManager rManager, Entity toBuild)
    {
        if(rManager.GetResources() >= toBuild.GetCost() && bGhost.CanBuild())
        {
            Entity built = Instantiate(toBuild, position - Vector3.up * position.y, Quaternion.identity);
            rManager.UseResources(toBuild.GetCost());
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets grid position of X and Z from a vector and returns the resulting vector.
    /// </summary>
    public Vector3 GetNearestPointOnGridXZ(Vector3 position, float gridSize)
    {

        int xCount = Mathf.RoundToInt(position.x / gridSize);
        float yCount = 0;
        int zCount = Mathf.RoundToInt(position.z / gridSize);

        Vector3 result = new Vector3(
            (float)xCount * gridSize,
            (float)yCount * gridSize,
            (float)zCount * gridSize);

        return result;
    }

    /// <summary>
    /// Has the mouse position changed since last frame.
    /// </summary>
    public bool HasMouseMovedSinceLastFrame()
    {
        if(mousePosNow != mousePosLastFrame)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Has the mouse position changed since we held down.
    /// </summary>
    public bool HasMouseMovedSinceHeldDown()
    {
        if (mousePosNow != mousePosSinceHeld)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Method to change pause state
    /// </summary>
    public void Pause()
    {
        paused = !paused;
    }

    /// <summary>
    /// Reload scene
    /// </summary>
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Return to main menu
    /// </summary>
    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    #endregion Methods

}
