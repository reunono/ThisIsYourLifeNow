using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using System;

namespace MoreMountains.ThisIsYourLifeNow
{
    public enum LifeCurrencies { Health, Safety, Social, Status, Esteem }

    [Serializable]
    [CreateAssetMenu(fileName = "FollowerData", menuName = "MoreMountains/FollowerData", order = 0)]
    public class FollowerData : ScriptableObject
    {
        [Header("Info")]
        public string ID;
        public string Title;
        public string MessageDisplayText;
        [TextArea]
        public string Description;
        public Sprite BodySprite;
        public FollowerData CostOnBuy;
        public FollowerData KilledOptionOnBuy;

        public LifeCurrencies Bonus = LifeCurrencies.Health;
        public LifeCurrencies Malus = LifeCurrencies.Safety;

        
        


    }
}