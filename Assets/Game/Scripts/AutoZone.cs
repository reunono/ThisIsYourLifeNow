using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using UnityEngine.UI;

namespace MoreMountains.ThisIsYourLifeNow
{

    [RequireComponent(typeof(Collider2D))]

    /// <summary>
    /// Add this class to an empty component. It will automatically add a boxcollider2d, set it to "is trigger". Then customize the dialogue zone
    /// through the inspector.
    /// </summary>

    public class AutoZone : ButtonActivated
    {
        [Header("Auto zone")]

        public Follower FollowerPrefab;

        public FollowerData Taken;
        public FollowerData Given;
        public FollowerData Requirement;
        public FollowerData RequirementMissing;
        public string Reason;

        public float xOffset = 10f;

        protected Vector3 _newPosition;
        protected Follower _givenFollower;


        protected virtual void Awake()
        {
            if (Given != null)
            {
                _givenFollower = Instantiate(FollowerPrefab);
                _givenFollower.Data = Given;
                _newPosition = this.transform.position;
                _newPosition.y = 23f;
                _givenFollower.transform.position = _newPosition;
                _givenFollower.TargetBuyZone.gameObject.SetActive(false);

            }       
        }

        /// <summary>
        /// Initializes the dialogue zone
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }

        protected virtual void GiveOrTake()
        {
            if (Requirement != null)
            {
                if (!FollowerInventory.Instance.Contains(Requirement))
                {
                    return;
                }
            }

            if (RequirementMissing != null)
            {
                if (FollowerInventory.Instance.Contains(RequirementMissing))
                {
                    return;
                }
            }

            if (Taken != null)
            {
                FollowerEvent.Trigger(FollowerEventTypes.Loss, Taken, null, null, Requirement, Reason);
            }

            if (Given != null)
            {
                FollowerEvent.Trigger(FollowerEventTypes.Gift, Given, null, null, Requirement, Reason);
                // we instantiate the given follower and make it follow the player
                _newPosition.z = _givenFollower.transform.position.z;
                _newPosition.x = LevelManager.Instance.Players[0].transform.position.x;
                _newPosition.y = LevelManager.Instance.Players[0].transform.position.y;
                _newPosition.x -= xOffset;
                _givenFollower.transform.position = _newPosition;
                StartCoroutine(_givenFollower.StartFollowing());
            }
        }

        /// <summary>
        /// When the button is pressed we start the dialogue
        /// </summary>
        public override void TriggerButtonAction()
        {
            if (!CheckNumberOfUses())
            {
                return;
            }
            base.TriggerButtonAction();            
            ActivateZone();
            GiveOrTake();
        }
    }
}