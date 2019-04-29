using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;

namespace MoreMountains.ThisIsYourLifeNow
{
    public enum FollowerEventTypes { Buy, Gift, Loss, Spawn, Destroy }

    public struct FollowerEvent
    {
        public FollowerEventTypes EventType;
        public FollowerData Origin;
        public FollowerData Cost;
        public FollowerData Option;
        public FollowerData Requirement;
        public string Reason;

        public FollowerEvent(FollowerEventTypes eventType, FollowerData origin, FollowerData cost, FollowerData option, FollowerData requirement, string reason)
        {
            EventType = eventType;
            this.Origin = origin;
            Cost = cost;
            Option = option;
            Requirement = requirement;
            Reason = reason;
        }

        static FollowerEvent e;
        public static void Trigger(FollowerEventTypes eventType, FollowerData origin, FollowerData cost, FollowerData option, FollowerData requirement, string reason)
        {
            e.EventType = eventType;
            e.Origin = origin;
            e.Cost = cost;
            e.Option = option;
            e.Requirement = requirement;
            e.Reason = reason;
            MMEventManager.TriggerEvent(e);
        }
    }

    public class Follower : MonoBehaviour, MMEventListener<FollowerEvent>
    {
        [Header("Data")]
        public FollowerData Data;
        public float StartFollowingAfterDelay = 2f;

        [Header("Buy")]
        public BuyZone TargetBuyZone;

        [Header("Feedbacks")]
        public MMFeedbacks DeathFeedback;

        [Header("Controller")]
        public GameObject Container;
        public float ModelRotationSpeed = 15f;
        public float GroundY = -7.55f;
        public MMWiggle Body;
        public GameObject LegsIdle;
        public GameObject LegsWalk;
        public ParticleSystem WalkParticles;
        public ParticleSystem TouchTheGroundParticles;
        public ParticleSystem JumpParticles;
        public float MinimumDistanceForFlip = 0.01f;
        public float MinimumDistanceForMovement = 0.001f;
        [ReadOnly]
        public bool Grounded = false;
        [ReadOnly]
        public bool GroundedLastFrame = false;

        [ReadOnly]
        public int Direction = 1;

        protected MMFollowTarget _mmFollowTarget;
        protected WaitForSeconds _waitForSecondsFollowing;
        protected Vector3 _targetModelRotation = Vector3.zero;
        protected float _distance;

        protected virtual void Start()
        {
            Initialization();   
        }

        protected virtual void Initialization()
        {
            _mmFollowTarget = this.gameObject.GetComponent<MMFollowTarget>();
            _mmFollowTarget.FollowPosition = false;

            TargetBuyZone.Title.text = Data.Title;
            TargetBuyZone.Description.text = Data.Description;
            TargetBuyZone.Bonus.text = "+ " + Data.Bonus.ToString();
            TargetBuyZone.Malus.text = "- " + Data.Malus.ToString();

            _waitForSecondsFollowing = new WaitForSeconds(StartFollowingAfterDelay);

            Direction = (Container.transform.localEulerAngles.y == 0) ? 1 : -1;

            Body.GetComponent<SpriteRenderer>().sprite = Data.BodySprite;
            
        }


        protected virtual void LateUpdate()
        {
            Move();
        }

        protected virtual void Move()
        {

            // determine direction
            _distance = Mathf.Abs(_mmFollowTarget.PositionLastFrame.x - _mmFollowTarget.Position.x);

            if (_distance > MinimumDistanceForFlip)
            {
                Direction = _mmFollowTarget.PositionLastFrame.x > _mmFollowTarget.Position.x ? -1 : 1;
            }

            if (_mmFollowTarget.FollowPosition != true)
            {
                Direction = -1;
            }

            if (_mmFollowTarget.Distance > MinimumDistanceForMovement)
            {
                LegsIdle.SetActive(false);
                LegsWalk.SetActive(true);
                Body.PositionWiggleProperties.FrequencyMin = 0.15f;
                Body.PositionWiggleProperties.FrequencyMax = 0.15f;
                if ((!WalkParticles.isPlaying) && Grounded)
                {
                    WalkParticles.Play();
                }
                if (!Grounded && WalkParticles.isPlaying)
                {
                    WalkParticles.Stop();
                }
            }
            else
            {
                LegsIdle.SetActive(true);
                LegsWalk.SetActive(false);
                Body.PositionWiggleProperties.FrequencyMin = 0.5f;
                Body.PositionWiggleProperties.FrequencyMax = 0.5f;
                if (WalkParticles.isPlaying)
                {
                    WalkParticles.Stop();
                }
            }

            if (this.transform.position.y > GroundY)
            {
                Grounded = false;
                LegsIdle.SetActive(false);
                LegsWalk.SetActive(true);
                Body.PositionWiggleProperties.FrequencyMin = 0.5f;
                Body.PositionWiggleProperties.FrequencyMax = 0.5f;
            }
            else
            {
                Grounded = true;
            }

            // if we just started touched the ground
            if (Grounded && !GroundedLastFrame)
            {
                TouchTheGroundParticles.Play();
            }

            // if we just started jumping
            if (!Grounded && GroundedLastFrame)
            {
                JumpParticles.Play();
            }

            // flip towards movement
            _targetModelRotation.y = (Direction == 1) ? 0f : 180f;
            Container.transform.localEulerAngles = Vector3.Lerp(Container.transform.localEulerAngles, _targetModelRotation, Time.deltaTime * ModelRotationSpeed);

            GroundedLastFrame = Grounded;
        }

        // called by the buy zone
        public virtual void Buy()
        {
            TargetBuyZone.HidePromptForever();
            StartCoroutine(StartFollowing());
            FollowerEvent.Trigger(FollowerEventTypes.Buy, Data, Data.CostOnBuy, Data.KilledOptionOnBuy, null, null);
        }

        protected virtual void OnBuy(FollowerEvent followerEvent)
        {
            if (followerEvent.Cost != null)
            {
                if (Data.ID == followerEvent.Cost.ID)
                {
                    DestroyFollower();
                }
            }

            if (followerEvent.Option != null)
            {
                if (Data.ID == followerEvent.Option.ID)
                {
                    DestroyFollower();
                }
            }
        }

        public virtual IEnumerator StartFollowing()
        {
            yield return _waitForSecondsFollowing;
            _mmFollowTarget.StartFollowing();
            yield return _waitForSecondsFollowing;
            CorgiEngineEvent.Trigger(CorgiEngineEventTypes.NewFollower);
        }

        protected virtual void DestroyFollower()
        {
            _mmFollowTarget.StopFollowing();
            TargetBuyZone.gameObject.SetActive(false);
            DeathFeedback?.PlayFeedbacks();
        }

        protected virtual void OnSpawn()
        {

        }

        public void OnMMEvent(FollowerEvent followerEvent)
        {

            switch (followerEvent.EventType)
            {
                case FollowerEventTypes.Buy:                    
                    OnBuy(followerEvent);
                    break;
                case FollowerEventTypes.Destroy:

                    if (followerEvent.Origin.ID != this.Data.ID)
                    {
                        return;
                    }
                    DestroyFollower();
                    break;
                
            }
        }

        protected virtual void OnEnable()
        {
            this.MMEventStartListening<FollowerEvent>();
        }

        protected virtual void OnDisable()
        {
            this.MMEventStopListening<FollowerEvent>();
        }
    }
}
