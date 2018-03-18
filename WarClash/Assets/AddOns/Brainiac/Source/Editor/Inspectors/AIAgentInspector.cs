using UnityEngine;
using UnityEditor;
using Brainiac;
using Logic.LogicObject;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace BrainiacEditor
{
    [CustomEditor(typeof(LogicObject))]
	public class AIAgentInspector : Editor
	{
        private IBlackboardInspector m_inspector;
	
        private void OnEnable()
        {
            if(EditorApplication.isPlaying)
            {
                
                LogicObject lo = (LogicObject)target;
                var aic = lo.so.GetComponent<AIAgent>();
                if (aic != null)
                {
                    AIAgent agent = aic;
                    Blackboard blackboard = agent.Blackboard;
                    IDictionary<string, object> dict = GetRuntimeValues(blackboard);
                    if (dict != null)
                    {
                        m_inspector = new PlayTimeBlackboardInspector(dict);
                    }
                }
               
                
            }
            else
            {
                m_inspector = new DesignTimeBlackboardInspector(serializedObject);
            }
        }

        private IDictionary<string, object> GetRuntimeValues(Blackboard blackboard)
        {
            Type type = blackboard.GetType();
            FieldInfo fi = type.GetField("m_values", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);

            if(fi != null)
                return fi.GetValue(blackboard) as IDictionary<string, object>;
            else
                return null;
        }
		
		public override void OnInspectorGUI()
		{
		    LogicObject lo = (LogicObject)target;
            if (EditorApplication.isPlaying)
		    {
		        if (!lo.ShowAttrs && GUILayout.Button("ShowAttributes"))
		        {
		            lo.ShowAttrs = true;
		        }
		        if (lo.ShowAttrs && GUILayout.Button("HideAttributes"))
		        {
		            lo.ShowAttrs = false;
		        }
            }
			serializedObject.Update();

			GUI.enabled = !EditorApplication.isPlaying;
			GUI.color = Color.white;

			serializedObject.ApplyModifiedProperties();
            var agent = lo.so.GetComponent<AIAgent>();
            if (agent != null)
            {
		        BTAsset btAsset = agent.BehaviourTree as BTAsset;
		        BehaviourTree btInstance = agent.GetBehaviourTree();

		        GUI.enabled = btAsset != null;
		        if (EditorApplication.isPlaying && btInstance != null)
		        {
		            if (GUILayout.Button("Preview", GUILayout.Height(24.0f)))
		            {
		                BehaviourTreeEditor.OpenDebug(btAsset, btInstance);
		            }
		        }
		        else
		        {
		            if (GUILayout.Button("Edit", GUILayout.Height(24.0f)))
		            {
		                BehaviourTreeEditor.Open(btAsset);
		            }
		        }
		    }

		    if(m_inspector != null)
            {
                BTEditorStyle.EnsureStyle();
                m_inspector.DrawGUI();
                Repaint();
            }
            else
            {
                EditorGUILayout.HelpBox("There are no values to display!", MessageType.Error);
            }
			GUI.enabled = true;
		}
	}
}
