using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
using DIALOGUE;

namespace TESTING
{
    public class AudioTesting : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Running());
        }

        Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

        IEnumerator Running2()
        {
            /**
            Character_Sprite Raelin = CreateCharacter("Raelin") as Character_Sprite;
            Character Me = CreateCharacter("Me");
            Raelin.Show();

            AudioManager.instance.PlaySoundEffect("Audio/SFX/RadioStatic", loop: true);

            yield return Me.Say("Please turn off the radio.");

            AudioManager.instance.StopSoundEffect("RadioStatic");
            AudioManager.instance.PlayVoice("Audio/Voices/Exclamation");

            Raelin.Say("Okay!");
            **/

            yield return null;
        }

        IEnumerator Running3()
        {
            /**
            yield return new WaitForSeconds(1);

            Character_Sprite Raelin = CreateCharacter("Raelin") as Character_Sprite;
            Raelin.Show();

            yield return DialogueSystem.instance.Say("Narrator", "Can we see your ship?");

            GraphicPanelManager.instance.GetPanel("background").GetLayer(0, true).SetTexture("Graphics/BG Images/5");
            AudioManager.instance.PlayTrack("Audio/Music/Calm", volumeCap: 0.5f);
            AudioManager.instance.PlayVoice("Audio/Voices/wakeup");

            Raelin.SetSprite(Raelin.GetSprite("B1"), 0);
            Raelin.SetSprite(Raelin.GetSprite("B_Shocked"), 1);
            Raelin.MoveToPosition(new Vector2(0.7f, 0), speed: 0.5f);
            yield return Raelin.Say("Yes, of course!");

            yield return Raelin.Say("Let me show you the engine room.");

            GraphicPanelManager.instance.GetPanel("background").GetLayer(0, true).SetTexture("Graphics/BG Images/EngineRoom");
            AudioManager.instance.PlayTrack("Audio/Music/Happy", volumeCap: 0.5f);
            **/
            yield return null;
        }


        IEnumerator Running()
        {
            Character_Sprite Raelin = CreateCharacter("Raelin") as Character_Sprite;
            Character Me = CreateCharacter("Me");
            Raelin.Show();

            GraphicPanelManager.instance.GetPanel("background").GetLayer(0, true).SetTexture("Graphics/BG Images/villagenight");

            AudioManager.instance.PlayTrack("Audio/Ambience/RainyMood", 0);
            AudioManager.instance.PlayTrack("Audio/Music/Calm", 1, pitch: 0.7f);

            yield return Raelin.Say("We can have multiple channels for playing ambience as well as music!");

            AudioManager.instance.StopTrack(1);

            yield return null;
        }
    }
}