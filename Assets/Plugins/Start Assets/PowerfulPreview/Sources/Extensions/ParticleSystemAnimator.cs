using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StartAssets.PowerfulPreview
{
    /// <summary>
    /// An utility class that provides an interface to interact with 
    /// a particle system prefab, so you can simulate it inside the preview. 
    /// </summary>
    public class ParticleSystemAnimator
    {
        /// <summary>
        /// Is the provider valid? At least one particle system exists. 
        /// </summary>
        public bool Valid
        {
            get
            {
                return mParticleSystems != null && mParticleSystems.Length > 0;
            }
        }
        /// <summary>
        /// Prefab which contains the particle system. 
        /// </summary>
        public GameObject GameObject
        {
            private set;
            get;
        }

        /// <summary>
        /// Create an instance of the provider for some specific prefab,
        /// which should contain a particle system inside. 
        /// </summary>
        public ParticleSystemAnimator( GameObject gameObject )
        {
            var system = gameObject.GetComponent<ParticleSystem>();
            if( system != null )
            {
                mParticleSystems = new ParticleSystem[] { system };
            }
            else
            {
                mParticleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
            }
            GameObject = gameObject;
            mPrevTime = 0.0f;
        }

        /// <summary>
        /// Simulates the particle systems at some specific time
        /// </summary>
        /// <param name="currentTime">Current time of the simulation.</param>
        public void Simulate( float currentTime )
        {
            if( !Mathf.Approximately( mPrevTime, currentTime ))
            {
                foreach (var system in mParticleSystems)
                {
                    system.Simulate(currentTime, true, true, false);
                }
            }
           
            mPrevTime = currentTime;
        }

        private ParticleSystem[] mParticleSystems;
        private float mPrevTime; 
    }
}
