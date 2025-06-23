using System.Collections.Generic;
using GamePlatform.PlatformActions.Base;
using UnityEngine;

namespace GamePlatform.PlatformActions.Actions {
    public class PlatformSpawnObjectAbove : ActionBase {
        [SerializeField] private Transform parent; // Will replace spawn point
        [SerializeField] private float spawnDistance;
        [SerializeField] private float spawnChance;
        [SerializeField] private List<GameObject> objectPool;

        public PlatformSpawnObjectAbove() : base(actOnStart: true) {}
        
        protected override void InternalAct() {
            if (Random.value > spawnChance) return;

            var randomIndex = Random.Range(0, objectPool.Count);
            var thisObject =
                Instantiate(objectPool[randomIndex], new Vector2(0, spawnDistance), Quaternion.identity);
            thisObject.transform.SetParent(parent, false); 
        }
    }
}