using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using ImGuiNET;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace TheDarkRoles.Modules
{
    // Made by Project Echo Development
    public class HostFunctions : MonoBehaviour
    {
        public HostFunctions(IntPtr ptr) : base(ptr) { }

        private readonly string[] colorOptions = { "Red", "Blue", "Green", "Pink", "Orange", "Yellow", "Black", "White",
        "Purple", "Brown", "Cyan", "Lime", "Maroon", "Rose", "Banana", "Gray", "Tan", "Coral"};
        private string colorChanger = "Color Changer";
        private Texture2D buttonTexture;
        private Texture2D hoveredButtonTexture;
        private Texture2D boxTexture;
        private Texture2D sepTexture;
        private Texture2D labelTexture;
        private Texture2D windowTexture;
        public static bool showMenu;
        private bool _showDropdown;

        void Awake()
        {
            Logger.Info("[HostFunctions] Awake called!", "HostFunctions");
            if (buttonTexture == null)
                buttonTexture = MakeTexture(2, 2, new Color(0.16f, 0.067f, 0.13f, 0.3f));
            if (hoveredButtonTexture == null)
                hoveredButtonTexture = MakeTexture(2, 2, new Color(0.16f, 0.067f, 0.13f, 0.5f));
            if (sepTexture == null)
                sepTexture = MakeTexture(2, 2, new Color(0.16f, 0.067f, 0.13f, 0.5f));
            if (labelTexture == null)
                labelTexture = MakeTexture(2, 2, new Color(0.16f, 0.067f, 0.13f, 0.3f));
            if (windowTexture == null)
                windowTexture = MakeTexture(2, 2, new Color(0.05f, 0.025f, 0.075f, 0.8f));
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightShift))
                showMenu = !showMenu;
        }

        void OnGUI()
        {
            if (!showMenu) return;
            Action<int> value = (id) =>
            {
                GUILayout.Space(20);
                if (GUILayout.Button("Toggle Menu (RightShift)", ButtonStyle()))
                    showMenu = !showMenu;

                if (GameStates.IsMeeting || GameStates.IsCountDown || GameStates.IsInGame)
                    GUILayout.Label("Game Controls", new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.UpperCenter,
                        normal =
                        { textColor = new Color(1.0f, 0.87f, 0.96f, 0.4f) },
                        fontSize = 14
                    });

                if (GameStates.IsInGame)
                    if (GUILayout.Button("End Game", ButtonStyle()))
                    {
                        CustomWinnerHolder.ResetAndSetWinner(CustomWinner.Draw);
                        GameManager.Instance.LogicFlow.CheckEndCriteria();
                    }

                if (GameStates.IsMeeting && GameStates.IsInGame)
                    if (GUILayout.Button("End Meeting", ButtonStyle()))
                        MeetingHud.Instance.RpcClose();

                if (GameStates.IsCountDown)
                    if (GUILayout.Button("Skip Countdown", ButtonStyle()))
                    {
                        Logger.Info("CountDownTimer set to 0", "KeyCommand");
                        GameStartManager.Instance.countDownTimer = 0;
                    }

                if (GameStates.IsLobby)
                {
                    GUILayout.Label("Host Color Changer", new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.UpperCenter,
                        normal =
                        { textColor = new Color(1.0f, 0.87f, 0.96f, 0.4f)},
                        fontSize = 14
                    });

                    if (GUILayout.Button(colorChanger, ButtonStyle())) _showDropdown = !_showDropdown;

                    if (_showDropdown)
                    {
                        //GUI.Box(new Rect(4, 100, 292, 1), "", new GUIStyle()
                        //{
                        //    normal = { background = sepTexture }
                        //});
                        GUILayout.Space(7);
                        for (var i = 0; i < colorOptions.Length; i++)
                        {
                            if (GUILayout.Button(colorOptions[i], ButtonStyle()))
                            {
                                PlayerControl.LocalPlayer.RpcSetColor(Utils.MsgToColor(colorOptions[i], true));
                                _showDropdown = false;
                            }
                        }
                    }
                }
            };
            GUILayout.Window(0, new Rect(10, 10, 300, 200), value, "Project Echo: Host Functions", new GUIStyle()
            {
                normal = { background = windowTexture, textColor = new Color(1.0f, 0.87f, 0.96f, 0.4f) },
                fontSize = 16,
                alignment = TextAnchor.UpperCenter,
                padding = { left = 0, right = 0, top = 4, bottom = 4 },
            });
        }
        void OnDestroy()
        {
            if (buttonTexture != null)
                Destroy(buttonTexture);
            if (boxTexture != null)
                Destroy(boxTexture);
            if (sepTexture != null)
                Destroy(sepTexture);
            if (hoveredButtonTexture != null)
                Destroy(hoveredButtonTexture);
            if (labelTexture != null)
                Destroy(labelTexture);

        }

        private unsafe void RenderImGui()
        {
            try
            {
                var drawData = ImGui.GetDrawData();
                GL.IssuePluginEvent((IntPtr)drawData.NativePtr, 0);
            }
            catch (Exception e)
            {
                Logger.Error($"Render error: {e}", "HostFunctions");
            }
        }

        private GUIStyle ButtonStyle()
        {
            var style = new GUIStyle(GUI.skin.button);
            style.normal.background = buttonTexture ?? Texture2D.whiteTexture;
            style.hover.background = hoveredButtonTexture ?? Texture2D.whiteTexture;
            style.normal.textColor = new Color(1.0f, 0.87f, 0.96f, 0.4f);
            style.hover.textColor = new Color(1.0f, 0.87f, 0.96f, 0.6f);
            style.active.background = buttonTexture;
            style.active.textColor = new Color(1.0f, 0.87f, 0.96f, 0.4f);
            style.alignment = TextAnchor.MiddleCenter;
            style.border = new RectOffset { left = 2, right = 2, top = 2, bottom = 2 };
            return style;
        }

        private Texture2D MakeTexture(int width, int height, Color color)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = color;

            Texture2D result = new(width, height);
            result.SetPixels(pix);
            result.Apply();
            result.hideFlags = HideFlags.HideAndDontSave;
            return result;
        }
    }
}