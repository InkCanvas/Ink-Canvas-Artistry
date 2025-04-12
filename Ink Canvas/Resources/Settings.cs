using Newtonsoft.Json;

namespace Ink_Canvas
{
    public class Settings
    {
        [JsonProperty("advanced")]
        public Advanced Advanced { get; set; } = new Advanced();
        [JsonProperty("appearance")]
        public Appearance Appearance { get; set; } = new Appearance();
        [JsonProperty("automation")]
        public Automation Automation { get; set; } = new Automation();
        [JsonProperty("behavior")]
        public PowerPointSettings PowerPointSettings { get; set; } = new PowerPointSettings();
        [JsonProperty("canvas")]
        public Canvas Canvas { get; set; } = new Canvas();
        [JsonProperty("gesture")]
        public Gesture Gesture { get; set; } = new Gesture();
        [JsonProperty("inkToShape")]
        public InkToShape InkToShape { get; set; } = new InkToShape();
        [JsonProperty("startup")]
        public Startup Startup { get; set; } = new Startup();
        [JsonProperty("randSettings")]
        public RandSettings RandSettings { get; set; } = new RandSettings();
    }

    public class Canvas
    {
        [JsonProperty("inkWidth")]
        public double InkWidth { get; set; } = 2.5;
        [JsonProperty("inkAlpha")]
        public int InkAlpha { get; set; } = 140;
        [JsonProperty("isCompressPicturesUploaded")]
        public bool IsCompressPicturesUploaded { get; set; } = false;
        [JsonProperty("isShowCursor")]
        public bool IsShowCursor { get; set; } = false;
        [JsonProperty("inkStyle")]
        public int InkStyle { get; set; } = 0;
        [JsonProperty("eraserSize")]
        public int EraserSize { get; set; } = 2;
        [JsonProperty("eraserType")] 
        public int EraserType { get; set; } = 0; // 0 - 图标切换模式      1 - 面积擦     2 - 线条擦
        [JsonProperty("hideStrokeWhenSelecting")]
        public bool HideStrokeWhenSelecting { get; set; } = true;

        [JsonProperty("usingWhiteboard")]
        public bool UsingWhiteboard { get; set; }

        [JsonProperty("hyperbolaAsymptoteOption")]
        public OptionalOperation HyperbolaAsymptoteOption { get; set; } = OptionalOperation.Ask;
    }

    public enum OptionalOperation
    {
        Yes,
        No,
        Ask
    }

    public class Gesture
    {
        [JsonIgnore]
        public bool IsEnableTwoFingerGesture => IsEnableTwoFingerZoom || IsEnableTwoFingerTranslate || IsEnableTwoFingerRotation;
        [JsonIgnore]
        public bool IsEnableTwoFingerGestureTranslateOrRotation => IsEnableTwoFingerTranslate || IsEnableTwoFingerRotation;
        [JsonProperty("isEnableMultiTouchMode")]
        public bool IsEnableMultiTouchMode { get; set; } = true;
        [JsonProperty("isEnableTwoFingerZoom")]
        public bool IsEnableTwoFingerZoom { get; set; } = true;
        [JsonProperty("isEnableTwoFingerTranslate")]
        public bool IsEnableTwoFingerTranslate { get; set; } = true;
        [JsonProperty("AutoSwitchTwoFingerGesture")]
        public bool AutoSwitchTwoFingerGesture { get; set; } = true;
        [JsonProperty("isEnableTwoFingerRotation")]
        public bool IsEnableTwoFingerRotation { get; set; } = false;
        [JsonProperty("isEnableTwoFingerRotationOnSelection")]
        public bool IsEnableTwoFingerRotationOnSelection { get; set; } = false;

        [JsonProperty("matrixTransformCenterPoint")]
        public MatrixTransformCenterPointOptions MatrixTransformCenterPoint { get; set; } = MatrixTransformCenterPointOptions.CanvasCenterPoint;
    }

    public enum MatrixTransformCenterPointOptions
    {
        CanvasCenterPoint,
        SelectedElementsCenterPoint,
        GestureOperationCenterPoint
    }

    public class Startup
    {
        [JsonProperty("isAutoUpdate")]
        public bool IsAutoUpdate { get; set; } = true;
        [JsonProperty("isAutoUpdateWithProxy")]
        public bool IsAutoUpdateWithProxy { get; set; } = false;
        [JsonProperty("autoUpdateProxy")]
        public string AutoUpdateProxy { get; set; } = "https://mirror.ghproxy.com/";
        [JsonProperty("isAutoUpdateWithSilence")]
        public bool IsAutoUpdateWithSilence { get; set; } = false;
        [JsonProperty("isAutoUpdateWithSilenceStartTime")]
        public string AutoUpdateWithSilenceStartTime { get; set; } = "00:00";
        [JsonProperty("isAutoUpdateWithSilenceEndTime")]
        public string AutoUpdateWithSilenceEndTime { get; set; } = "00:00";

        [JsonProperty("isEnableNibMode")]
        public bool IsEnableNibMode { get; set; } = false;
        /*
        [JsonProperty("isAutoHideCanvas")]
        public bool IsAutoHideCanvas { get; set; } = true;
        [JsonProperty("isAutoEnterModeFinger")]
        public bool IsAutoEnterModeFinger { get; set; } = false;*/
        [JsonProperty("isFoldAtStartup")]
        public bool IsFoldAtStartup { get; set; } = false;
    }

    public class Appearance
    {
        [JsonProperty("isEnableDisPlayFloatBarText")]
        public bool IsEnableDisPlayFloatBarText { get; set; } = false;

        [JsonProperty("isEnableDisPlayNibModeToggler")]
        public bool IsEnableDisPlayNibModeToggler { get; set; } = true;

        [JsonProperty("isColorfulViewboxFloatingBar")]
        public bool IsColorfulViewboxFloatingBar { get; set; } = false;

        [JsonProperty("enableViewboxFloatingBarScaleTransform")]
        public double FloatingBarScale { get; set; } = 100.0; // Default to 100%

        [JsonProperty("enableViewboxBlackBoardScaleTransform")]
        public double BlackboardScale { get; set; } = 100.0; // Default to 100%

        [JsonProperty("isTransparentButtonBackground")]
        public bool IsTransparentButtonBackground { get; set; } = true;

        [JsonProperty("isShowExitButton")]
        public bool IsShowExitButton { get; set; } = true;

        [JsonProperty("isShowEraserButton")]
        public bool IsShowEraserButton { get; set; } = true;

        [JsonProperty("isShowHideControlButton")]
        public bool IsShowHideControlButton { get; set; } = false;

        [JsonProperty("isShowLRSwitchButton")]
        public bool IsShowLRSwitchButton { get; set; } = false;

        [JsonProperty("isShowModeFingerToggleSwitch")]
        public bool IsShowModeFingerToggleSwitch { get; set; } = true;
        [JsonProperty("theme")]
        public int Theme { get; set; } = 0;            
    }

    public class PowerPointSettings
    {
        [JsonProperty("isShowPPTNavigationBottom")]
        public bool IsShowPPTNavigationBottom { get; set; } = true;
        [JsonProperty("isShowPPTNavigationSides")]
        public bool IsShowPPTNavigationSides { get; set; } = true;
        [JsonProperty("isShowBottomPPTNavigationPanel")]
        public bool IsShowBottomPPTNavigationPanel { get; set; } = true;
        [JsonProperty("isShowSidePPTNavigationPanel")]
        public bool IsShowSidePPTNavigationPanel { get; set; } = true;
        [JsonProperty("powerPointSupport")]
        public bool PowerPointSupport { get; set; } = true;
        [JsonProperty("isShowCanvasAtNewSlideShow")]
        public bool IsShowCanvasAtNewSlideShow { get; set; } = true;
        [JsonProperty("isNoClearStrokeOnSelectWhenInPowerPoint")]
        public bool IsNoClearStrokeOnSelectWhenInPowerPoint { get; set; } = true;
        [JsonProperty("isShowStrokeOnSelectInPowerPoint")]
        public bool IsShowStrokeOnSelectInPowerPoint { get; set; } = false;
        [JsonProperty("isAutoSaveStrokesInPowerPoint")]
        public bool IsAutoSaveStrokesInPowerPoint { get; set; } = true;
        [JsonProperty("isAutoSaveScreenShotInPowerPoint")]
        public bool IsAutoSaveScreenShotInPowerPoint { get; set; } = false;
        [JsonProperty("isNotifyPreviousPage")]
        public bool IsNotifyPreviousPage { get; set; } = false;
        [JsonProperty("isNotifyHiddenPage")]
        public bool IsNotifyHiddenPage { get; set; } = true;
        [JsonProperty("isNotifyAutoPlayPresentation")]
        public bool IsNotifyAutoPlayPresentation { get; set; } = true;
        [JsonProperty("isEnableTwoFingerGestureInPresentationMode")]
        public bool IsEnableTwoFingerGestureInPresentationMode { get; set; } = false;
        [JsonProperty("isEnableFingerGestureSlideShowControl")]
        public bool IsEnableFingerGestureSlideShowControl { get; set; } = true;
        [JsonProperty("isSupportWPS")]
        public bool IsSupportWPS { get; set; } = true;
    }

    public class Automation
    {
        [JsonIgnore]
        public bool IsEnableAutoFold => 
            IsAutoFoldInEasiNote
            || IsAutoFoldInEasiCamera
            || IsAutoFoldInEasiNote3C
            || IsAutoFoldInSeewoPincoTeacher
            || IsAutoFoldInHiteTouchPro
            || IsAutoFoldInHiteCamera
            || IsAutoFoldInWxBoardMain
            || IsAutoFoldInOldZyBoard
            || IsAutoFoldInPPTSlideShow
            || IsAutoFoldInMSWhiteboard;

        [JsonProperty("isAutoFoldInEasiNote")]
        public bool IsAutoFoldInEasiNote { get; set; } = false;

        [JsonProperty("isAutoFoldInEasiNoteIgnoreDesktopAnno")]
        public bool IsAutoFoldInEasiNoteIgnoreDesktopAnno { get; set; } = false;

        [JsonProperty("isAutoFoldInEasiCamera")]
        public bool IsAutoFoldInEasiCamera { get; set; } = false;

        [JsonProperty("isAutoFoldInEasiNote3C")]
        public bool IsAutoFoldInEasiNote3C { get; set; } = false;

        [JsonProperty("isAutoFoldInSeewoPincoTeacher")]
        public bool IsAutoFoldInSeewoPincoTeacher { get; set; } = false;

        [JsonProperty("isAutoFoldInHiteTouchPro")]
        public bool IsAutoFoldInHiteTouchPro { get; set; } = false;

        [JsonProperty("isAutoFoldInHiteCamera")]
        public bool IsAutoFoldInHiteCamera { get; set; } = false;

        [JsonProperty("isAutoFoldInWxBoardMain")]
        public bool IsAutoFoldInWxBoardMain { get; set; } = false;
        /*
        [JsonProperty("isAutoFoldInZySmartBoard")]
        public bool IsAutoFoldInZySmartBoard { get; set; } = false;
        */
        [JsonProperty("isAutoFoldInOldZyBoard")]
        public bool IsAutoFoldInOldZyBoard { get; set; } = false;

        [JsonProperty("isAutoFoldInMSWhiteboard")]
        public bool IsAutoFoldInMSWhiteboard { get; set; } = false;

        [JsonProperty("isAutoFoldInPPTSlideShow")]
        public bool IsAutoFoldInPPTSlideShow { get; set; } = false;

        [JsonProperty("isAutoKillPptService")]
        public bool IsAutoKillPptService { get; set; } = false;

        [JsonProperty("isAutoKillEasiNote")]
        public bool IsAutoKillEasiNote { get; set; } = false;

        [JsonProperty("isSaveScreenshotsInDateFolders")]
        public bool IsSaveScreenshotsInDateFolders { get; set; } = false;

        [JsonProperty("isAutoSaveStrokesAtScreenshot")]
        public bool IsAutoSaveStrokesAtScreenshot { get; set; } = false;

        [JsonProperty("isAutoSaveStrokesAtClear")]
        public bool IsAutoSaveStrokesAtClear { get; set; } = false;

        [JsonProperty("isAutoClearWhenExitingWritingMode")]
        public bool IsAutoClearWhenExitingWritingMode { get; set; } = false;

        [JsonProperty("minimumAutomationStrokeNumber")]
        public int MinimumAutomationStrokeNumber { get; set; } = 0;

        [JsonProperty("autoSavedStrokesLocation")]
        public string AutoSavedStrokesLocation = @"D:\Ink Canvas";

        [JsonProperty("autoDelSavedFiles")]
        public bool AutoDelSavedFiles = false;

        [JsonProperty("autoDelSavedFilesDaysThreshold")]
        public int AutoDelSavedFilesDaysThreshold = 15;
    }

    public class Advanced
    {
        [JsonProperty("isSpecialScreen")]
        public bool IsSpecialScreen { get; set; } = false;

        [JsonProperty("isQuadIR")]
        public bool IsQuadIR { get; set; } = false;

        [JsonProperty("touchMultiplier")]
        public double TouchMultiplier { get; set; } = 0.25;

        [JsonProperty("nibModeBoundsWidth")]
        public int NibModeBoundsWidth { get; set; } = 10;

        [JsonProperty("fingerModeBoundsWidth")]
        public int FingerModeBoundsWidth { get; set; } = 30;

        [JsonProperty("nibModeBoundsWidthThresholdValue")]
        public double NibModeBoundsWidthThresholdValue { get; set; } = 2.5;

        [JsonProperty("fingerModeBoundsWidthThresholdValue")]
        public double FingerModeBoundsWidthThresholdValue { get; set; } = 2.5;

        [JsonProperty("nibModeBoundsWidthEraserSize")]
        public double NibModeBoundsWidthEraserSize { get; set; } = 0.8;

        [JsonProperty("fingerModeBoundsWidthEraserSize")]
        public double FingerModeBoundsWidthEraserSize { get; set; } = 0.8;

        [JsonProperty("isEnableEdgeGestureUtil")]
        public bool IsEnableEdgeGestureUtil { get; set; } = false;

        [JsonProperty("isLogEnabled")]
        public bool IsLogEnabled { get; set; } = true;

        [JsonProperty("isSecondConfimeWhenShutdownApp")]
        public bool IsSecondConfimeWhenShutdownApp { get; set; } = false;
    }

    public class InkToShape
    {
        [JsonProperty("isInkToShapeEnabled")]
        public bool IsInkToShapeEnabled { get; set; } = true;
    }

    public class RandSettings {
        [JsonProperty("peopleCount")]
        public int PeopleCount { get; set; } = 60;
        [JsonProperty("isNotRepeatName")]
        public bool IsNotRepeatName { get; set; } = false;
    }
}