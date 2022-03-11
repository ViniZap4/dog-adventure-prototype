using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AprendaUnity
{
    public class Grass : MonoBehaviour
    {
        public ParticleSystem fxHit;
        private bool isCut;

        void GetHit(int amount)
        {
            if (isCut == false)
            {
                isCut = true;
                transform.localScale = new Vector3(1f, 1f, 1f);
                fxHit.Emit(10);
            }
        }


    }
}
