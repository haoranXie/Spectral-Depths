using UnityEngine;

namespace EmeraldAI
{
    [System.Serializable]
    public class EmoteAnimationClass
    {
        public EmoteAnimationClass(int NewAnimationID, AnimationClip NewEmoteAnimationClip)
        {
            AnimationID = NewAnimationID;
            EmoteAnimationClip = NewEmoteAnimationClip;
        }

        public int AnimationID = 1;
        public AnimationClip EmoteAnimationClip;
    }
}