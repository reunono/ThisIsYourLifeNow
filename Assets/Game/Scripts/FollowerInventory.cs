using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using MoreMountains.Feedbacks;

namespace MoreMountains.ThisIsYourLifeNow
{
    public class FollowerInventory : PersistentSingleton<FollowerInventory>, MMEventListener<FollowerEvent>, MMEventListener<MMGameEvent>
    {
        public List<FollowerData> Content;
        public Follower FollowerPrefab;
        
        public MMFeedbacks GiftFeedback;
        public MMFeedbacks LossFeedback;

        protected string _newText;
        protected Follower _follower;
        protected Vector3 _newPosition;

        [TextArea]
        public string Story = "You were born. ";

        
        public virtual void Add(FollowerData data)
        {
            Content.Add(data);
        }

        protected virtual void SceneInit()
        {
            if (SceneManager.GetActiveScene().name == "1-Childhood")
            {
                //MMDebug.DebugLogTime("scene init, scene name : " + SceneManager.GetActiveScene().name + " clear");
                Content.Clear();
            }

            if ( (SceneManager.GetActiveScene().name == "2-Adolescence")
                || (SceneManager.GetActiveScene().name == "3-Adulthood")
                || (SceneManager.GetActiveScene().name == "4-OldAge")
                )
            {
                foreach(FollowerData data in Content)                    
                {
                    _follower = Instantiate(FollowerPrefab);
                    _follower.Data = data;
                    _newPosition = LevelManager.Instance.Players[0].transform.position;
                    _newPosition.x -= 10f;
                    _follower.transform.position = _newPosition;
                    _follower.TargetBuyZone.gameObject.SetActive(false);
                    StartCoroutine(_follower.StartFollowing());
                    CorgiEngineEvent.Trigger(CorgiEngineEventTypes.SortFollowers);
                }
            }

            if (SceneManager.GetActiveScene().name == "5-End")
            {
                Story += "\n\n";

                Story += "At the end you had ";

                for (int i=0; i<Content.Count; i++)
                {

                    if (i < Content.Count - 2)
                    {
                        Story += "<color=red>"+Content[i].MessageDisplayText + "</color>, ";
                    }
                    if (i == Content.Count - 2)
                    {
                        Story += "<color=red>" + Content[i].MessageDisplayText + "</color> and ";
                    }
                    if (i == Content.Count - 1)
                    {
                        Story += "finally <color=red>you died</color>.";
                    }
                }

                Story += "\n\n";

                Story += "This was your life. You could have done better. Press any key to try a new life and maybe make better choices.";
            }
        }

        public virtual void Remove(FollowerData data)
        {
            Content.Remove(data);
        }

        public virtual bool Contains(FollowerData data)
        {
            if (data == null)
            {
                return false;
            }
            return Content.Contains(data);
        }

        public void OnMMEvent(FollowerEvent followerEvent)
        {
            switch (followerEvent.EventType)
            {
                case FollowerEventTypes.Buy:
                    // adds to the inventory
                    Add(followerEvent.Origin);
                    _newText = "You've obtained <color=yellow>" + followerEvent.Origin.MessageDisplayText + "</color>. ";
                    Story += "Got <color=yellow>" + followerEvent.Origin.MessageDisplayText + "</color>. ";

                    // test cost                   
                    if (Contains(followerEvent.Cost))
                    {
                        Remove(followerEvent.Cost);
                        _newText += "It cost you <color=yellow>" + followerEvent.Cost.MessageDisplayText + "</color>. ";
                        Story += "Lost <color=yellow>" + followerEvent.Cost.MessageDisplayText + "</color>. ";
                    }

                    // test option
                    if (followerEvent.Option != null)
                    {
                        _newText += "You'll never get <color=yellow>" + followerEvent.Option.MessageDisplayText + "</color>.";
                    }                    

                    MessageEvent.Trigger(_newText);
                    break;

                case FollowerEventTypes.Gift:
                    Add(followerEvent.Origin);
                    _newText = "You've got <color=yellow>" + followerEvent.Origin.MessageDisplayText + "</color> now";
                    Story += "Got <color=yellow>" + followerEvent.Origin.MessageDisplayText + "</color>. ";
                    if (followerEvent.Reason != "")
                    {
                        _newText += " because " + followerEvent.Reason+".";
                    }
                    else
                    {
                        _newText += ".";
                    }
                    MessageEvent.Trigger(_newText);
                    GiftFeedback.PlayFeedbacks();
                    break;

                case FollowerEventTypes.Loss:
                    if (Contains(followerEvent.Origin))
                    {
                        FollowerEvent.Trigger(FollowerEventTypes.Destroy, followerEvent.Origin, null, null, null, null);

                        _newText = "You just lost <color=yellow>" + followerEvent.Origin.MessageDisplayText + "</color>";
                        Story += "Lost <color=yellow>" + followerEvent.Origin.MessageDisplayText + "</color>. ";
                        Remove(followerEvent.Origin);
                        if (followerEvent.Reason != "")
                        {
                            _newText += " because " + followerEvent.Reason + ".";
                        }
                        else
                        {
                            _newText += ".";
                        }
                        MessageEvent.Trigger(_newText);
                        LossFeedback.PlayFeedbacks();
                    }
                    break;
            }
        }

        protected virtual void OnEnable()
        {
            this.MMEventStartListening<FollowerEvent>();
            this.MMEventStartListening<MMGameEvent>();
        }

        protected virtual void OnDisable()
        {
            this.MMEventStopListening<FollowerEvent>();
            this.MMEventStopListening<MMGameEvent>();
        }

        public void OnMMEvent(MMGameEvent gameEvent)
        {
            if (gameEvent.EventName == "Load")
            {
                SceneInit();
            }
        }
    }
}
