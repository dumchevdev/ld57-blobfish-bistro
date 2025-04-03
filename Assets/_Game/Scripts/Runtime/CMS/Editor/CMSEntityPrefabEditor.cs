using System;
using UnityEditor;
using UnityEngine;

namespace Game.Runtime.CMS.Editor
{
    [CustomEditor(typeof(CMSEntityPrefab))]
    public class CMSEntityPrefabEditor : UnityEditor.Editor
    {
        private SerializedProperty _componentsProperty;
        private string _searchString = "";
        private Vector2 _scrollPos;

        private void OnEnable()
        {
            _componentsProperty = serializedObject.FindProperty("Components");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawEntityInfoSection();
            DrawComponentsSection();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawEntityInfoSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            var entity = (CMSEntityPrefab)target;
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Entity ID", EditorStyles.boldLabel, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField(entity.EntityId, EditorStyles.textField, GUILayout.ExpandWidth(true));
            EditorGUI.EndDisabledGroup();
            
            if (GUILayout.Button("Ping", EditorStyles.miniButton, GUILayout.Width(50)))
            {
                entity.PingEntity();
                EditorUtility.SetDirty(entity);
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Дополнительная информация о сущности
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Components Count", EditorStyles.label, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
            EditorGUILayout.LabelField(_componentsProperty.arraySize.ToString(), EditorStyles.label, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
            
            string newSearch = EditorGUILayout.TextField(_searchString, EditorStyles.toolbarSearchField);
            if (newSearch != _searchString)
            {
                _searchString = newSearch;
                Repaint();
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawComponentsSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Components:", EditorStyles.boldLabel, GUILayout.Width(EditorGUIUtility.labelWidth - 4));

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(false));
            
            for (int i = 0; i < _componentsProperty.arraySize; i++)
            {
                SerializedProperty element = _componentsProperty.GetArrayElementAtIndex(i);
                if (element.managedReferenceValue == null) continue;

                string typeName = element.managedReferenceValue.GetType().Name;
                
                // Фильтрация по поиску
                if (!string.IsNullOrEmpty(_searchString) && 
                    !typeName.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandWidth(true));

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(12); // Увеличьте значение для большего отступа
                // Заголовок компонента
                
                bool isExpanded = EditorGUILayout.Foldout(
                    element.isExpanded,
                    typeName,
                    true
                );
                element.isExpanded = isExpanded;
                
                // Кнопка удаления
                if (GUILayout.Button("×", EditorStyles.miniButtonRight, GUILayout.Width(20), GUILayout.Height(18)))
                {
                    _componentsProperty.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }
                
                EditorGUILayout.EndHorizontal();

                if (isExpanded)
                {
                    // Свойства компонента
                    EditorGUILayout.Space(2);
                    EditorGUI.indentLevel++;
                    DrawAllProperties(element);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space(2);
                }
                else
                {
                    EditorGUILayout.Space(2);
                }
                
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndScrollView();
            
            if (GUILayout.Button("Add Component", EditorStyles.miniButton))
            {
                ShowAddComponentMenu();
            }
            EditorGUILayout.EndVertical();
        }

        private void ShowAddComponentMenu()
        {
            var window = EditorWindow.GetWindow<CMSComponentSelectionWindow>(true, "Add Component", true);
            window.Initialize(this);
            window.position = new Rect(
                GUIUtility.GUIToScreenPoint(Event.current.mousePosition),
                new Vector2(350, 500)
            );
            window.ShowPopup();
        }

        private void DrawAllProperties(SerializedProperty property)
        {
            SerializedProperty iterator = property.Copy();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if ("data" == iterator.name) continue;
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(iterator, true);
                EditorGUILayout.EndHorizontal();
            }
        }

        public void AddComponent(Type componentType)
        {
            int index = _componentsProperty.arraySize;
            _componentsProperty.arraySize++;
            SerializedProperty element = _componentsProperty.GetArrayElementAtIndex(index);
            element.managedReferenceValue = Activator.CreateInstance(componentType);
            serializedObject.ApplyModifiedProperties();
        }
    }
}