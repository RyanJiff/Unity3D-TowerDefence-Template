using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class WaveManager : MonoBehaviour
{
    /*
     * controls the flow of enemy spawns
     * spawn events can be triggered externally
     */

    [SerializeField] private GameObject hq = null;


    [SerializeField] Transform spawnPosition = null;
    [SerializeField] private Transform enemiesRootTransform;

    private GameObject pathCheckerPrefab = null;
    private GameObject pathCheckerObject = null;
    NavMeshPath path = null;

    [SerializeField] WaveSet waveSet = null;
    [SerializeField] int waveCount = 0;
    [SerializeField] int currentWaveNumber = 0;
    [SerializeField] Wave currentWave = null;
    [SerializeField] public List<Wave.WaveClump> activeClumps = new List<Wave.WaveClump>();
    [SerializeField] bool[] activeClumpsFlag;

    [SerializeField] float waveTimer = 0.0f;

    [SerializeField] bool waveJustStarted = false;

    private void Start()
    {
        InitWaveManager();
    }

    private void Update()
    {
        if (!hq)
        {
            Headquarters hqObj = GameObject.FindObjectOfType<Headquarters>();
            if (hqObj)
            {
                hq = hqObj.gameObject;
            }
        }


        if (IsWaveInProgress()  && currentWave)
        {
            int i = 0;
            waveTimer += Time.deltaTime;
            foreach (Wave.WaveClump wClump in currentWave.waveClumps)
            {
                if (wClump.activateTime <= waveTimer && activeClumpsFlag[i] == false)
                {
                    Debug.Log("clump incoming");
                    activeClumps.Add(new Wave.WaveClump(wClump.count,wClump.enemy,wClump.timeInterval,wClump.activateTime));
                    activeClumpsFlag[i] = true;
                }
                i++;
            }
            foreach (Wave.WaveClump wClump in activeClumps)
            {
                if (wClump.Tick(Time.deltaTime))
                {
                    Instantiate(wClump.GetEnemy(), spawnPosition.position, Quaternion.identity);
                    waveJustStarted = false;
                }
            }
        }
    }


    /// <summary>
    /// Initializes the wave manager and loads required resources.
    /// </summary>
    void InitWaveManager()
    {
        path = new NavMeshPath();
        pathCheckerPrefab = Resources.LoadAll("PathChecker", typeof(GameObject)).Cast<GameObject>().ToList()[0];
        enemiesRootTransform = GameObject.FindGameObjectWithTag("Enemies").transform;

        pathCheckerObject = Instantiate(pathCheckerPrefab, spawnPosition.position, Quaternion.identity);
    }

    /// <summary>
    /// Returns true if there is no path from each entrance to HQ
    /// </summary>
    bool IsPathBlocked()
    {
        pathCheckerObject.GetComponent<NavMeshAgent>().CalculatePath(hq.transform.position, path);
        print("New path calculated");
        if (path.status != NavMeshPathStatus.PathComplete)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// are there enemies still alive?
    /// </summary>
    public bool IsWaveInProgress()
    {
        if (enemiesRootTransform.childCount > 0 || waveJustStarted)
        {
            return true;
        }
        foreach (Wave.WaveClump wClump in activeClumps)
        {
            if (!wClump.AreWeFinished())
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Start the wave From the WaveSet in waveset
    /// </summary>
    public void StartWave()
    {
        if(waveJustStarted || IsWaveInProgress() || !hq)
        {
            return;
        }


        if (waveSet)
        {
            waveCount = waveSet.GetTotalWaveCount();
            if (IsPathBlocked())
            {
                Debug.LogWarning("Path Blocked! Cant Start Wave!");
                return;
            }

            waveTimer = 0.0f;
            activeClumps.Clear();
            Debug.Log("clumps cleared");
            if (currentWaveNumber < waveCount)
            {
                Debug.Log("Wave Started!");
                currentWaveNumber++;
                currentWave = waveSet.waves[currentWaveNumber - 1];
                activeClumpsFlag = new bool[currentWave.waveClumps.Count];
                waveJustStarted = true;
            }
        }
    }

    /// <summary>
    /// Get total wave count
    /// </summary>
    public int GetTotalWavesNumber()
    {
        return waveSet.waves.Count;
    }

    /// <summary>
    /// Get current wave
    /// </summary>
    public int GetCurrentWaveNumber()
    {
        return currentWaveNumber;
    } 

}