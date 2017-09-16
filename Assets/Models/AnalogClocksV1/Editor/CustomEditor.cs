using UnityEditor;
using UnityEngine;

[UnityEditor.CustomEditor(typeof(Clock))]
[CanEditMultipleObjects]

public class CustomEditor : Editor
{
    protected static bool showTimerSettings = true;
    protected static int toolbarInt = 0;
    static string[] toolbarStrings;
    //Bools
    SerializedProperty PlayPointerSound, SystemDate, AlarmActive;
    //Transforms
    SerializedProperty SecPointer, MinPointer, HourPointer, AlarmPointer, Hammer;
    //AudioSources  
    SerializedProperty AlarmAudio;
    //Floats
    SerializedProperty Speed;
    //Ints
    SerializedProperty NewYear, NewMonth, NewDay, NewHour, NewMinute, NewSecond, AlarmHour, AlarmMin;
    //Color
    SerializedProperty NewColor;
    //
    bool SysClock = false;
    string strFormat = "R";
    //   
    Clock Instance;
    string PrefabName;   


    void OnEnable()
    {
        Instance = (Clock)target;
        PrefabName = PrefabUtility.GetPrefabParent(Instance.gameObject).name;
        Instance.ClockType = PrefabName;
        switch (PrefabName)
        {
            case "Clock1":
                toolbarStrings = new string[] { "Timer Settings", "Objects" };
                break;
            case "Clock2":
                toolbarStrings = new string[] { "Timer Settings", "Objects" };
                break;
            case "Clock3":
                toolbarStrings = new string[] { "Timer Settings", "Alarm Settings", "Objects" };
                break;
        }
        
        PlayPointerSound = serializedObject.FindProperty("PlayPointerSound");
        SecPointer = serializedObject.FindProperty("SecPointer");
        MinPointer = serializedObject.FindProperty("MinPointer");
        HourPointer = serializedObject.FindProperty("HourPointer");
        if (PrefabName == "Clock3")
        {
            AlarmPointer = serializedObject.FindProperty("AlarmPointer");
            Hammer = serializedObject.FindProperty("Hammer");
            AlarmAudio = serializedObject.FindProperty("AlarmAudio");
        }
        Speed = serializedObject.FindProperty("speed");
        //Bools
        SystemDate = serializedObject.FindProperty("SystemDate");
        AlarmActive = serializedObject.FindProperty("AlarmActive");
        //Ints
        NewYear = serializedObject.FindProperty("NewYear");
        NewMonth    = serializedObject.FindProperty("NewMonth");
        NewDay = serializedObject.FindProperty("NewDay"); ;
        NewHour = serializedObject.FindProperty("NewHour"); ;
        NewMinute = serializedObject.FindProperty("NewMinute");
        NewSecond = serializedObject.FindProperty("NewSecond");
        AlarmHour = serializedObject.FindProperty("AlarmHour");
        AlarmMin = serializedObject.FindProperty("AlarmMin");
        //
        NewColor = serializedObject.FindProperty("NewColor");
    }


    public override void OnInspectorGUI()
    {
        toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);
        switch (toolbarInt)
        {
            case 0:
                GUILayout.Label("Timer Properties", EditorStyles.boldLabel);
                GUILayout.BeginVertical("Box");
                EditorGUILayout.PropertyField(PlayPointerSound, new GUIContent("Play Pointer Sound"));
                GUILayout.EndVertical();
                GUILayout.BeginVertical("Box");
                EditorGUILayout.PropertyField(SystemDate, new GUIContent("System Date"));
                SysClock = Instance.SystemDate;
                GUILayout.EndVertical();
                if (!SysClock)
                {
                    GUILayout.BeginVertical("Box");
                    EditorGUILayout.IntSlider(NewYear, 1,50000,new GUIContent("New Year"));
                    EditorGUILayout.IntSlider(NewMonth, 1, 12, new GUIContent("New Month"));
                    EditorGUILayout.IntSlider(NewDay, 1, 31, new GUIContent("New Day"));
                    EditorGUILayout.IntSlider(NewHour, 0, 23, new GUIContent("New Hour"));
                    EditorGUILayout.IntSlider(NewMinute, 0, 59, new GUIContent("New Minute"));
                    EditorGUILayout.IntSlider(NewSecond, 0, 59, new GUIContent("New Second"));

                    EditorGUILayout.Slider(Speed,1,100, new GUIContent("Speed"));
                    if (GUILayout.Button("Set a new Time"))
                    {
                       Instance.CustomClock = new System.DateTime(Instance.NewYear,Instance.NewMonth, Instance.NewDay, Instance.NewHour, Instance.NewMinute, Instance.NewSecond);
                    }
                    GUILayout.EndVertical();
                }
                if (PrefabName == "Clock3")
                {
                    GUILayout.BeginVertical("Box");
                    EditorGUILayout.PropertyField(NewColor, new GUIContent("Clock Color"));
                    if (GUILayout.Button("Change Color"))
                    {
                        Instance.SetNewColor();
                    }
                    GUILayout.EndVertical();
                }
                break;
            case 1:
                if (PrefabName == "Clock3")
                {
                    GUILayout.BeginVertical("Box");
                    GUILayout.Label("Alarm Settings", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(AlarmActive, new GUIContent("AlarmActive"));
                    EditorGUILayout.IntSlider(AlarmHour, 0, 23, new GUIContent("Alarm Hour"));
                    EditorGUILayout.IntSlider(AlarmMin, 0, 59, new GUIContent("Alarm Minute"));
                    GUILayout.EndVertical();
                }else
                {
                    GUILayout.BeginVertical("Box");
                    GUILayout.Label("Transforms", EditorStyles.boldLabel);
                    EditorGUILayout.ObjectField(SecPointer, new GUIContent("Second pointer"));
                    EditorGUILayout.ObjectField(MinPointer, new GUIContent("Minute pointer"));
                    EditorGUILayout.ObjectField(HourPointer, new GUIContent("Hour pointer"));
                    GUILayout.EndVertical();
                }
                break;
            case 2:
                GUILayout.BeginVertical("Box");
                GUILayout.Label("Transforms", EditorStyles.boldLabel);
                EditorGUILayout.ObjectField(SecPointer, new GUIContent("Second pointer"));
                EditorGUILayout.ObjectField(MinPointer, new GUIContent("Minute pointer"));
                EditorGUILayout.ObjectField(HourPointer, new GUIContent("Hour pointer"));
                if (PrefabName == "Clock3")
                {
                    EditorGUILayout.ObjectField(AlarmPointer, new GUIContent("Alarm pointer"));
                    EditorGUILayout.ObjectField(Hammer, new GUIContent("Hammer"));
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical("Box");
                    GUILayout.Label("AudioSource", EditorStyles.boldLabel);
                    EditorGUILayout.ObjectField(AlarmAudio, new GUIContent("Alarm Audio"));
                    GUILayout.EndVertical();
                }else { GUILayout.EndVertical(); }
                
               
                break;
        }
        if (!SysClock)
        {
            GUILayout.BeginVertical("Box");
            GUILayout.Label("Time Active: " + Instance.CustomClock.ToString(strFormat), EditorStyles.boldLabel);
            GUILayout.EndVertical();
        }
        else
        {
            GUILayout.BeginVertical("Box");
            GUILayout.Label("Time Active: " + System.DateTime.Now.ToString(strFormat), EditorStyles.boldLabel);
            GUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();
       
    }
}