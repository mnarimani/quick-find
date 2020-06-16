using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QuickFind
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

                string labelText = label?.text;

                if (labelText == null)
                {
                    labelText = Property.Parent.Label.text;
                }
                
                if (labelText == null)
                {
                    ValueEntry.SmartValue = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
                }
                else
                {
                    // Split label text into words
                    string[] words = labelText.SplitPascalCase().Split(' ', '-', '_');
                    
                    (string path, string name)[] names = guids
                        .Select(guid => (AssetDatabase.GUIDToAssetPath(guid)))
                        .Select(path => (path, Path.GetFileName(path)))
                        .ToArray();

                    string mostSimilarPath = GetMostSimilarTo(words, names);
                    
                    ValueEntry.SmartValue = AssetDatabase.LoadAssetAtPath<T>(mostSimilarPath);
                }
            }
        }

        private string GetMostSimilarTo(string[] words, (string path, string name)[] names)
        {
            string chosen = null;
            int bestScore = int.MinValue;
            
            foreach ((string path, string name) in names)
            {
                // Calculate score on a word basis and add them all together
                int currentScore = words.Sum(x => GetScore(name, x));

                if (currentScore > bestScore)
                {
                    chosen = path;
                    bestScore = currentScore;
                }
            }

            return chosen;
        }
        
        private int GetScore(string source, string other)
        {
            const int sameCaseCharScore = 2;
            const int sameCharScore = 1;

            int score = 0;
            
            for (int i = 0; i < Math.Min(source.Length, other.Length); i++)
            {
                char sChar = source[i];
                char otherChar = other[i];
                
                if (sChar == otherChar)
                {
                    score += sameCaseCharScore;
                    continue;
                }

                if (char.ToLower(sChar) == char.ToLower(otherChar))
                {
                    score += sameCharScore;
                }
            }

            return score;
        }
    }
}