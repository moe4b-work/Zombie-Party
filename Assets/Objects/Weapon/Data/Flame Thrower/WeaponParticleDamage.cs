﻿using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Game
{
	public class WeaponParticleDamage : Weapon.Module
	{
        [SerializeField]
        protected ParticleSystem particle;
        public ParticleSystem Particle { get { return particle; } }

        [SerializeField]
        protected LayerMask mask = Physics.DefaultRaycastLayers;
        public LayerMask Mask { get { return mask; } }

        [SerializeField]
        protected float damage = 100f;
        public float Damage { get { return damage; } }

        [SerializeField]
        protected float radius = 1f;
        public float Radius { get { return radius; } }

        [SerializeField]
        [Range(0f, 1f)]
        protected float minLifeTime = 0.3f;
        public float MinLifeTime { get { return minLifeTime; } }

        protected virtual void Reset()
        {
            particle = GetComponent<ParticleSystem>();
        }

        public override void Init(Weapon weapon)
        {
            base.Init(weapon);

            particles = new ParticleSystem.Particle[particle.main.maxParticles];
        }

        void Update()
        {
            CalculateRange();

            CalculateOverlap();

            ApplyDamage();
        }

        float range = 0f;
        ParticleSystem.Particle[] particles;
        void CalculateRange()
        {
            var count = particle.GetParticles(particles);

            range = 0f;

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    if (particles[i].remainingLifetime / particle.main.startLifetime.Evaluate(0f) < minLifeTime) continue;

                    var localPosition = particle.transform.InverseTransformPoint(particles[i].position);

                    if (localPosition.z > range)
                        range = localPosition.z;
                }

                range /= 2f;
            }
        }

        Queue<Entity> targets = new Queue<Entity>();
        void CalculateOverlap()
        {
            var p1 = particle.transform.position;
            var p2 = p1 + particle.transform.forward * range;

            var array = Physics.OverlapCapsule(p1, p2, radius, mask, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].attachedRigidbody == null) continue;

                var entity = array[i].GetComponent<Entity>();

                if (entity == null) continue;

                if (targets.Contains(entity)) continue;

                targets.Enqueue(entity);
            }
        }

        void ApplyDamage()
        {
            while (targets.Count != 0)
            {
                weapon.Damage(targets.Dequeue(), damage * Time.deltaTime);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;

            var p1 = particle.transform.position;
            var p2 = p1 + particle.transform.forward * range;

            var p3 = particle.transform.position + (particle.transform.forward * range / 2f) + (particle.transform.right * radius / 2f);
            var p4 = particle.transform.position + (particle.transform.forward * range / 2f) - (particle.transform.right * radius / 2f);

            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p3, p4);
        }
    }
}