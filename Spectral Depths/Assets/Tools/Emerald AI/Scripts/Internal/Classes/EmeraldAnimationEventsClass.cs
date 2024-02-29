using UnityEngine;

namespace EmeraldAI
{
    public class EmeraldAnimationEventsClass
    {
        public string eventDisplayName;
        public string eventDescription;
        public AnimationEvent animationEvent;

        public EmeraldAnimationEventsClass(string m_eventDisplayName, AnimationEvent m_animationEvent, string m_eventDescription)
        {
            eventDisplayName = m_eventDisplayName;
            animationEvent = m_animationEvent;
            eventDescription = m_eventDescription;
        }
    }
}