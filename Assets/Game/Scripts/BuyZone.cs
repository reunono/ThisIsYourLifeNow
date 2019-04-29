using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using UnityEngine.UI;
using MoreMountains.Feedbacks;

namespace MoreMountains.ThisIsYourLifeNow
{

    [RequireComponent(typeof(Collider2D))]

    /// <summary>
    /// Add this class to an empty component. It will automatically add a boxcollider2d, set it to "is trigger". Then customize the dialogue zone
    /// through the inspector.
    /// </summary>

    public class BuyZone : ButtonActivated
    {
        [Header("Buy zone")]
        public GameObject PromptContainer;
        public Follower TargetFollower;

        public Text Title;
        public Text Description;
        public Text Bonus;
        public Text Malus;

        public MMFeedbacks BuyFeedback;

        [Header("Transition")]
        /// the duration of the in and out fades
        public float FadeDuration = 0.2f;

        protected WaitForSeconds _waitForSecondsFade;
        protected bool _activated = false;
        protected bool _hiding = false;
        protected WaitForSeconds _waitBuyCo;

        /// <summary>
        /// Initializes the dialogue zone
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            _waitBuyCo = new WaitForSeconds(0.2f);
            _waitForSecondsFade = new WaitForSeconds(FadeDuration);
            PromptContainer.SetActive(false);
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
            Buy();
            ActivateZone();
        }

        public override void ShowPrompt()
        {
            if (_promptHiddenForever)
            {
                return;
            }
            PromptContainer.gameObject.SetActive(true);
            MMTween.TweenScale(this, PromptContainer.transform, Vector3.zero, Vector3.one, null, 0f, FadeDuration, MMTween.MMTweenCurve.EaseOutElastic);
        }

        /// <summary>
        /// Hides the button A prompt.
        /// </summary>
        public override void HidePrompt()
        {
            MMTween.TweenScale(this, PromptContainer.transform, Vector3.one, Vector3.zero, null, 0f, FadeDuration, MMTween.MMTweenCurve.EaseOutQuartic);
            StartCoroutine(HidePromptCo());
        }

        public virtual void HidePromptForever()
        {
            StartCoroutine(HidePromptCo(true));
        }

        protected virtual IEnumerator HidePromptCo(bool forever=false)
        {
            yield return _waitForSecondsFade;
            
            if (forever)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                PromptContainer.gameObject.SetActive(false);
            }
        }

        protected virtual void Buy()
        {
            BuyFeedback.PlayFeedbacks();
            StartCoroutine(BuyCo());
        }

        protected virtual IEnumerator BuyCo()
        {
            yield return _waitBuyCo;
            TargetFollower.Buy();
            _characterButtonActivation.InButtonActivatedZone = false;
        }
    }
}