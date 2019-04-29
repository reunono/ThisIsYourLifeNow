using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine.SceneManagement;

namespace MoreMountains.ThisIsYourLifeNow
{
    public class CharacterItsYourLifeNow : CharacterAbility
    {
        public MMWiggle BodyKid;
        public MMWiggle BodyTeenager;
        public MMWiggle BodyAdult;
        public MMWiggle BodyOld;
        public GameObject IdleLegs;
        public GameObject WalkLegs;

        protected MMWiggle _body;

        protected override void Initialization()
        {
            base.Initialization();
            BodyKid.gameObject.SetActive(false);
            BodyTeenager.gameObject.SetActive(false);
            BodyAdult.gameObject.SetActive(false);
            BodyOld.gameObject.SetActive(false);

            switch(SceneManager.GetActiveScene().name)
            {
                case "1-Childhood":
                    _body = BodyKid;
                    break;
                case "2-Adolescence":
                    _body = BodyTeenager;
                    break;
                case "3-Adulthood":
                    _body = BodyAdult;
                    break;
                case "4-OldAge":
                    _body = BodyOld;
                    break;
            }
            _body.gameObject.SetActive(true);
        }

        public override void ProcessAbility()
        {
            base.ProcessAbility();

            if (_movement.CurrentState == CharacterStates.MovementStates.Idle)
            {
                IdleLegs.SetActive(true);
                WalkLegs.SetActive(false);
                _body.PositionWiggleProperties.FrequencyMin = 0.5f;
                _body.PositionWiggleProperties.FrequencyMax = 0.5f;
            }
            else
            {
                IdleLegs.SetActive(false);
                WalkLegs.SetActive(true);
                _body.PositionWiggleProperties.FrequencyMin = 0.15f;
                _body.PositionWiggleProperties.FrequencyMax = 0.15f;
            }            
        }
    }
}
