using UnityEditor;
using UnityEngine;

namespace Valve.VR
{
    public class SteamVR_Input_LiveWindow : EditorWindow
    {
        private GUIStyle labelStyle;

        [MenuItem("Window/SteamVR Input Live View")]
        public static void ShowWindow()
        {
            GetWindow<SteamVR_Input_LiveWindow>(false, "SteamVR Input Live View", true);
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private Vector2 scrollPosition;

        private void OnGUI()
        {
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(EditorStyles.textField);
                labelStyle.normal.background = Texture2D.whiteTexture;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            var defaultColor = GUI.backgroundColor;

            var actionSets = SteamVR_Input.actionSets;
            if (actionSets == null)
                actionSets = SteamVR_Input_References.instance.actionSetObjects;

            var sources = SteamVR_Input_Source.GetUpdateSources();
            for (var sourceIndex = 0; sourceIndex < sources.Length; sourceIndex++)
            {
                var source = sources[sourceIndex];
                EditorGUILayout.LabelField(source.ToString());

                EditorGUI.indentLevel++;

                for (var actionSetIndex = 0; actionSetIndex < actionSets.Length; actionSetIndex++)
                {
                    var set = actionSets[actionSetIndex];
                    var activeText = set.IsActive() ? "Active" : "Inactive";
                    var setLastChanged = set.GetTimeLastChanged();

                    if (setLastChanged != -1)
                    {
                        var timeSinceLastChanged = Time.time - setLastChanged;
                        if (timeSinceLastChanged < 1)
                        {
                            var setColor = Color.Lerp(Color.green, defaultColor, timeSinceLastChanged);
                            GUI.backgroundColor = setColor;
                        }
                    }

                    EditorGUILayout.LabelField(set.GetShortName(), activeText, labelStyle);
                    GUI.backgroundColor = defaultColor;

                    EditorGUI.indentLevel++;

                    for (var actionIndex = 0; actionIndex < set.allActions.Length; actionIndex++)
                    {
                        var action = set.allActions[actionIndex];

                        if (action.actionSet == null || action.actionSet.IsActive() == false)
                        {
                            EditorGUILayout.LabelField(action.GetShortName(), "-", labelStyle);
                            continue;
                        }

                        var actionLastChanged = action.GetTimeLastChanged(source);

                        var actionText = "";

                        float timeSinceLastChanged = -1;

                        if (action is SteamVR_Action_In && ((SteamVR_Action_In)action).GetActive(source) == false)
                        {
                            GUI.backgroundColor = Color.red;
                        }
                        else if (actionLastChanged != -1)
                        {
                            timeSinceLastChanged = Time.time - actionLastChanged;

                            if (timeSinceLastChanged < 1)
                            {
                                var setColor = Color.Lerp(Color.green, defaultColor, timeSinceLastChanged);
                                GUI.backgroundColor = setColor;
                            }
                        }


                        if (action is SteamVR_Action_Boolean)
                        {
                            var actionBoolean = (SteamVR_Action_Boolean)action;
                            actionText = actionBoolean.GetState(source).ToString();
                        }
                        else if (action is SteamVR_Action_Single)
                        {
                            var actionSingle = (SteamVR_Action_Single)action;
                            actionText = actionSingle.GetAxis(source).ToString("0.0000");
                        }
                        else if (action is SteamVR_Action_Vector2)
                        {
                            var actionVector2 = (SteamVR_Action_Vector2)action;
                            actionText = string.Format("({0:0.0000}, {1:0.0000})", actionVector2.GetAxis(source).x, actionVector2.GetAxis(source).y);
                        }
                        else if (action is SteamVR_Action_Vector3)
                        {
                            var actionVector3 = (SteamVR_Action_Vector3)action;
                            var axis = actionVector3.GetAxis(source);
                            actionText = string.Format("({0:0.0000}, {1:0.0000}, {2:0.0000})", axis.x, axis.y, axis.z);
                        }
                        else if (action is SteamVR_Action_Pose)
                        {
                            var actionPose = (SteamVR_Action_Pose)action;
                            var position = actionPose.GetLocalPosition(source);
                            var rotation = actionPose.GetLocalRotation(source);
                            actionText = string.Format("({0:0.0000}, {1:0.0000}, {2:0.0000}) : ({3:0.0000}, {4:0.0000}, {5:0.0000}, {6:0.0000})",
                                position.x, position.y, position.z,
                                rotation.x, rotation.y, rotation.z, rotation.w);
                        }
                        else if (action is SteamVR_Action_Skeleton)
                        {
                            var actionSkeleton = (SteamVR_Action_Skeleton)action;
                            var position = actionSkeleton.GetLocalPosition(source);
                            var rotation = actionSkeleton.GetLocalRotation(source);
                            actionText = string.Format("({0:0.0000}, {1:0.0000}, {2:0.0000}) : ({3:0.0000}, {4:0.0000}, {5:0.0000}, {6:0.0000})",
                                position.x, position.y, position.z,
                                rotation.x, rotation.y, rotation.z, rotation.w);
                        }
                        else if (action is SteamVR_Action_Vibration)
                        {
                            //SteamVR_Input_Action_Vibration actionVibration = (SteamVR_Input_Action_Vibration)action;

                            if (timeSinceLastChanged == -1)
                                actionText = "never used";

                            actionText = string.Format("{0:0} seconds since last used", timeSinceLastChanged);
                        }

                        EditorGUILayout.LabelField(action.GetShortName(), actionText, labelStyle);
                        GUI.backgroundColor = defaultColor;
                    }

                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }


                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndScrollView();
        }
    }
}