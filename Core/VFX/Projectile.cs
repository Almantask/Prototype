using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Core.VFX {
    public class Projectile : MonoBehaviour {
        [SerializeField] protected ParticleSystem _mainParticle;
        public ParticleSystem MainParticle { get { return _mainParticle; } }

        [SerializeField] protected ParticleSystem _collisionParticle;
        public ParticleSystem CollisionParticle { get { return _collisionParticle; } }

        public bool Hit { get; private set; }

        private void OnTriggerEnter(Collider col) {
            StartCoroutine(CollisionRoutine(col));
        }

        private IEnumerator CollisionRoutine(Collider col) {
            Hit = true;
            CollisionParticle.Play(true);
            yield return new WaitForSeconds(1f);
            MainParticle.Stop(true);
            Destroy(gameObject);
        }
    }
}