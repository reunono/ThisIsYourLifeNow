using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using UnityEngine.UI;

namespace MoreMountains.ThisIsYourLifeNow
{
    public class FollowerSorter : MonoBehaviour, MMEventListener<CorgiEngineEvent>
    {
        public Follower[] Followers;

        protected Vector3 _newPosition;

        protected virtual void Start()
        {
            Sort();
        }

        protected virtual void Sort()
        {
            Followers = FindObjectsOfType<Follower>();

            // fill the bag
            var shuffleBag = new ShuffleBag(40);
            int amount = 1;
            float minValue = -2f;
            float maxValue = 2f;
            float newValue = 0f;
            for (int i = 0; i < 40; i++)
            {
                newValue = MMMaths.Remap(i, 0, 40, minValue, maxValue);
                if (newValue == 0f)
                {
                    newValue = 2.2f;
                }
                shuffleBag.Add(newValue, amount);
            }

            float followOffset = 1f;
            foreach (Follower follower in Followers)
            {
                _newPosition = follower.transform.position;
                _newPosition.z = shuffleBag.Next() * 1.5f + Random.Range(0.01f, 0.04f);

                follower.transform.position = _newPosition;
                int dice = MMMaths.RollADice(4);

                float offset = shuffleBag.Next();
                if (offset > 0f)
                {
                    offset += 2.5f;
                }
                else
                {
                    offset -= 2.5f;
                }
                follower.gameObject.GetComponentNoAlloc<MMFollowTarget>().SetXOffset(offset);

                
                follower.gameObject.GetComponentNoAlloc<MMFollowTarget>().FollowPositionSpeed = Random.Range(3f, 10f);

                followOffset += 0.2f;
            }
        }

        public void OnMMEvent(CorgiEngineEvent engineEvent)
        {
            if (engineEvent.EventType == CorgiEngineEventTypes.SortFollowers)
            {
                Sort();
            }
        }

        protected virtual void OnEnable()
        {
            this.MMEventStartListening<CorgiEngineEvent>();
        }
        protected virtual void OnDisable()
        {
            this.MMEventStopListening<CorgiEngineEvent>();
        }
    }
}
