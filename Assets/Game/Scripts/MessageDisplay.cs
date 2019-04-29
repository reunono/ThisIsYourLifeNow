using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine.UI;

namespace MoreMountains.ThisIsYourLifeNow
{
    public struct MessageEvent
    {
        public string Message;

        public MessageEvent(string message)
        {
            Message = message;
        }

        static MessageEvent e;
        public static void Trigger(string message)
        {
            e.Message = message;
            MMEventManager.TriggerEvent(e);
        }
    }

    public class MessageDisplay : MonoBehaviour, MMEventListener<MessageEvent>
    {
        public GameObject Container;
        public Text TargetText;
        public float InitialDelay = 1f;
        public float DisplayDuration = 3f;
        public float TransitionDuration = 0.5f;

        protected WaitForSeconds _initialWaitForSeconds;
        protected WaitForSeconds _displayWaitForSeconds;
        protected WaitForSeconds _transitionWaitForSeconds;

        protected virtual void Start()
        {
            _initialWaitForSeconds = new WaitForSeconds(InitialDelay);
            _displayWaitForSeconds = new WaitForSeconds(DisplayDuration);
            _transitionWaitForSeconds = new WaitForSeconds(TransitionDuration);
            Container.SetActive(false);
        }

        public void OnMMEvent(MessageEvent messageEvent)
        {
            DisplayMessage(messageEvent.Message);
        }

        protected virtual void DisplayMessage(string message)
        {
            TargetText.text = message;
            StartCoroutine(DisplayMessageCo());
        }

        protected virtual IEnumerator DisplayMessageCo()
        {
            yield return _initialWaitForSeconds;
            Container.SetActive(true);
            MMTween.TweenScale(this, this.transform, Vector3.zero, Vector3.one, null, 0f, TransitionDuration, MMTween.MMTweenCurve.EaseOutOverhead);
            yield return _displayWaitForSeconds;
            MMTween.TweenScale(this, this.transform, Vector3.one, Vector3.zero, null, 0f, TransitionDuration, MMTween.MMTweenCurve.EaseInQuadratic);
            yield return _transitionWaitForSeconds;
            Container.SetActive(false);
        }

        protected virtual void OnEnable()
        {
            this.MMEventStartListening<MessageEvent>();
        }

        protected virtual void OnDisable()
        {
            this.MMEventStopListening<MessageEvent>();
        }
    }   
}