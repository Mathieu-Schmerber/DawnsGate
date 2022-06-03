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

namespace Game.Editor
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Data.Mesh)));
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

                foreach (var item in Data.AttackCombos.ToArray())
                    DrawAttackContent(item);
            }
            SirenixEditorGUI.EndBox();
        }

        private void DrawAttackContent(WeaponData.WeaponAttack item)
        {
            if (!_foldouts.ContainsKey(item))
                _foldouts[item] = new();

            GUILayout.Space(5);

            var bg = GUI.backgroundColor;

            GUI.backgroundColor = Color.black;
            SirenixEditorGUI.BeginBox();
            {
                GUI.backgroundColor = bg;
                SirenixEditorGUI.BeginBoxHeader();
                {
                    Rect rect = EditorGUILayout.BeginHorizontal();
                    {
                        _foldouts[item].foldout = SirenixEditorGUI.Foldout(new Rect(rect.position, new Vector2(10, EditorGUIUtility.singleLineHeight)), _foldouts[item].foldout, new GUIContent());
                        GUILayout.Space(20);
                        item.Attack.AttackData = (AttackBaseData)EditorGUILayout.ObjectField(Data.IsHeavy(item) ? "Heavy Attack" : "Light Attack", item.Attack.AttackData, typeof(AttackBaseData), false);
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

                if (!_foldouts[item].foldout)
                {
                    SirenixEditorGUI.EndBox();
                    return;
                }

                // Animation

                SirenixEditorGUI.BeginBox();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        item.AttackAnimation = (AnimationClip)EditorGUILayout.ObjectField("Animation", item.AttackAnimation, typeof(AnimationClip), false);
                        if (GUILayout.Button(new GUIContent("", EditorGUIUtility.IconContent("animationvisibilitytoggleon").image), GUILayout.Width(45)))
                            AnimationPreviewWindow.OpenWindow<WeaponAttackEvent>(item.AttackAnimation, "OnAnimationEvent", Databases.Database.Templates.Editor.AnimationPreview);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                SirenixEditorGUI.EndBox();

                // Tabs

                var tabs = new GUIContent[] {
                    new GUIContent("Attack", EditorGUIUtility.IconContent("d_AudioMixerController Icon").image),
                    new GUIContent("Dash", EditorGUIUtility.IconContent("d_NavMeshAgent Icon").image),
                    new GUIContent("FX", EditorGUIUtility.IconContent("d_ParticleSystem Icon").image)
                };
                _foldouts[item].selectedTab = GUILayout.Toolbar(_foldouts[item].selectedTab, tabs, GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.5f));

                switch (_foldouts[item].selectedTab)
                {
                    case 0:
                        item.Attack.StartOffset = SirenixEditorFields.Vector3Field("Start Offset", item.Attack.StartOffset);
                        item.Attack.StartOffset = SirenixEditorFields.Vector3Field("Travel distance", item.Attack.TravelDistance);
                        item.Attack.AimAssist = EditorGUILayout.Toggle("Aim assist", item.Attack.AimAssist);
                        item.Attack.LockAim = EditorGUILayout.Toggle("Lock aim", item.Attack.LockAim);
                        item.Attack.LockMovement = EditorGUILayout.Toggle("Lock movement", item.Attack.LockMovement);
                        break;
                    case 1:
                        item.Dash.OnlyWhenMoving = EditorGUILayout.Toggle("Only When Moving", item.Dash.OnlyWhenMoving);
                        item.Dash.OnAnimationEventOnly = EditorGUILayout.Toggle("On Animation Event Only", item.Dash.OnAnimationEventOnly);
                        item.Dash.Distance = SirenixEditorFields.FloatField("Distance", item.Dash.Distance);
                        item.Dash.Duration = SirenixEditorFields.FloatField("Duration", item.Dash.Duration);
                        break;
                    case 2:
                        item.FX.CameraShakeForce = SirenixEditorFields.Vector3Field("Camera Shake Force", item.FX.CameraShakeForce);
                        item.FX.CameraShakeDuration = SirenixEditorFields.FloatField("Camera Shake Duration", item.FX.CameraShakeDuration);
                        item.FX.VibrationForce = SirenixEditorFields.RangeFloatField("Vibration Force", item.FX.VibrationForce, 0, 1);
                        item.FX.VibrationDuration = SirenixEditorFields.FloatField("Vibration Duration", item.FX.VibrationDuration);
                        break;
                }
            }
            SirenixEditorGUI.EndBox();
        }
    }
}