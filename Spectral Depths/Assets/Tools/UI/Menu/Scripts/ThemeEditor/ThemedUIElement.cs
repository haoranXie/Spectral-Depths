using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlimUI.ModernMenu{
	[System.Serializable]
	public class ThemedUIElement : ThemedUI {
		[Header("Parameters")]
		Color outline;
		Image image;
		GameObject message;
		public enum OutlineStyle {solidThin, solidThick, dottedThin, dottedThick};
		public bool isImage = false;
		public bool isBackground = false;
		public bool isText = false;
		public bool isTextGUI = false;


		protected override void OnSkinUI(){
			base.OnSkinUI();

			if(isImage){
				image = GetComponent<Image>();
				image.color = themeController.currentColor;
			}
			if(isBackground){
				image = GetComponent<Image>();
				image.color = themeController.currentBackground;
			}

			message = gameObject;

			if(isText){
				message.GetComponent<TextMeshPro>().color = themeController.textColor;
			}
			if(isTextGUI){
				message.GetComponent<TextMeshProUGUI>().color = themeController.textColor;
			}
		}
	}
}