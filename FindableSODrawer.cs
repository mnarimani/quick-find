using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ShipShared.Editor
{
    public class FindableSODrawer<T> : OdinValueDrawer<T> where T : ScriptableObject
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Rect position = EditorGUILayout.GetControlRect();
            Rect fieldRect = position.SubXMax(25);

            ValueEntry.SmartValue = (T) SirenixEditorFields.UnityObjectField(fieldRect,
                label,
                ValueEntry.SmartValue,
                typeof(T),
                false);

            var buttonRect = new Rect(fieldRect.xMax, fieldRect.y, 25, fieldRect.height);
            if (GUI.Button(buttonRect, "F"))
            {
                var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");

                if (guids.Length == 0)
                {
                    Debug.LogWarning("Could not find instance of type " + typeof(T));
                    return;
                }

                var similars = guids
                    .Select(g => (AssetDatabase.GUIDToAssetPath(g)))
                    .Select(p => (p, Path.GetFileName(p)))
                    .ToArray();

                string labelText = label?.text;

                if (labelText == null)
                {
                    labelText = Property.Parent.Label.text;
                }

                if (labelText == null)
                {
                    ValueEntry.SmartValue = AssetDatabase.LoadAssetAtPath<T>(similars.First().p);
                }
                else
                {
                    var mostSimilar = GetMostSimiliarTo(labelText, similars.Select(s => s.Item2));

                    ValueEntry.SmartValue =
                        AssetDatabase.LoadAssetAtPath<T>(similars.First(s => s.Item2 == mostSimilar).p);
                }
            }
        }

        private string GetMostSimiliarTo(string source, IEnumerable<string> similars)
        {
            var scores = similars.Select(s => (GetSimilarScore(source, s), s)).ToList();
            var mostScore = scores.Max(s => s.Item1);
            return scores.First(s => s.Item1 == mostScore).s;
        }


        private int GetSimilarScore(string source, string other)
        {
            other = other.Replace(" ", "").Replace("_", "");
            const int SameCaseCharScore = 2;
            const int SameCharScore = 1;

            int score = 0;
            for (int i = 0; i < Math.Min(source.Length, other.Length); i++)
            {
                var sChar = source[i];
                var otherChar = other[i];
                if (sChar == otherChar)
                {
                    score += SameCaseCharScore;
                    continue;
                }

                if (char.ToLower(sChar) == char.ToLower(otherChar))
                {
                    score += SameCharScore;
                }
            }

            return score;
        }

        // public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        // {
        //     Rect fieldRect = position.SubXMax(60);
        //     EditorGUI.PropertyField(fieldRect, property, label);
        //
        //     if (GUI.Button(new Rect(fieldRect.xMax, fieldRect.y, 60, fieldRect.height), "Find SO"))
        //     {
        //         string type = property.GetProperTypeName();
        //
        //         var guids = AssetDatabase.FindAssets($"t:{type}");
        //
        //         if (guids.Length == 0)
        //         {
        //             Debug.LogWarning("Could not find instance of type " + type);
        //             return;
        //         }
        //
        //         if (guids.Length > 1)
        //         {
        //             Debug.LogWarning("Found more than one instance of type " + type);
        //         }
        //         
        //         string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        //         var loaded = AssetDatabase.LoadAssetAtPath<FindableSO>(path);
        //         property.objectReferenceValue = loaded;
        //     }
        // }
    }
}