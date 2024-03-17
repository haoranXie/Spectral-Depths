using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CHARACTERS
{
    //The Base class from which all character types derive from
    public class Character
    {
        public const bool ENABLE_ON_START = true;//WHETHER TO SHOW CHARACTERS ON START OR NOT (use for testing so you dont have to .show() every time)
        private const float UNHIGHLIGHTED_DARKEN_STRENGTH = 0.65f; //CHANGE TO CUSTOMIZE HOW DARK WHEN CHARACTERS ARE NOT HIGHLIGHTED

        public string name = "";
        public string displayName = "";
        public RectTransform root = null;
        public CharacterConfigData config;
        public Animator animator;
        public Color color { get; private set; } = Color.white;
        protected Color displayColor => highlighted ? highlightedColor : unhighlightedColor;
        protected Color highlightedColor => color;
        protected Color unhighlightedColor => new Color(color.r * UNHIGHLIGHTED_DARKEN_STRENGTH, color.g * UNHIGHLIGHTED_DARKEN_STRENGTH, color.b * UNHIGHLIGHTED_DARKEN_STRENGTH, color.a);


        public bool highlighted { get; protected set; } = true;

        protected CharacterManager characterManager => CharacterManager.instance;
        public DialogueSystem dialogueSystem => DialogueSystem.instance;

        //Coroutines
        protected Coroutine co_revealing, co_hiding;
        protected Coroutine co_moving;
        protected Coroutine co_changingColor;
        protected Coroutine co_highlighting;
        public bool isRevealing => co_revealing != null;
        public bool isHiding => co_hiding != null;
        public bool isMoving => co_moving != null;
        public bool isChangingColor => co_changingColor != null;
        public bool isHighlighting => (highlighted && co_highlighting != null);
        public bool isUnHighlighting => (!highlighted && co_highlighting != null);
        public virtual bool isVisible { get; set; }

        //When creating a new character
        public Character(string name, CharacterConfigData config, GameObject prefab)
        {
            this.name = name;
            displayName = name;
            this.config = config;

            if (prefab != null)
            {
                GameObject ob = Object.Instantiate(prefab, characterManager.characterPanel);
                ob.name = characterManager.FormatCharacterPath(characterManager.characterPrefabNameFormat, name);
                ob.SetActive(true);
                root = ob.GetComponent<RectTransform>();
                animator = root.GetComponentInChildren<Animator>();
            }
        }

        public Coroutine Say(string dialogue) => Say(new List<string> { dialogue });

        public Coroutine Say(List<string> dialogue)
        {
            dialogueSystem.ShowSpeakerName(displayName);
            UpdateTextCustomizationsOnScreen();

            //NOT OFFICIAL
            dialogue = FormatDialogue(dialogue);
            //NOT OFFICIAL

            return dialogueSystem.Say(dialogue);
        }

        public void SetNameFont(TMP_FontAsset font) => config.nameFont = font;
        public void SetDialogueFont(TMP_FontAsset font) => config.dialogueFont = font;
        public void SetNameColor(Color color) => config.nameColor = color;
        public void SetDialogueColor(Color color) => config.dialogueColor = color;
        public void ResetConfigurationData() => config = CharacterManager.instance.GetCharacterConfig(name);
        public void UpdateTextCustomizationsOnScreen() => dialogueSystem.ApplySpeakerDataToDialogueContainer(config);

        //Reveal Character
        public virtual Coroutine Show()
        {
            if (isRevealing)
                return co_revealing;

            if (isHiding)
                characterManager.StopCoroutine(co_hiding);

            co_revealing = characterManager.StartCoroutine(ShowingOrHiding(true));

            return co_revealing;
        }

        //Hide Character
        public virtual Coroutine Hide()
        {
            if (isHiding)
                return co_hiding;

            if (isRevealing)
                characterManager.StopCoroutine(co_revealing);

            co_hiding = characterManager.StartCoroutine(ShowingOrHiding(false));

            return co_hiding;
        }

        //Determining whether to show or hide
        public virtual IEnumerator ShowingOrHiding(bool show)
        {
            Debug.Log("Show/Hide cannot be called from a base character type.");
            yield return null;
        }

        //Setting position of Character Sprites
        public virtual void SetPosition(Vector2 position)
        {
            if (root == null)
                return;

            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPositionToRelativeCharacterAnchorTargets(position);

            root.anchorMin = minAnchorTarget;
            root.anchorMax = maxAnchorTarget;
        }

        public virtual Coroutine MoveToPosition(Vector2 position, float speed = 2f, bool teleport = false)
        {
            if (root == null)
                return null;

            if (isMoving)
                characterManager.StopCoroutine(co_moving);

            co_moving = characterManager.StartCoroutine(MovingToPosition(position, speed, teleport));

            return co_moving;
        }

        private IEnumerator MovingToPosition(Vector2 position, float speed, bool teleport)
        {
            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPositionToRelativeCharacterAnchorTargets(position);
            Vector2 padding = root.anchorMax - root.anchorMin;

            if (teleport == true)
            {
                Hide();
                while (root.anchorMin != minAnchorTarget || root.anchorMax != maxAnchorTarget)
                {
                    root.anchorMin = Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed * Time.deltaTime * 0.75f); //CHANGE SPEED OF MOVEMENT HERE
                    root.anchorMax = root.anchorMin + padding;
                    yield return null;
                }
                Show();
                yield return new WaitForSeconds(0.5f);
                
            }
            else
            {
                while (root.anchorMin != minAnchorTarget || root.anchorMax != maxAnchorTarget)
                {
                    //root.anchorMin = teleport ?
                    //Vector2.Lerp(root.anchorMin, minAnchorTarget, speed * Time.deltaTime)
                    root.anchorMin = Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed * Time.deltaTime * 0.75f); //CHANGE SPEED OF MOVEMENT HERE
                    root.anchorMax = root.anchorMin + padding;

                    /**
                    if (teleport && Vector2.Distance(root.anchorMin, minAnchorTarget) <= 0.001f)
                    {
                        root.anchorMin = minAnchorTarget;
                        root.anchorMax = maxAnchorTarget;
                        break;
                    }
                    **/

                    yield return null;
                }
            }

            Debug.Log("Done Moving");
            co_moving = null;
        }

        protected (Vector2, Vector2) ConvertUITargetPositionToRelativeCharacterAnchorTargets(Vector2 position)
        {
            Vector2 padding = root.anchorMax - root.anchorMin;

            float maxX = 1f - padding.x;
            float maxY = 1f - padding.y;

            Vector2 minAnchorTarget = new Vector2(maxX * position.x, maxY * position.y);
            Vector2 maxAnchorTarget = minAnchorTarget + padding;

            return (minAnchorTarget, maxAnchorTarget);
        }

        public virtual void SetColor(Color color)
        {
            this.color = color;
        }

        public Coroutine TransitionColor(Color color, float speed = 1f)
        {
            this.color = color;

            if (isChangingColor)
                characterManager.StopCoroutine(co_changingColor);

            co_changingColor = characterManager.StartCoroutine(ChangingColor(displayColor, speed));

            return co_changingColor;
        }

        public virtual IEnumerator ChangingColor(Color color, float seed)
        {
            Debug.Log("Color changing is not aplicable on this character type!");
            yield return null;
        }

        public Coroutine Highlight(float speed = 1f)
        {
            if (isHighlighting)
                return co_highlighting;

            if (isUnHighlighting)
                characterManager.StopCoroutine(co_highlighting);

            highlighted = true;
            co_highlighting = characterManager.StartCoroutine(Highlighting(highlighted, speed));

            return co_highlighting;
        }

        public Coroutine UnHighlight(float speed = 1f)
        {
            if (isUnHighlighting)
                return co_highlighting;

            if (isHighlighting)
                characterManager.StopCoroutine(co_highlighting);

            highlighted = false;
            co_highlighting = characterManager.StartCoroutine(Highlighting(highlighted, speed));

            return co_highlighting;
        }

        public virtual IEnumerator Highlighting(bool highlight, float speedMultiplier)
        {
            Debug.Log("Highlighting is not available on this character type!");
            yield return null;
        }

        //Determining what kind of character
        public enum CharacterType
        {
            Text,
            Sprite,
            SpriteSheet,
        }

        //NOT OFFICIAL STUFF
        private List<string> FormatDialogue(List<string> dialogue)
        {
            List<string> formattedDialogue = new List<string>();

            foreach (string line in dialogue)
            {
                formattedDialogue.Add($"{displayName} \"{line}\"");
            }

            return formattedDialogue;
        }
        //NOT OFFICIAL STUFF
    }
}