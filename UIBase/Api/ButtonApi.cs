﻿using System;
using Il2CppSystem.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace WengaPort.Api
{

    public static class QMButtonAPI
    {
    	//REPLACE THIS STRING SO YOUR MENU DOESNT COLLIDE WITH OTHER MENUS
    	public static string identifier = "WengaPort";
        public static Color mBackground = Color.red;
        public static Color mForeground = Color.white;
        public static Color bBackground = Color.red;
        public static Color bForeground = Color.yellow;
        public static List<QMSingleButton> allSingleButtons = new List<QMSingleButton>();
        public static List<QMToggleButton> allToggleButtons = new List<QMToggleButton>();
        public static List<QMNestedButton> allNestedButtons = new List<QMNestedButton>();
    }

    public class QMButtonBase
    {
        protected GameObject button;
        protected string btnQMLoc;
        protected string btnType;
        protected string btnTag;
        protected int[] initShift = { 0, 0 };
        protected Color OrigBackground;
        protected Color OrigText;

        public GameObject getGameObject()
        {
            return button;
        }

        public void setActive(bool isActive)
        {
            button.gameObject.SetActive(isActive);
        }
        public void SetParent(Transform Parent)
        {
            this.button.transform.SetParent(Parent);
        }

        public void setIntractable(bool isIntractable)
        {
            if (isIntractable)
            {
                setBackgroundColor(OrigBackground, false);
                setTextColor(OrigText, false);
            }
            else
            {
                setBackgroundColor(new Color(0.5f, 0.5f, 0.5f, 1), false);
                setTextColor(new Color(0.7f, 0.7f, 0.7f, 1), false); ;
            }
            button.gameObject.GetComponent<Button>().interactable = isIntractable;
        }

        public void setLocation(float buttonXLoc, float buttonYLoc)
        {
            button.GetComponent<RectTransform>().anchoredPosition += Vector2.right * (420 * (buttonXLoc + initShift[0]));
            button.GetComponent<RectTransform>().anchoredPosition += Vector2.down * (420 * (buttonYLoc + initShift[1]));

            btnTag = "(" + buttonXLoc + "," + buttonYLoc + ")";
            button.name = btnQMLoc + "/" + btnType + btnTag;
            button.GetComponent<Button>().name = btnType + btnTag;
        }

        public void setToolTip(string buttonToolTip)
        {
            button.GetComponent<UiTooltip>().field_Public_String_0 = buttonToolTip;
            button.GetComponent<UiTooltip>().field_Public_String_1 = buttonToolTip;
        }

        public void DestroyMe()
        {
            try
            {
                UnityEngine.Object.Destroy(button);
            }
            catch { }
        }
        public void LoadSprite(string url)
        {
            MelonLoader.MelonCoroutines.Start(LoadSprite(button.GetComponent<Image>(), url));
        }
        private static IEnumerator LoadSprite(Image Instance, string url)
        {
            while (VRCPlayer.field_Internal_Static_VRCPlayer_0 != true) yield return null;
            var Sprite = new Sprite();
            WWW www = new WWW(url);
            yield return www;
            {
                Sprite = Sprite.CreateSprite(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0), 100 * 1000, 1000, SpriteMeshType.FullRect, Vector4.zero, false);
            }
            Instance.sprite = Sprite;
            Instance.color = Color.white;
            yield break;
        }
        public void SetParent(QMNestedButton Parent)
        {
            button.transform.SetParent(QMStuff.GetQuickMenuInstance().transform.Find(Parent.getMenuName()));
        }

        public virtual void setBackgroundColor(Color buttonBackgroundColor, bool save = true) { }
        public virtual void setTextColor(Color buttonTextColor, bool save = true) { }
    }

    public class QMSingleButton : QMButtonBase
    {
        public QMSingleButton(QMNestedButton btnMenu, float btnXLocation, float btnYLocation, String btnText, System.Action btnAction, String btnToolTip, Color? btnBackgroundColor = null, Color? btnTextColor = null)
        {
            btnQMLoc = btnMenu.getMenuName();
            btnType = "SingleButton";
            initButton(btnXLocation, btnYLocation, btnText, btnAction, btnToolTip, btnBackgroundColor, btnTextColor);
        }

        public QMSingleButton(string btnMenu, float btnXLocation, float btnYLocation, String btnText, System.Action btnAction, String btnToolTip, Color? btnBackgroundColor = null, Color? btnTextColor = null)
        {
            btnQMLoc = btnMenu;
            btnType = "SingleButton";
            initButton(btnXLocation, btnYLocation, btnText, btnAction, btnToolTip, btnBackgroundColor, btnTextColor);
        }

        private void initButton(float btnXLocation, float btnYLocation, String btnText, System.Action btnAction, String btnToolTip, Color? btnBackgroundColor = null, Color? btnTextColor = null)
        {
            btnType = "SingleButton";
            button = UnityEngine.Object.Instantiate(QMStuff.SingleButtonTemplate(), QMStuff.GetQuickMenuInstance().transform.Find(btnQMLoc), true);

            initShift[0] = -1;
            initShift[1] = 0;
            setLocation(btnXLocation, btnYLocation);
            setButtonText(btnText);
            setToolTip(btnToolTip);
            setAction(btnAction);


            if (btnBackgroundColor != null)
                setBackgroundColor((Color)btnBackgroundColor);
            else
                OrigBackground = button.GetComponentInChildren<UnityEngine.UI.Image>().color;

            if (btnTextColor != null)
                setTextColor((Color)btnTextColor);
            else
                OrigText = button.GetComponentInChildren<Text>().color;

            setActive(true);
            QMButtonAPI.allSingleButtons.Add(this);
        }

        public void setButtonText(string buttonText)
        {
            button.GetComponentInChildren<Text>().text = buttonText;
        }

        public void setAction(Action buttonAction)
        {
            button.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            if (buttonAction != null)
            button.GetComponent<Button>().onClick.AddListener(DelegateSupport.ConvertDelegate<UnityAction>(buttonAction));
        }

        public override void setBackgroundColor(Color buttonBackgroundColor, bool save = true)
        {
            //button.GetComponentInChildren<UnityEngine.UI.Image>().color = buttonBackgroundColor;
            if (save)
                OrigBackground = buttonBackgroundColor;
            //UnityEngine.UI.Image[] btnBgColorList = ((btnOn.GetComponentsInChildren<UnityEngine.UI.Image>()).Concat(btnOff.GetComponentsInChildren<UnityEngine.UI.Image>()).ToArray()).Concat(button.GetComponentsInChildren<UnityEngine.UI.Image>()).ToArray();
            //foreach (UnityEngine.UI.Image btnBackground in btnBgColorList) btnBackground.color = buttonBackgroundColor;
            button.GetComponentInChildren<Button>().colors = new ColorBlock()
            {
                colorMultiplier = 1f,
                disabledColor = Color.grey,
                highlightedColor = buttonBackgroundColor * 1.5f,
                normalColor = buttonBackgroundColor / 1.5f,
                pressedColor = Color.grey * 1.5f,
            };
        }

        public override void setTextColor(Color buttonTextColor, bool save = true)
        {
            button.GetComponentInChildren<Text>().color = buttonTextColor;
            if (save)
                OrigText = buttonTextColor;
        }
    }

    public class QMToggleButton : QMButtonBase
    {
        public GameObject btnOn;
        public GameObject btnOff;
        public List<QMButtonBase> showWhenOn = new List<QMButtonBase>();
        public List<QMButtonBase> hideWhenOn = new List<QMButtonBase>();
        public bool shouldSaveInConfig = false;

        Action btnOnAction = null;
        Action btnOffAction = null;

        public QMToggleButton(QMNestedButton btnMenu, float btnXLocation, float btnYLocation, string btnTextOn, Action btnActionOn, string btnTextOff, Action btnActionOff, string btnToolTip, Color? btnBackgroundColor = null, Color? btnTextColor = null, bool shouldSaveInConfig = false, bool defaultPosition = false)
        {
            btnQMLoc = btnMenu.getMenuName();
            initButton(btnXLocation, btnYLocation, btnTextOn, btnActionOn, btnTextOff, btnActionOff, btnToolTip, btnBackgroundColor, btnTextColor, shouldSaveInConfig, defaultPosition);
        }

        public QMToggleButton(string btnMenu, int btnXLocation, int btnYLocation, string btnTextOn, Action btnActionOn, string btnTextOff, Action btnActionOff, string btnToolTip, Color? btnBackgroundColor = null, Color? btnTextColor = null, bool shouldSaveInConfig = false, bool defaultPosition = false)
        {
            btnQMLoc = btnMenu;
            initButton(btnXLocation, btnYLocation, btnTextOn, btnActionOn, btnTextOff, btnActionOff, btnToolTip, btnBackgroundColor, btnTextColor, shouldSaveInConfig, defaultPosition);
        }

        private void initButton(float btnXLocation, float btnYLocation, string btnTextOn, Action btnActionOn, string btnTextOff, Action btnActionOff, string btnToolTip, Color? btnBackgroundColor = null, Color? btnTextColor = null, bool shouldSaveInConf = false, bool defaultPosition = false)
        {
            btnType = "ToggleButton";
            button = UnityEngine.Object.Instantiate(QMStuff.ToggleButtonTemplate(), QMStuff.GetQuickMenuInstance().transform.Find(btnQMLoc), true);

            btnOn = button.transform.Find("Toggle_States_Visible/ON").gameObject;
            btnOff = button.transform.Find("Toggle_States_Visible/OFF").gameObject;

            initShift[0] = -3;
            initShift[1] = -1;
            setLocation(btnXLocation, btnYLocation);

            setOnText(btnTextOn);
            setOffText(btnTextOff);
            Text[] btnTextsOn = btnOn.GetComponentsInChildren<Text>();
            btnTextsOn[0].name = "Text_ON";
            btnTextsOn[0].resizeTextForBestFit = true;
            btnTextsOn[1].name = "Text_OFF";
            btnTextsOn[1].resizeTextForBestFit = true;
            Text[] btnTextsOff = btnOff.GetComponentsInChildren<Text>();
            btnTextsOff[0].name = "Text_ON";
            btnTextsOff[0].resizeTextForBestFit = true;
            btnTextsOff[1].name = "Text_OFF";
            btnTextsOff[1].resizeTextForBestFit = true;

            setToolTip(btnToolTip);
            //button.transform.GetComponentInChildren<UiTooltip>().SetToolTipBasedOnToggle();

            setAction(btnActionOn, btnActionOff);
            btnOn.SetActive(false);
            btnOff.SetActive(true);

            if (btnBackgroundColor != null)
                setBackgroundColor((Color)btnBackgroundColor);
            else
                OrigBackground = btnOn.GetComponentsInChildren<Text>().First().color;

            if (btnTextColor != null)
                setTextColor((Color)btnTextColor);
            else
                OrigText = btnOn.GetComponentsInChildren<Image>().First().color;

            setActive(true);
            shouldSaveInConfig = shouldSaveInConf;
            if (defaultPosition == true)// && !ButtonSettings.Contains(this))
            {
                setToggleState(true, true);
            }

            QMButtonAPI.allToggleButtons.Add(this);
            //ButtonSettings.InitToggle(this);
        }

        public override void setBackgroundColor(Color buttonBackgroundColor, bool save = true)
        {
            Image[] btnBgColorList = ((btnOn.GetComponentsInChildren<Image>()).Concat(btnOff.GetComponentsInChildren<Image>()).ToArray()).Concat(button.GetComponentsInChildren<UnityEngine.UI.Image>()).ToArray();
            foreach (Image btnBackground in btnBgColorList) btnBackground.color = buttonBackgroundColor;
            if (save)
                OrigBackground = buttonBackgroundColor;
        }

        public override void setTextColor(Color buttonTextColor, bool save = true)
        {
            Text[] btnTxtColorList = (btnOn.GetComponentsInChildren<Text>()).Concat(btnOff.GetComponentsInChildren<Text>()).ToArray();
            foreach (Text btnText in btnTxtColorList) btnText.color = buttonTextColor;
            if (save)
                OrigText = buttonTextColor;
        }

        public void setAction(Action buttonOnAction, Action buttonOffAction)
        {
            btnOnAction = buttonOnAction;
            btnOffAction = buttonOffAction;

            button.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            button.GetComponent<Button>().onClick.AddListener(UnhollowerRuntimeLib.DelegateSupport.ConvertDelegate<UnityAction>((System.Action)(() =>
          {
              if (btnOn.activeSelf)
              {
                  setToggleState(false, true);
              }
              else
              {
                  setToggleState(true, true);
              }
          })));
        }


        public void setToggleState(bool toggleOn, bool shouldInvoke = false)
        {
            btnOn.SetActive(toggleOn);
            btnOff.SetActive(!toggleOn);
            try
            {
                if (toggleOn && shouldInvoke)
                {
                    btnOnAction.Invoke();
                    showWhenOn.ForEach(x => x.setActive(true));
                    hideWhenOn.ForEach(x => x.setActive(false));
                }
                else if (!toggleOn && shouldInvoke)
                {
                    btnOffAction.Invoke();
                    showWhenOn.ForEach(x => x.setActive(false));
                    hideWhenOn.ForEach(x => x.setActive(true));
                }
            }
            catch { }

            if (shouldSaveInConfig)
            {
                //ButtonSettings.UpdateToggle(this);
            }
        }

        public string getOnText()
        {
            return btnOn.GetComponentsInChildren<Text>()[0].text;
        }

        public void setOnText(string buttonOnText)
        {
            Text[] btnTextsOn = btnOn.GetComponentsInChildren<Text>();
            btnTextsOn[0].text = buttonOnText;
            Text[] btnTextsOff = btnOff.GetComponentsInChildren<Text>();
            btnTextsOff[0].text = buttonOnText;
        }

        public void setOffText(string buttonOffText)
        {
            Text[] btnTextsOn = btnOn.GetComponentsInChildren<Text>();
            btnTextsOn[1].text = buttonOffText;
            Text[] btnTextsOff = btnOff.GetComponentsInChildren<Text>();
            btnTextsOff[1].text = buttonOffText;
        }

    }

    public class QMNestedButton
    {
        protected QMSingleButton mainButton;
        protected QMSingleButton backButton;
        protected string menuName;
        protected string btnQMLoc;
        protected string btnType;
        public Transform menu;
        public Image img;

        public QMNestedButton(QMNestedButton btnMenu, float btnXLocation, float btnYLocation, String btnText, String btnToolTip, Color? btnBackgroundColor = null, Color? btnTextColor = null, Color? backbtnBackgroundColor = null, Color? backbtnTextColor = null)
        {
            btnQMLoc = btnMenu.getMenuName();
            initButton(btnXLocation, btnYLocation, btnText, btnToolTip, btnBackgroundColor, btnTextColor, backbtnBackgroundColor, backbtnTextColor);
        }

        public QMNestedButton(string btnMenu, float btnXLocation, float btnYLocation, String btnText, String btnToolTip, Color? btnBackgroundColor = null, Color? btnTextColor = null, Color? backbtnBackgroundColor = null, Color? backbtnTextColor = null)
        {
            btnQMLoc = btnMenu;
            initButton(btnXLocation, btnYLocation, btnText, btnToolTip, btnBackgroundColor, btnTextColor, backbtnBackgroundColor, backbtnTextColor);
        }

        public void initButton(float btnXLocation, float btnYLocation, String btnText, String btnToolTip, Color? btnBackgroundColor = null, Color? btnTextColor = null, Color? backbtnBackgroundColor = null, Color? backbtnTextColor = null)
        {
            btnType = "NestedButton";

            Transform menu = UnityEngine.Object.Instantiate(QMStuff.NestedMenuTemplate(), QMStuff.GetQuickMenuInstance().transform);
            menuName = QMButtonAPI.identifier + btnQMLoc + "_" + btnXLocation + "_" + btnYLocation;
            menu.name = menuName;

            mainButton = new QMSingleButton(btnQMLoc, btnXLocation, btnYLocation, btnText, () => { QMStuff.ShowQuickmenuPage(menuName); }, btnToolTip, btnBackgroundColor, btnTextColor);

            Il2CppSystem.Collections.IEnumerator enumerator = menu.transform.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Il2CppSystem.Object obj = enumerator.Current;
                Transform btnEnum = obj.Cast<Transform>();
                if (btnEnum != null)
                {
                    UnityEngine.Object.Destroy(btnEnum.gameObject);
                }
            }

            if (backbtnTextColor == null)
            {
                backbtnTextColor = Color.yellow;
            }
            QMButtonAPI.allNestedButtons.Add(this);
            backButton = new QMSingleButton(this, 5, 2.25f, "<---", () => { QMStuff.ShowQuickmenuPage(btnQMLoc); }, "Go Back", backbtnBackgroundColor, backbtnTextColor);
            backButton.getGameObject().GetComponent<RectTransform>().sizeDelta /= new Vector2(1, 2);
        }

        public string getMenuName()
        {
            return menuName;
        }

        public QMSingleButton getMainButton()
        {
            return mainButton;
        }

        public QMSingleButton getBackButton()
        {
            return backButton;
        }

        public void DestroyMe()
        {
            mainButton.DestroyMe();
            backButton.DestroyMe();
        }
    }
    public class QMSlider
    {
        public GameObject Slider;
        public Text label;
        public Text SliderLabel;
        private GameObject labelObj;
        private bool displayState;
        private float[] initShift = { 0, 0 };

        public QMSlider(Transform parent, string name, float x, float y, Action<float> evt, float defaultValue = 0f, float MaxValue = 1f, float MinValue = 0f, bool DisplayState = true)
        {
            Slider = UnityEngine.Object.Instantiate(QMStuff.GetVRCUiMInstance().field_Public_GameObject_0.transform.Find("Screens/Settings/VolumePanel/VolumeGameWorld"), parent).gameObject;
            GameObject.Destroy(Slider.GetComponent<UiSettingConfig>());
            Slider.name = "QMSlider";
            labelObj = Slider.transform.Find("Label").gameObject;
            label = labelObj.GetComponent<Text>();
            SliderLabel = Slider.transform.Find("SliderLabel").GetComponent<Text>();
            displayState = DisplayState;
            initShift[0] = -1f;
            initShift[1] = -0.5f;
            Slider.GetComponentInChildren<RectTransform>().anchorMin += new Vector2(0.06f, 0f);
            Slider.GetComponentInChildren<RectTransform>().anchorMax += new Vector2(0.1f, 0f);
            Slider.GetComponentInChildren<Slider>().onValueChanged = new Slider.SliderEvent();
            Slider.GetComponentInChildren<Slider>().onValueChanged.AddListener(evt);
            if (DisplayState)
                Slider.GetComponentInChildren<Slider>().onValueChanged.AddListener(new Action<float>((value) =>
                {
                    SetSliderLabelText(Slider.GetComponentInChildren<Slider>().value.ToString("0.00"));
                }));

            SetRawPos(x, y);
            SetMaxValue(MaxValue);
            SetMinValue(MinValue);
            SetValue(defaultValue);
            SetTextLabel(name);
        }

        public QMSlider(QMNestedButton parent, string name, float x, float y, Action<float> evt, float defaultValue = 0f, float MaxValue = 1f, float MinValue = 0f, bool DisplayState = true)
        {
            Slider = UnityEngine.Object.Instantiate(QMStuff.GetVRCUiMInstance().field_Public_GameObject_0.transform.Find("Screens/Settings/VolumePanel/VolumeGameWorld"), QMStuff.GetQuickMenuInstance().transform.Find(parent.getMenuName())).gameObject;
            GameObject.Destroy(Slider.GetComponent<UiSettingConfig>());
            Slider.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            Slider.name = "QMSlider";
            labelObj = Slider.transform.Find("Label").gameObject;
            label = labelObj.GetComponent<Text>();
            SliderLabel = Slider.transform.Find("SliderLabel").GetComponent<Text>();
            displayState = DisplayState;
            initShift[0] = -1f;
            initShift[1] = -0.5f;
            Slider.GetComponentInChildren<RectTransform>().anchorMin += new Vector2(0.06f, 0f);
            Slider.GetComponentInChildren<RectTransform>().anchorMax += new Vector2(0.1f, 0f);
            Slider.GetComponentInChildren<Slider>().onValueChanged = new Slider.SliderEvent();
            Slider.GetComponentInChildren<Slider>().onValueChanged.AddListener(evt);
            if (DisplayState)
                Slider.GetComponentInChildren<Slider>().onValueChanged.AddListener(new Action<float>((value) =>
                {
                    SetSliderLabelText(Slider.GetComponentInChildren<Slider>().value.ToString("0.00"));
                }));

            SetMaxValue(MaxValue);
            SetMinValue(MinValue);
            SetValue(defaultValue);
            SetTextLabel(name);
            SetRawPos(x, y);
        }

        public void SetMaxValue(float MaxValue)
        {
            Slider.GetComponentInChildren<Slider>().maxValue = MaxValue;
        }

        public void SetMinValue(float MinValue)
        {
            Slider.GetComponentInChildren<Slider>().minValue = MinValue;
        }

        public void SetPos(float SliderXLoc, float SliderYLoc)
        {
            Slider.GetComponent<RectTransform>().anchoredPosition += Vector2.right * (420 * (SliderXLoc + initShift[0]));
            Slider.GetComponent<RectTransform>().anchoredPosition += Vector2.down * (420 * (SliderYLoc + initShift[1]));
        }

        public void SetRawPos(float SliderXLoc, float SliderYLoc)
        {
            Slider.GetComponent<RectTransform>().anchoredPosition = new Vector2(SliderXLoc, SliderYLoc);
        }

        public void SetValue(float Value)
        {
            Slider.GetComponentInChildren<Slider>().value = Value;
            SetSliderLabelText(Value.ToString());
        }

        public void SetTextLabel(string text)
        {
            label.text = text;
        }

        public void SetSliderLabelText(string Text)
        {
            SliderLabel.text = Text;
        }

        public void SetLocalLabelPosition(float x, float y)
        {
            labelObj.transform.position = new Vector3(x, y);
        }
    }
    public class QMStuff
    {
        // Internal cache of the BoxCollider Background for the Quick Menu
        private static BoxCollider QuickMenuBackgroundReference;

        // Internal cache of the Single Button Template for the Quick Menu
        private static GameObject SingleButtonReference;

        // Internal cache of the Toggle Button Template for the Quick Menu
        private static GameObject ToggleButtonReference;

        // Internal cache of the Nested Menu Template for the Quick Menu
        private static Transform NestedButtonReference;

        // Internal cache of the QuickMenu
        private static QuickMenu quickmenuInstance;

        // Internal cache of the VRCUiManager
        private static VRCUiManager vrcuimInstance;



        // Fetch the background from the Quick Menu
        public static BoxCollider QuickMenuBackground()
        {
            if (QuickMenuBackgroundReference == null)
                QuickMenuBackgroundReference = GetQuickMenuInstance().GetComponent<BoxCollider>();
            return QuickMenuBackgroundReference;
        }

        // Fetch the Single Button Template from the Quick Menu
        public static GameObject SingleButtonTemplate()
        {
            if (SingleButtonReference == null)
                SingleButtonReference = GetQuickMenuInstance().transform.Find("CameraMenu/Screenshot").gameObject;
            return SingleButtonReference;
        }

        // Fetch the Toggle Button Template from the Quick Menu
        public static GameObject ToggleButtonTemplate()
        {
            if (ToggleButtonReference == null)
            {
                ToggleButtonReference = GetQuickMenuInstance().transform.Find("UserInteractMenu/BlockButton").gameObject;
            }
            return ToggleButtonReference;
        }

        // Fetch the Nested Menu Template from the Quick Menu
        public static Transform NestedMenuTemplate()
        {
            if (NestedButtonReference == null)
            {
                NestedButtonReference = GetQuickMenuInstance().transform.Find("CameraMenu");
            }
            return NestedButtonReference;
        }

        // Fetch the Quick Menu instance
        public static QuickMenu GetQuickMenuInstance()
        {
            if (quickmenuInstance == null)
            {
                quickmenuInstance = QuickMenu.prop_QuickMenu_0;
            }
            return quickmenuInstance;
        }
        // Fetch the VRCUiManager instance
        public static VRCUiManager GetVRCUiMInstance()
        {
            if (vrcuimInstance == null)
            {
                vrcuimInstance = VRCUiManager.prop_VRCUiManager_0;
            }
            return vrcuimInstance;
        }

        private static FieldInfo currentPageGetter;
        private static GameObject shortcutMenu;
        private static GameObject userInteractMenu;
        public static void ShowQuickmenuPage(string pagename)
        {
            QuickMenu quickmenu = GetQuickMenuInstance();
            Transform pageTransform = quickmenu?.transform.Find(pagename);
            if (pageTransform == null)
            {
                Console.WriteLine("[QMStuff] pageTransform is null!");
            }

            if (currentPageGetter == null)
            {
                GameObject shortcutMenu = quickmenu.transform.Find("ShortcutMenu").gameObject;
                if (!shortcutMenu.activeInHierarchy)
                    shortcutMenu = quickmenu.transform.Find("UserInteractMenu").gameObject;


                FieldInfo[] fis = Il2CppType.Of<QuickMenu>().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where((fi) => fi.FieldType == Il2CppType.Of<GameObject>()).ToArray();
                //MelonLoader.MelonModLogger.Log("[QMStuff] GameObject Fields in QuickMenu:");
                int count = 0;
                foreach (FieldInfo fi in fis)
                {
                    GameObject value = fi.GetValue(quickmenu)?.TryCast<GameObject>();
                    if (value == shortcutMenu && ++count == 2)
                    {
                        //MelonLoader.MelonModLogger.Log("[QMStuff] currentPage field: " + fi.Name);
                        currentPageGetter = fi;
                        break;
                    }
                }
                if (currentPageGetter == null)
                {
                    Console.WriteLine("[QMStuff] Unable to find field currentPage in QuickMenu");
                    return;
                }
            }

            currentPageGetter.GetValue(quickmenu)?.Cast<GameObject>().SetActive(false);

            GameObject infoBar = GetQuickMenuInstance().transform.Find("QuickMenu_NewElements/_InfoBar").gameObject;
            infoBar.SetActive(pagename == "ShortcutMenu");

            QuickMenuContextualDisplay quickmenuContextualDisplay = GetQuickMenuInstance().field_Private_QuickMenuContextualDisplay_0;
            quickmenuContextualDisplay.Method_Public_Void_EnumNPublicSealedvaUnNoToUs7vUsNoUnique_0(QuickMenuContextualDisplay.EnumNPublicSealedvaUnNoToUs7vUsNoUnique.NoSelection);
            //quickmenuContextualDisplay.Method_Public_Nested0_0(QuickMenuContextualDisplay.Nested0.NoSelection);
            pageTransform.gameObject.SetActive(true);
            currentPageGetter.SetValue(quickmenu, pageTransform.gameObject);
            if (shortcutMenu == null)
                shortcutMenu = QuickMenu.prop_QuickMenu_0.transform.Find("ShortcutMenu")?.gameObject;

            if (userInteractMenu == null)
                userInteractMenu = QuickMenu.prop_QuickMenu_0.transform.Find("UserInteractMenu")?.gameObject;

            if (pagename == "ShortcutMenu")
            {
                SetIndex(0);
            }
            else if (pagename == "UserInteractMenu")
            {
                SetIndex(3);
            }
            else
            {
                SetIndex(-1);
                shortcutMenu?.SetActive(false);
                userInteractMenu?.SetActive(false);
            }
        }

        // Set the current Quick Menu index
        public static void SetIndex(int index)
        {
            GetQuickMenuInstance().field_Private_Int32_0 = index;
        }
    }

    public enum ButtonType
    {
        Default,
        Single,
        Nested,
        Toggle
    }
    public enum MenuType
    {
        ShortCut,
        UserInteract,
        Nested,
        UserInfo,
        AvatarMenu,
        SettingsMenu,
        SocialMenu,
        WorldMenu
    }
}
