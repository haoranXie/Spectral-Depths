using UnityEngine;

namespace EmeraldAI
{
    [System.Serializable]
    public class AnimationClass
    {
        public AnimationClass(float NewAnimationSpeed, AnimationClip NewAnimationClip, bool NewMirror)
        {
            AnimationSpeed = NewAnimationSpeed;
            AnimationClip = NewAnimationClip;
            Mirror = NewMirror;
        }

        public float AnimationSpeed = 1;
        public AnimationClip AnimationClip;
        public bool Mirror = false;
    }
}