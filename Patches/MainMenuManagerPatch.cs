using System;
using System.Linq;
using HarmonyLib;
using TheDarkRoles.Templates;
using UnityEngine;

namespace TheDarkRoles
{
    [HarmonyPatch(typeof(MainMenuManager))]
    public class MainMenuManagerPatch
    {
        private static SimpleButton discordButton;
        public static SimpleButton UpdateButton { get; private set; }
        private static SimpleButton gitHubButton;
        private static PassiveButton template;

        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate)), HarmonyPostfix]
        public static void Postfix(MainMenuManager __instance)
        {
            if (__instance == null) return;
            __instance.playButton.transform.gameObject.SetActive(Options.IsLoaded);
        }

        [HarmonyPatch(nameof(MainMenuManager.Start)), HarmonyPostfix, HarmonyPriority(Priority.Normal)]
        public static void StartPostfix(MainMenuManager __instance)
        {
            if (template == null) template = __instance.quitButton;

            // FPS
            Application.targetFrameRate = 165;

            if (template == null) return;

            var howToPlayButton = __instance.howToPlayButton;
            var freeplayButton = howToPlayButton.transform.parent.Find("FreePlayButton");

            if (freeplayButton != null) freeplayButton.gameObject.SetActive(false);

            howToPlayButton.transform.SetLocalX(0);

        }

        /// <summary>TOHロゴの子としてボタンを生成</summary>
        /// <param name="name">オブジェクト名</param>
        /// <param name="normalColor">普段のボタンの色</param>
        /// <param name="hoverColor">マウスが乗っているときのボタンの色</param>
        /// <param name="action">押したときに発火するアクション</param>
        /// <param name="label">ボタンのテキスト</param>
        /// <param name="scale">ボタンのサイズ 変更しないなら不要</param>
        private static SimpleButton CreateButton(
            string name,
            Vector3 localPosition,
            Color32 normalColor,
            Color32 hoverColor,
            Action action,
            string label,
            Vector2? scale = null,
            bool isActive = true)
        {
            var button = new SimpleButton(CredentialsPatch.TohLogo.transform, name, localPosition, normalColor, hoverColor, action, label, isActive);
            if (scale.HasValue)
            {
                button.Scale = scale.Value;
            }
            return button;
        }

        // プレイメニュー，アカウントメニュー，クレジット画面が開かれたらロゴとボタンを消す
        [HarmonyPatch(nameof(MainMenuManager.OpenGameModeMenu))]
        [HarmonyPatch(nameof(MainMenuManager.OpenOnlineMenu))]
        [HarmonyPatch(nameof(MainMenuManager.OpenEnterCodeMenu))]
        [HarmonyPatch(nameof(MainMenuManager.ClickBackOnline))]
        [HarmonyPatch(nameof(MainMenuManager.OpenAccountMenu))]
        [HarmonyPatch(nameof(MainMenuManager.OpenCredits))]
        [HarmonyPostfix]
        public static void OpenMenuPostfix()
        {
            if (CredentialsPatch.TohLogo != null)
            {
                CredentialsPatch.TohLogo.gameObject.SetActive(false);
            }
        }
        [HarmonyPatch(nameof(MainMenuManager.ResetScreen)), HarmonyPostfix]
        public static void ResetScreenPostfix()
        {
            if (CredentialsPatch.TohLogo != null)
            {
                CredentialsPatch.TohLogo.gameObject.SetActive(true);
            }
        }
        [HarmonyPatch(nameof(MainMenuManager.OpenOnlineMenu)), HarmonyPostfix]
        public static void FindGameButtonDisable(MainMenuManager __instance)
        {
            __instance.findGameButton.gameObject.SetActive(false);
            //中央Lineを消す
            var line = __instance.findGameButton.gameObject.transform.parent.Find("Line");
            if (line != null)
            {
                line.gameObject.SetActive(false);
            }
            //ロビー作成、コード入力ボタンを中央に移動
            __instance.createGameButton.transform.SetLocalX(0);
            var enterCodeButton = __instance.createGameButton.transform.parent.Find("Enter Code Button");
            if (enterCodeButton != null)
            {
                enterCodeButton.gameObject.transform.SetLocalX(0);
            }
        }

        public static void DisableMainMenuPanels(MainMenuManager __instance)
        {
            GameObject amongUsLogo = GameObject.Find("LOGO-AU");
            amongUsLogo.SetActive(false);

            __instance.screenTint.gameObject.transform.localPosition += new Vector3(1000f, 0f);
            __instance.screenTint.gameObject.SetActive(false);

            __instance.rightPanelMask.SetActive(true);
            __instance.mainMenuUI.gameObject.transform.position += new Vector3(-0.2f, 0f);
            Transform[] children = __instance.mainMenuUI.GetComponentsInChildren<Transform>();

            foreach (var child in children)
            {
                if (child.name == "BackgroundTexture") child.gameObject.SetActive(false);
                if (child.name == "WindowShine") child.gameObject.SetActive(false);
                if (child.name == "ScreenCover") child.gameObject.SetActive(false);

                if (child.name == "LeftPanel")
                {
                    GameObject leftPanel = child.gameObject;
                    leftPanel.GetComponent<SpriteRenderer>().enabled = false;
                    SpriteRenderer[] leftPanelChildren = leftPanel.GetComponentsInChildren<SpriteRenderer>();
                    foreach (var left_child in leftPanelChildren)
                    {
                        if (left_child.name == "Divider") left_child.enabled = false;
                    }

                    leftPanel.GetComponentsInChildren<SpriteRenderer>(true).Where(r => r.name == "Shine").ToList().ForEach(r => r.enabled = false);
                }

                if (child.name == "RightPanel")
                {
                    GameObject rightPanel = child.gameObject;
                    rightPanel.GetComponent<SpriteRenderer>().enabled = false;

                    Transform[] rightPanelChildren = rightPanel.GetComponentsInChildren<Transform>();
                    foreach (var right_child in rightPanelChildren)
                    {
                        if (right_child.name == "MaskedBlackScreen")
                        {
                            GameObject maskedBlackScreen = right_child.gameObject;
                            maskedBlackScreen.GetComponent<SpriteRenderer>().enabled = false;
                            maskedBlackScreen.transform.localScale = new Vector3(7.35f, 4.5f, 4f);
                        }
                    }
                }
            }
        }

        public static void ColourMainMenuButtons(MainMenuManager __instance)
        {
            __instance.playButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(.7f, .1f, .7f);
            __instance.playButton.activeSprites.GetComponent<SpriteRenderer>().color = new Color(.5f, .1f, .5f);
            Color originalColorPlayButton = __instance.playButton.inactiveSprites.GetComponent<SpriteRenderer>().color;
            Color originalColorPlayButton2 = __instance.playButton.activeSprites.GetComponent<SpriteRenderer>().color;
            __instance.playButton.inactiveSprites.GetComponent<SpriteRenderer>().color = originalColorPlayButton * 0.5f;
            __instance.playButton.activeSprites.GetComponent<SpriteRenderer>().color = originalColorPlayButton2 * 0.5f;
            __instance.playButton.activeTextColor = Color.white;
            __instance.playButton.inactiveTextColor = Color.white;

            __instance.inventoryButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(.7f, .1f, .7f);
            __instance.inventoryButton.activeSprites.GetComponent<SpriteRenderer>().color = new Color(.5f, .1f, .5f);
            Color originalColorInventoryButton = __instance.inventoryButton.inactiveSprites.GetComponent<SpriteRenderer>().color;
            Color originalColorInventoryButton2 = __instance.inventoryButton.activeSprites.GetComponent<SpriteRenderer>().color;
            __instance.inventoryButton.inactiveSprites.GetComponent<SpriteRenderer>().color = originalColorInventoryButton * 0.5f;
            __instance.inventoryButton.activeSprites.GetComponent<SpriteRenderer>().color = originalColorInventoryButton2 * 0.5f;
            __instance.inventoryButton.activeTextColor = Color.white;
            __instance.inventoryButton.inactiveTextColor = Color.white;

            __instance.shopButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(.7f, .1f, .7f);
            __instance.shopButton.activeSprites.GetComponent<SpriteRenderer>().color = new Color(.5f, .1f, .5f);
            Color originalColorShopButton = __instance.shopButton.inactiveSprites.GetComponent<SpriteRenderer>().color;
            Color originalColorShopButton2 = __instance.shopButton.activeSprites.GetComponent<SpriteRenderer>().color;
            __instance.shopButton.inactiveSprites.GetComponent<SpriteRenderer>().color = originalColorShopButton * 0.5f;
            __instance.shopButton.activeSprites.GetComponent<SpriteRenderer>().color = originalColorShopButton2 * 0.5f;
            __instance.shopButton.activeTextColor = Color.white;
            __instance.shopButton.inactiveTextColor = Color.white;

            __instance.newsButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(.7f, .1f, .7f);
            __instance.newsButton.activeSprites.GetComponent<SpriteRenderer>().color = new Color(.5f, .1f, .5f);
            Color originalColorNewsButton = __instance.newsButton.inactiveSprites.GetComponent<SpriteRenderer>().color;
            Color originalColorNewsButton2 = __instance.newsButton.activeSprites.GetComponent<SpriteRenderer>().color;
            __instance.newsButton.inactiveSprites.GetComponent<SpriteRenderer>().color = originalColorNewsButton * 0.5f;
            __instance.newsButton.activeSprites.GetComponent<SpriteRenderer>().color = originalColorNewsButton2 * 0.5f;
            __instance.newsButton.activeTextColor = Color.white;
            __instance.newsButton.inactiveTextColor = Color.white;

            __instance.myAccountButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(.7f, .1f, .7f);
            __instance.myAccountButton.activeSprites.GetComponent<SpriteRenderer>().color = new Color(.5f, .1f, .5f);
            Color originalColorMyAccount = __instance.myAccountButton.inactiveSprites.GetComponent<SpriteRenderer>().color;
            Color originalColorMyAccount2 = __instance.myAccountButton.activeSprites.GetComponent<SpriteRenderer>().color;
            __instance.myAccountButton.inactiveSprites.GetComponent<SpriteRenderer>().color = originalColorMyAccount * 0.5f;
            __instance.myAccountButton.activeSprites.GetComponent<SpriteRenderer>().color = originalColorMyAccount2 * 0.5f;
            __instance.myAccountButton.activeTextColor = Color.white;
            __instance.myAccountButton.inactiveTextColor = Color.white;
            __instance.accountButtons.transform.position += new Vector3(0f, 0f, -1f);

            __instance.settingsButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(.7f, .1f, .7f);
            __instance.settingsButton.activeSprites.GetComponent<SpriteRenderer>().color = new Color(.5f, .1f, .5f);
            Color originalColorSettingsButton = __instance.settingsButton.inactiveSprites.GetComponent<SpriteRenderer>().color;
            Color originalColorSettingsButton2 = __instance.settingsButton.activeSprites.GetComponent<SpriteRenderer>().color;
            __instance.settingsButton.inactiveSprites.GetComponent<SpriteRenderer>().color = originalColorSettingsButton * 0.5f;
            __instance.settingsButton.activeSprites.GetComponent<SpriteRenderer>().color = originalColorSettingsButton2 * 0.5f;
            __instance.settingsButton.activeTextColor = Color.white;
            __instance.settingsButton.inactiveTextColor = Color.white;

            __instance.quitButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(.7f, .1f, .7f);
            __instance.quitButton.activeSprites.GetComponent<SpriteRenderer>().color = new Color(.5f, .1f, .5f);
            Color originalColorQuitButton = __instance.quitButton.inactiveSprites.GetComponent<SpriteRenderer>().color;
            Color originalColorQuitButton2 = __instance.quitButton.activeSprites.GetComponent<SpriteRenderer>().color;
            __instance.quitButton.inactiveSprites.GetComponent<SpriteRenderer>().color = originalColorQuitButton * 0.5f;
            __instance.quitButton.activeSprites.GetComponent<SpriteRenderer>().color = originalColorQuitButton2 * 0.5f;
            __instance.quitButton.activeTextColor = Color.white;
            __instance.quitButton.inactiveTextColor = Color.white;

            __instance.creditsButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(.7f, .1f, .7f);
            __instance.creditsButton.activeSprites.GetComponent<SpriteRenderer>().color = new Color(.5f, .1f, .5f);
            Color originalColorCreditsButton = __instance.creditsButton.inactiveSprites.GetComponent<SpriteRenderer>().color;
            Color originalColorCreditsButton2 = __instance.creditsButton.activeSprites.GetComponent<SpriteRenderer>().color;
            __instance.creditsButton.inactiveSprites.GetComponent<SpriteRenderer>().color = originalColorCreditsButton * 0.5f;
            __instance.creditsButton.activeSprites.GetComponent<SpriteRenderer>().color = originalColorCreditsButton2 * 0.5f;
            __instance.creditsButton.activeTextColor = Color.white;
            __instance.creditsButton.inactiveTextColor = Color.white;
            __instance.creditsButton.inactiveTextColor = Color.white;
        }
    }
}
