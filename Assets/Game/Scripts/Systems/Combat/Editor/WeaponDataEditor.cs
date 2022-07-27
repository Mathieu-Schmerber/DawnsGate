using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Game.Systems.Combat.Weapons;
using Sirenix.Utilities.Editor;
using System;
using Game.Systems.Combat.Attacks;
using Nawlian.Lib.EditorTools.AnimationPreviewWindow;

namespace Game.Systems.Combat.Editor
{
    [CustomEditor(typeof(WeaponData))]
    public class WeaponDataEditor : OdinEditor
    {
        public class Settings
        {
            public bool foldout;
            public int selectedTab;
        }

        private Dictionary<WeaponData.WeaponAttack, Settings> _foldouts = new();

        public WeaponData Data { get; set; }

        public override void OnInspectorGUI()
        {
            Data = target as WeaponData;            
            Draw();
        }

        private void Draw()
        {
            // Get latest version
            serializedObject.Update();

            SirenixEditorGUI.Title("Graphics", "", TextAlignment.Left, true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Data.Material)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Data.Mesh)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Data.InHandPosition)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Data.InHandRotation)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Data.SmoothHandPlacement)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Data.LocomotionLayer)));

            GUILayout.Space(10);

            SirenixEditorGUI.Title("Combo settings", "", TextAlignment.Left, true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Data.AttackSpeed)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Data.ComboIntervalTime)));

            GUILayout.Space(10);

            DrawAttackCombo();
            
            // Save
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAttackCombo()
        {
            SirenixEditorGUI.BeginBox();
            {
                SirenixEditorGUI.BeginBoxHeader();
                {
                    SirenixEditorGUI.Title("Attacks", "", TextAlignment.Left, false);
                    if (GUILayout.Button(new GUIContent("", EditorGUIUtility.IconContent("d_Toolbar Plus").image), GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(40)))
                        Data.AttackCombos.Add(new());
                }
                SirenixEditorGUI.EndBoxHeader();

				for (int i = 0; i < Data.AttackCombos.Count; i++)
                    DrawAttackContent(Data.AttackCombos[i], serializedObject.FindProperty(nameof(Data.AttackCombos)).GetArrayElementAtIndex(i));
                    
            }
            SirenixEditorGUI.EndBox();
        }

        private void DrawAttackContent(WeaponData.WeaponAttack item, SerializedProperty itemProp)
        {
            if (!_foldouts.ContainsKey(item))
                _foldouts[item] = new();

            GUILayout.Space(5);

            var bg = GUI.backgroundColor;

            GUI.backgroundColor = item.HasError() ? Color.red : Color.black;
            SirenixEditorGUI.BeginBox();
            {
                GUI.backgroundColor = bg;
                SirenixEditorGUI.BeginBoxHeader();
                {
                    Rect rect = EditorGUILayout.BeginHorizontal();
                    {
                        _foldouts[item].foldout = SirenixEditorGUI.Foldout(new Rect(rect.position, new Vector2(10, EditorGUIUtility.singleLineHeight)), _foldouts[item].foldout, new GUIContent());
                        GUILayout.Space(20);

                        GUI.backgroundColor = item.Attack.AttackData == null ? Color.red : bg;
                        item.Attack.AttackData = (AttackBaseData)EditorGUILayout.ObjectField(Data.IsHeavy(item) ? "Heavy Attack" : "Light Attack", item.Attack.AttackData, typeof(AttackBaseData), false);
                        GUI.backgroundColor = bg;
                        if (GUILayout.Button(new GUIContent("", EditorGUIUtility.IconContent("P4_DeletedLocal").image), GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(40)))
                        {
                            if (item.Attack.AttackData == null
                                || EditorUtility.DisplayDialog($"Attack {item.Attack.AttackData}", "Do you want to delete this attack ?", "yes", "cancel"))
                                Data.AttackCombos.Remove(item);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                SirenixEditorGUI.EndBoxHeader();

                if (!_foldouts[item].foldout || item.Attack.AttackData == null)
                {
                    SirenixEditorGUI.EndBox();
                    return;
                }

                // Animation

                SirenixEditorGUI.BeginBox();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        bg = GUI.backgroundColor;
                        GUI.backgroundColor = item.ContainsEvent(WeaponAttackEvent.Attack) ? bg : Color.red;
                        EditorGUILayout.PropertyField(itemProp.FindPropertyRelative(nameof(item.AttackAnimation)));
                        if (item.AttackAnimation != null && GUILayout.Button(new GUIContent("", EditorGUIUtility.IconContent("animationvisibilitytoggleon").image), GUILayout.Width(45)))
                            AnimationPreviewWindow.OpenWindow<WeaponAttackEvent>(item.AttackAnimation, "OnAnimationEvent", Databases.Database.Templates.Editor.AnimationPreview);
                        GUI.backgroundColor = bg;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                SirenixEditorGUI.EndBox();

                // Tabs

                var tabs = new GUIContent[] {
                    new GUIContent("Attack", EditorGUIUtility.IconContent("d_AudioMixerController Icon").image),
                    new GUIContent("Auto Dash", EditorGUIUtility.IconContent("d_NavMeshAgent Icon").image),
                    new GUIContent("FX", EditorGUIUtility.IconContent("d_ParticleSystem Icon").image),
                };
                _foldouts[item].selectedTab = GUILayout.Toolbar(_foldouts[item].selectedTab, tabs, GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.5f));

                switch (_foldouts[item].selectedTab)
                {
                    case 0:
                        SerializedProperty attackProp = itemProp.FindPropertyRelative(nameof(item.Attack));

                        if (item.ContainsEvent(WeaponAttackEvent.Attack))
                        {
                            EditorGUILayout.PropertyField(attackProp.FindPropertyRelative(nameof(item.Attack.StartOffset)));
                            EditorGUILayout.PropertyField(attackProp.FindPropertyRelative(nameof(item.Attack.TravelDistance)));
                            EditorGUILayout.PropertyField(attackProp.FindPropertyRelative(nameof(item.Attack.AimAssist)));
                            EditorGUILayout.PropertyField(attackProp.FindPropertyRelative(nameof(item.Attack.LockAim)));
                            EditorGUILayout.PropertyField(attackProp.FindPropertyRelative(nameof(item.Attack.LockMovement)));

                            EditorGUILayout.PropertyField(attackProp.FindPropertyRelative(nameof(item.Attack.UseCustomHandPosition)));
                            if (item.Attack.UseCustomHandPosition)
							{
                                EditorGUILayout.PropertyField(attackProp.FindPropertyRelative(nameof(item.Attack.InHandPosition)));
                                EditorGUILayout.PropertyField(attackProp.FindPropertyRelative(nameof(item.Attack.InHandRotation)));
                                EditorGUILayout.PropertyField(attackProp.FindPropertyRelative(nameof(item.Attack.SmoothHandPlacement)));
                            }
                        }
                        else
                            ShowError($"The '{nameof(item.AttackAnimation)}' doesn't contain any {WeaponAttackEvent.Attack} event.");
                        break;
                    case 1:
                        SerializedProperty dashProp = itemProp.FindPropertyRelative(nameof(item.Dash));

                        if (item.ContainsEvent(WeaponAttackEvent.Dash))
                        {
                            ShowInfo(item.Dash.GetInfo());
                            ShowWarning(item.Dash.GetWarning());

                            EditorGUILayout.PropertyField(dashProp.FindPropertyRelative(nameof(item.Dash.OnlyWhenMoving)));
                            EditorGUILayout.PropertyField(dashProp.FindPropertyRelative(nameof(item.Dash.Distance)));
                        }
                        else
                            ShowInfo($"No 'Dash' event have been setup within the {nameof(item.AttackAnimation)}.");
                        break;
                    case 2:
                        SerializedProperty fxProp = itemProp.FindPropertyRelative(nameof(item.FX));

                        ShowInfo(item.FX.GetInfo());
                        ShowWarning(item.FX.GetWarning());

                        EditorGUILayout.PropertyField(fxProp.FindPropertyRelative(nameof(item.FX.CameraShakeForce)));
                        if (item.FX.CameraShakeForce > 0)
                            EditorGUILayout.PropertyField(fxProp.FindPropertyRelative(nameof(item.FX.CameraShakeDuration)));
                        EditorGUILayout.PropertyField(fxProp.FindPropertyRelative(nameof(item.FX.VibrationForce)));
                        if (item.FX.VibrationForce > 0)
                            EditorGUILayout.PropertyField(fxProp.FindPropertyRelative(nameof(item.FX.VibrationDuration)));
                        break;
                }
            }
            SirenixEditorGUI.EndBox();
        }

        private void ShowWarning(string warning)
        {
            if (string.IsNullOrEmpty(warning))
                return;
            EditorGUILayout.HelpBox(warning, MessageType.Warning);
        }

        private void ShowInfo(string info)
        {
            if (string.IsNullOrEmpty(info))
                return;
            EditorGUILayout.HelpBox(info, MessageType.Info);
        }

        private void ShowError(string error)
        {
            if (string.IsNullOrEmpty(error))
                return;
            EditorGUILayout.HelpBox(error, MessageType.Error);
        }
    }
}