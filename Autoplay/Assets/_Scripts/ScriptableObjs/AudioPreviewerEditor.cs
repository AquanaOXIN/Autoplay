using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioPreviewer), true)]
public class AudioPreviewerEditor : Editor
{
    [SerializeField]
    private AudioSource _previewer;

    public void OnEnable()
    {
        _previewer = EditorUtility.CreateGameObjectWithHideFlags("Audio preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
    }

    public void OnDisable()
    {
        DestroyImmediate(_previewer.gameObject);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
        if(GUILayout.Button("Preview"))
        {
            ((AudioPreviewer)target).PlayPreview(_previewer);
        }
        if(GUILayout.Button("Stop Preview"))
        {
            ((AudioPreviewer)target).StopPreview(_previewer);
        }
        EditorGUI.EndDisabledGroup();
    }
}
