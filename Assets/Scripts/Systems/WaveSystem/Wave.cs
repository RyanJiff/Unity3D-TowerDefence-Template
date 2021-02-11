using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "wave data", menuName = "Waves/Wave", order = 1)]
public class Wave : ScriptableObject
{
    /*
    *wave data object
    */

    public List<WaveClump> waveClumps = new List<WaveClump>();


    [System.Serializable]
    public class WaveClump
    {

        public int count = 1;
        public GameObject enemy = null;
        public float timeInterval = 1f;
        [Tooltip("The time into the wave before we activate this wave clump.")]
        public float activateTime = 2f;
        float timer = 0;

        public WaveClump(int c, GameObject e, float timeInt, float activeTime)
        {
            this.count = c;
            this.enemy = e;
            this.timeInterval = timeInt;
            this.activateTime = activeTime;
            this.timer = 0;
        }


        /// <summary>
        /// Returns whether its time to spawn a unit or not
        /// </summary>
        public bool Tick(float deltaTime)
        {
            timer -= deltaTime;
            if(timer <= 0 && count > 0)
            {
                ResetTimer();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if there are no more units to spawn in this wave clump
        /// </summary>
        public bool AreWeFinished()
        {
            if(this.count <= 0)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Resets the timer and reduces spawned units count
        /// Returns whether we have more units to spawn or not
        /// </summary>
        bool ResetTimer()
        {
            count--;
            timer = timeInterval;
            if(count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns GameObject to be spawned in the wave clump
        /// </summary>
        public GameObject GetEnemy()
        {
            return enemy;
        }
    }
}
