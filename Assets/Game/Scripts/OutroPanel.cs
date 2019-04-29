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
    public class OutroPanel : MonoBehaviour
    {
        public Text OutroText;

        void Start()
        {
            MMGameEvent.Trigger("Load");
            OutroText.text = FollowerInventory.Instance.Story;
        }

        private void Update()
        {
            if (Input.anyKey)
            {
                Destroy(FollowerInventory.Instance.gameObject);
                LoadingSceneManager.LoadScene("1-Childhood");
            }
        }
    }
}
