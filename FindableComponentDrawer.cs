using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace ShipShared.Editor
{
    public class FindableComponentDrawer<T> : OdinValueDrawer<T> , IDefinesGenericMenuItems where T : Component
    {
        protected override void Initialize()
        {
            base.Initialize();
            SkipWhenDrawing = true;
        }

        public void PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
        {
            genericMenu.AddItem(new GUIContent("Find In Self"), false, FindInSelf);
            genericMenu.AddItem(new GUIContent("Find In Parent"), false, FindInParent);
            genericMenu.AddItem(new GUIContent("Find In Children"), false, FindInChildren);
            genericMenu.AddItem(new GUIContent("Find In Scene"), false, FindInScene);
        }

        private void FindInSelf()
        {
            for (int i = 0; i < Property.Tree.WeakTargets.Count; ++i)
            {
                var target = Property.Tree.WeakTargets[i] as Component;

                if (target == null)
                    continue;

                ValueEntry.Values[i] = target.GetComponent<T>();
            }
        }

        private void FindInParent()
        {
            for (int i = 0; i < Property.Tree.WeakTargets.Count; ++i)
            {
                var target = Property.Tree.WeakTargets[i] as Component;

                if (target == null)
                    continue;

                ValueEntry.Values[i] = target.GetComponentInParent<T>();
            }
        }

        private void FindInChildren()
        {
            for (int i = 0; i < Property.Tree.WeakTargets.Count; ++i)
            {
                var target = Property.Tree.WeakTargets[i] as Component;

                if (target == null)
                    continue;

                ValueEntry.Values[i] = target.GetComponentInChildren<T>();
            }
        }
        
        private void FindInScene()
        {
            for (int i = 0; i < Property.Tree.WeakTargets.Count; ++i)
            {
                var target = Property.Tree.WeakTargets[i] as Component;

                if (target == null)
                    continue;

                ValueEntry.Values[i] = Object.FindObjectOfType<T>();
            }
        }
    }
}