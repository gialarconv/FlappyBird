using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.AttributeUsage(System.AttributeTargets.Field)]
public class InspectorButtonAttribute : PropertyAttribute
{
	public static float kDefaultButtonWidth = 80;

	public readonly string methodName;
	public readonly string buttonLabel;

	private float _buttonWidth = kDefaultButtonWidth;
	public float ButtonWidth
	{
		get { return _buttonWidth; }
		set { _buttonWidth = value; }
	}

	private bool _isToggle = false;
	public bool IsToggle
	{
		get { return _isToggle; }
		set { _isToggle = value; }
	}

	public InspectorButtonAttribute(string inButtonLabel, string inMethodName)
	{
		this.buttonLabel = inButtonLabel;
		this.methodName = inMethodName;
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
public class InspectorButtonPropertyDrawer : PropertyDrawer
{
	private MethodInfo _eventMethodInfo = null;
	private PropertyInfo _togglePropertyInfo = null;

	public override void OnGUI(Rect inPosition, SerializedProperty inProp, GUIContent inLabel)
	{
		InspectorButtonAttribute inspectorButtonAttribute = (InspectorButtonAttribute)attribute;
		Rect buttonRect = new Rect(inPosition.x + (inPosition.width - ((inPosition.width / 3) * 2)) * 0.5f, inPosition.y, ((inPosition.width / 3) * 2), inPosition.height);
		System.Type eventOwnerType = inProp.serializedObject.targetObject.GetType();
		string eventName = inspectorButtonAttribute.methodName;

		if(inspectorButtonAttribute.IsToggle)
		{
			if(_togglePropertyInfo == null)
			{
				_togglePropertyInfo = eventOwnerType.GetProperty(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			}
			
			if(_togglePropertyInfo != null)
			{
				bool value = (bool)_togglePropertyInfo.GetValue(inProp.serializedObject.targetObject);
				EditorGUI.BeginChangeCheck();
				GUI.Toggle(inPosition, value, inspectorButtonAttribute.buttonLabel);
				if(EditorGUI.EndChangeCheck())
				{
					_togglePropertyInfo.SetValue(inProp.serializedObject.targetObject, !value);
				}
			}
		}
		else if(GUI.Button(buttonRect, inspectorButtonAttribute.buttonLabel))
		{
			if(_eventMethodInfo == null)
			{
				_eventMethodInfo = eventOwnerType.GetMethod(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			}

			if(_eventMethodInfo != null)
			{
				_eventMethodInfo.Invoke(inProp.serializedObject.targetObject, null);
			}
			else
			{
				Debug.LogError($"#InspectorButton# Unable to find method {eventName} in {eventOwnerType}");
			}
		}
	}
}
#endif