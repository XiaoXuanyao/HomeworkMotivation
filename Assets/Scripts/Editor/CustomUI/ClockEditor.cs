using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;



[CustomEditor(typeof(Clock))]
public class ClockEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Clock clock = (Clock)target;

        if (GUILayout.Button("开始计时"))
            clock.StartTiming();
        if (GUILayout.Button("结束计时"))
            clock.StopTiming();
        if (GUILayout.Button("重置计时"))
            clock.ResetTiming();
        if (GUILayout.Button("随机颜色"))
        {
            Undo.RecordObject(clock, "随机颜色");
            clock.hoursCircleColor = clock.RandomColor();
            clock.minutesCircleColor = clock.RandomColor();
            clock.secondsCircleColor = clock.RandomColor();
            clock.coreColor = clock.RandomColor();
            clock.hoursPointerColor = clock.RandomColor();
            clock.minutesPointerColor = clock.RandomColor();
            clock.secondsPointerColor = clock.RandomColor();
            EditorUtility.SetDirty(clock);
            MethodInfo onValidateMethod = typeof(Clock).GetMethod(
                "OnValidate", BindingFlags.NonPublic | BindingFlags.Instance
            );
            if (onValidateMethod != null)
                onValidateMethod.Invoke(clock, null);
        }
    }

}
