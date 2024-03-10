using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmeraldAI.Utility
{
    /// <summary>
    /// Populates the Animation Preview Editor with the preset Animation Event data.
    /// </summary>
    public static class AnimationEventInitializer
    {
        public static List<EmeraldAnimationEventsClass> GetEmeraldAnimationEvents ()
        {
            List<EmeraldAnimationEventsClass> EmeraldAnimationEvents = new List<EmeraldAnimationEventsClass>();

            //Custom
            AnimationEvent Custom = new AnimationEvent();
            Custom.functionName = "---YOUR FUNCTION NAME HERE---";
            EmeraldAnimationEvents.Add(new EmeraldAnimationEventsClass("Custom", Custom, "A custom/default event with no added parameters"));

            //Emerald Attack Event
            AnimationEvent EmeraldAttack = new AnimationEvent();
            EmeraldAttack.functionName = "CreateAbility";
            EmeraldAnimationEvents.Add(new EmeraldAnimationEventsClass("Create Ability", EmeraldAttack, "An event used for creating an AI's current ability (previously called EmeraldAttackEvent). This is required for Ability Objects to be triggered and should be done for all attack animations.\n\nNote: If your AI uses Attack Transform, " +
                "you should include the name of the Attack Transform in the String Paramter of this event. This will allow an ability to spawn from the Attack Transform location."));

            //Charge Ability
            AnimationEvent ChargeEffect = new AnimationEvent();
            ChargeEffect.functionName = "ChargeEffect";
            EmeraldAnimationEvents.Add(new EmeraldAnimationEventsClass("Charge Effect", ChargeEffect, "An event used for triggering an AI's current abilty's Charge Effect. You will need to add the Attack Transform you would like the charge effect to spawn at. " +
                "This is done through the String Parameter and is based off of the AI's Attack Transform list within its Combat Component. An Ability Object must have a Charge Module and have it enabled or this event will be skipped." +
                "\n\nNote: This will not create an ability. The CreateAbility event still needs to be assigned, which should be after a Charge Effect event. This Animation Event is completely optional."));

            //Fade Out IK
            AnimationEvent FadeOutIK = new AnimationEvent();
            FadeOutIK.functionName = "FadeOutIK";
            FadeOutIK.floatParameter = 5f;
            FadeOutIK.stringParameter = "---YOUR RIG NAME TO FADE HERE---";
            EmeraldAnimationEvents.Add(new EmeraldAnimationEventsClass("Fade Out IK", FadeOutIK, "Fade out an AI's IK overtime. This is helpful if an AI's IK is interfering with certain animations " +
                "(such as hit, equipping, certain attacks, and death animations).\n\nFloatParamer = Fade Out Time (In Seconds)\n\nStringParameter = The name of your rig you'd like to fade out"));

            //Fade In IK
            AnimationEvent FadeInIK = new AnimationEvent();
            FadeInIK.functionName = "FadeInIK";
            FadeInIK.floatParameter = 5f;
            FadeInIK.stringParameter = "---YOUR RIG NAME TO FADE HERE---";
            EmeraldAnimationEvents.Add(new EmeraldAnimationEventsClass("Fade In IK", FadeInIK, "Fade in an AI's IK overtime. This should be used after Fade Out IK has been used.\n\nFloatParamer = Fade In Time (In Seconds)\n\nStringParameter = The name of your rig you'd like to fade in"));

            //Enable Weapon Collider
            AnimationEvent EnableWeaponCollider = new AnimationEvent();
            EnableWeaponCollider.functionName = "EnableWeaponCollider";
            EnableWeaponCollider.stringParameter = "---THE NAME OF YOUR AI'S WEAPON HERE---";
            EmeraldAnimationEvents.Add(new EmeraldAnimationEventsClass("Enable Weapon Collider", EnableWeaponCollider, "Enables an AI's weapon's collider (The weapon object must also have a WeaponCollider component and be set up through an AI's EmeraldItems component)." +
                "\n\nNote: You must also assign the gameobject name of your AI's weapon to the String parameter of this Animation Event. This is used to search through an AI's Items Component to find which weapon to enable. For a detailed tutorial on this, see the Emerald AI Wiki."));

            //Disable Weapon Collider
            AnimationEvent DisableWeaponCollider = new AnimationEvent();
            DisableWeaponCollider.functionName = "DisableWeaponCollider";
            DisableWeaponCollider.stringParameter = "---THE NAME OF YOUR AI'S WEAPON HERE---";
            EmeraldAnimationEvents.Add(new EmeraldAnimationEventsClass("Disable Weapon Collider", DisableWeaponCollider, "Disables an AI's weapon's collider (The weapon object must also have a WeaponCollider component and be set up through an AI's EmeraldItems component)." +
                "\n\nNote: You must also assign the gameobject name of your AI's weapon to the String parameter of this Animation Event. This is used to search through an AI's Items Component to find which weapon to disable. For a detailed tutorial on this, see the Emerald AI Wiki."));

            //Equip Weapon 1
            AnimationEvent EquipWeapon1 = new AnimationEvent();
            EquipWeapon1.functionName = "EquipWeapon";
            EquipWeapon1.stringParameter = "Weapon Type 1";
            EmeraldAnimationEvents.Add(new EmeraldAnimationEventsClass("Equip Weapon Type 1", EquipWeapon1, "Equip an AI's Type 1 Weapon (The weapon object must be setup through Emerald AI)"));

            //Equip Weapon 2
            AnimationEvent EquipWeapon2 = new AnimationEvent();
            EquipWeapon2.functionName = "EquipWeapon";
            EquipWeapon2.stringParameter = "Weapon Type 2";
            EmeraldAnimationEvents.Add(new EmeraldAnimationEventsClass("Equip Weapon Type 2", EquipWeapon2, "Equip an AI's Type 2 Weapon (The weapon object must be setup through Emerald AI)"));

            //Unequip Weapon 1
            AnimationEvent UnequipWeapon1 = new AnimationEvent();
            UnequipWeapon1.functionName = "UnequipWeapon";
            UnequipWeapon1.stringParameter = "Weapon Type 1";
            EmeraldAnimationEvents.Add(new EmeraldAnimationEventsClass("Unequip Weapon Type 1", UnequipWeapon1, "Unquip an AI's Type 1 Weapon (The weapon object must be setup through Emerald AI)"));

            //Unequip Weapon 2
            AnimationEvent UnequipWeapon2 = new AnimationEvent();
            UnequipWeapon2.functionName = "UnequipWeapon";
            UnequipWeapon2.stringParameter = "Weapon Type 2";
            EmeraldAnimationEvents.Add(new EmeraldAnimationEventsClass("Unequip Weapon Type 2", UnequipWeapon2, "Unequip an AI's Type 2 Weapon (The weapon object must be setup through Emerald AI)"));

            //Enable Item
            AnimationEvent EnableItem = new AnimationEvent();
            EnableItem.functionName = "EnableItem";
            EmeraldAnimationEvents.Add(new EmeraldAnimationEventsClass("Enable Item", EnableItem, "Enable an Item by passing the ItemID. This is based off of an AI's Item List and an AI must have an EmeraldAIItem component.\n\nIntParameter = ItemID"));

            //Disable Item
            AnimationEvent DisableItem = new AnimationEvent();
            DisableItem.functionName = "DisableItem";
            EmeraldAnimationEvents.Add(new EmeraldAnimationEventsClass("Disable Item", DisableItem, "Disable an Item by passing the ItemID. This is based off of an AI's Item List and an AI must have an EmeraldAIItem component.\n\nIntParameter = ItemID"));

            //Walk Footstep Sound
            AnimationEvent WalkFootstepSound = new AnimationEvent();
            WalkFootstepSound.functionName = "WalkFootstepSound";
            EmeraldAnimationEvents.Add(new EmeraldAnimationEventsClass("Play Walk Sound", WalkFootstepSound, "Play a random walk sound based off of your AI's Walk Sound List."));

            //Run Footstep Sound
            AnimationEvent RunFootstepSound = new AnimationEvent();
            RunFootstepSound.functionName = "RunFootstepSound";
            EmeraldAnimationEvents.Add(new EmeraldAnimationEventsClass("Play Run Sound", RunFootstepSound, "Play a random run sound based off of your AI's Run Sound List."));

            //Play Attack Sound
            AnimationEvent PlayAttackSound = new AnimationEvent();
            PlayAttackSound.functionName = "PlayAttackSound";
            EmeraldAnimationEvents.Add(new EmeraldAnimationEventsClass("Play Attack Sound", PlayAttackSound, "Play a random attack sound based off of your AI's Attack Sound List."));

            //Play Sound Effect
            AnimationEvent PlaySoundEffect = new AnimationEvent();
            PlaySoundEffect.functionName = "PlaySoundEffect";
            EmeraldAnimationEvents.Add(new EmeraldAnimationEventsClass("Play Sound Effect", PlaySoundEffect, "Play a specified sound from an AI's Sounds List by passing the SoundEffectID.\n\nIntParameter = SoundEffectID"));

            //Play Warning Sound
            AnimationEvent PlayWarningSound = new AnimationEvent();
            PlayWarningSound.functionName = "PlayWarningSound";
            EmeraldAnimationEvents.Add(new EmeraldAnimationEventsClass("Play Warning Sound", PlayWarningSound, "Play a random warning sound based off of your AI's Warning Sound List."));

            return EmeraldAnimationEvents;
        }
    }
}