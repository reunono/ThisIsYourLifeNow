using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace MoreMountains.ThisIsYourLifeNow
{
    public class ShuffleBag
    {
        
        private List<float> data;

        private float currentItem;
        private int currentPosition = -1;

        private int Capacity { get { return data.Capacity; } }
        public int Size { get { return data.Count; } }

        public ShuffleBag(int initCapacity)
        {
            data = new List<float>(initCapacity);
        }

        public void Add(float item, int amount)
        {
            for (int i = 0; i < amount; i++)
                data.Add(item);

            currentPosition = Size - 1;
        }

        public float Next()
        {
            if (currentPosition < 1)
            {
                currentPosition = Size - 1;
                currentItem = data[0];

                return currentItem;
            }

            int pos = Random.Range(0,currentPosition);

            currentItem = data[pos];
            data[pos] = data[currentPosition];
            data[currentPosition] = currentItem;
            currentPosition--;

            return currentItem;
     

        }
    }
}
