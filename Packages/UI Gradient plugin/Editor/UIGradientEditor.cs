using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

/// <summary>
/// Custom editor for UI gradient Effect Component
/// </summary>
namespace PolyAndCode.UI.effect
{
    using Editor = UnityEditor.Editor;

    [CustomEditor(typeof(UIGradient), true)]
    public class UIGradientEditor : Editor
    {
        //Styles under Advanced(Fragment) mode only.
        //UI Gradient uses a single enum for Default(Vertex) and Advanced(Fragment) gradients
        //This Enum is solely for the representation of only styles under Advanced(Fragment) category in the inspector.
        private enum FragmentGradientStyles
        {
            linear,
            radial,
            angle,
            reflected,
            diamond
        }

        //Editor for
        UIGradient script;

        //Gradient as overlay
       string _asOverlayText = "As overlay, gradient's opacity will not affect the source opacity.";

        //Default/advanced selection toolbar 
        int _gradientType;
        string[] toolbarStrings = { "Default (Vertex)", "Advanced (Fragment/Pixel)" };

        //Advanced gradient
        FragmentGradientStyles _fragmentStyle;
        float _linearOffset;

        //Advanced blending in default Gradient
        bool _advancedBlendingReadMore = false;
        string _advancedBlendingText = "The default UI shader cannot be used to achieve advanced blending with the source image. Checking Advanced blending will automatically assign a material with the Gradient shader";

        //Settings
        bool _showSettings = false;

        private void OnEnable()
        {
            script = (UIGradient)target;
            //If graphic component is not available, show dialog and don't add.
            if (script.GetComponent<Graphic>() == null)
            {
                if (EditorUtility.DisplayDialog("Dependencies missing!", "UI Gradient needs a Graphic component(Image, Raw Image etc) ", "Ok"))
                {
                    target.hideFlags |= HideFlags.HideInInspector;
                    DestroyImmediate(target);
                }
                return;
            }
            else
            {
                int gradintStyle = (int)script.gradientStyle - 1;
                _fragmentStyle = (FragmentGradientStyles)(gradintStyle < 0 ? 0 : gradintStyle);
            }

            //Event handling
            script.requireRepaint += repaint;
            Undo.undoRedoPerformed += MyUndoCallback;
        }

        private void OnDisable()
        {
            script.requireRepaint -= repaint;
            Undo.undoRedoPerformed -= MyUndoCallback;
        }

        #region CALLBACKS
        public void repaint()
        {
            _linearOffset = script.offset.x;
            int fragmentGradientIndex = (int)script.gradientStyle - 1;
            _fragmentStyle = (FragmentGradientStyles)(fragmentGradientIndex < 0 ? 0 : fragmentGradientIndex);
            this.Repaint();
        }

        void MyUndoCallback()
        {
            repaint();
            script.UpdateGradientMap();
            script.UpdateMaterial();
        }

        #endregion


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();  
          

            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            bool gradientAsOverlay = EditorGUILayout.Toggle("Gradient as Overlay", script.AsOverlay);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, script.gameObject.name + "Gradient as Overlay");
                script.AsOverlay = gradientAsOverlay;
            }

            EditorGUILayout.HelpBox(_asOverlayText, MessageType.Info);

            EditorGUILayout.Space();
            //EditorGUI.indentLevel--;
            EditorGUI.BeginChangeCheck();
            _gradientType = GUILayout.Toolbar(script.gradientStyle == GradientStyle.Vertex ? 0 : 1, toolbarStrings);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, script.gameObject.name + " gradient type");
                script.gradientStyle = (GradientStyle)_gradientType;
            }

            EditorGUILayout.Space();

            //Vertex Gradient
            if (_gradientType == 0)
            {
                EditorGUI.BeginChangeCheck();
                VertexGradientStyle vertexGradientStyle = (VertexGradientStyle)EditorGUILayout.EnumPopup("Style", script.vertexGradientStyle);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, script.gameObject.name + " gradient style");
                    script.vertexGradientStyle = vertexGradientStyle;
                }
                if (script.vertexGradientStyle == VertexGradientStyle.Linear)
                {
                    EditorGUI.BeginChangeCheck();
                    VertexGradientDirection vertexGradientDirection = (VertexGradientDirection)EditorGUILayout.EnumPopup("Direction", script.vertexGradientDirection);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, script.gameObject.name + " vertex gradient direction");
                        script.vertexGradientDirection = vertexGradientDirection;
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                if (script.vertexGradientStyle == VertexGradientStyle.Corners)
                {
                    EditorGUI.BeginChangeCheck();

                    //Top Left and Right Colors
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Top Left ", GUILayout.MaxWidth(80));
                    Color topLeftColor = EditorGUILayout.ColorField(script.topLeftColor);
                    EditorGUILayout.LabelField("Top Right ", GUILayout.MaxWidth(80));
                    Color topRightColor = EditorGUILayout.ColorField(script.topRightColor);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    //Bottom Left and Right Colors
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Bottom Left ", GUILayout.MaxWidth(80));
                    Color bottomLeftColor = EditorGUILayout.ColorField(script.bottomLeftColor);
                    EditorGUILayout.LabelField("Bottom Right ", GUILayout.MaxWidth(80));
                    Color bottomRightColor = EditorGUILayout.ColorField(script.bottomRightColor);
                    EditorGUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, script.gameObject.name + " corner colors");
                        script.topLeftColor = topLeftColor;
                        script.topRightColor = topRightColor;
                        script.bottomLeftColor = bottomLeftColor;
                        script.bottomRightColor = bottomRightColor;
                        script.SetVerticesDirty();
                    }
                }
                else
                {
                    //Start and end colors
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Start Color ", GUILayout.MaxWidth(80));
                    Color startColor = EditorGUILayout.ColorField(script.startColor);
                    EditorGUILayout.LabelField("End Color", GUILayout.MaxWidth(80));
                    Color endColor = EditorGUILayout.ColorField(script.endColor);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, script.gameObject.name + " linear colors");
                        script.startColor = startColor;
                        script.endColor = endColor;
                        script.SetVerticesDirty();
                    }
                }

                EditorGUILayout.Space();
                DrawUILine(Color.gray);

                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                bool advancedBlending = EditorGUILayout.Toggle("Advanced Blending", script.advancedVertexBlending);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, script.gameObject.name + "advanced blending");
                    script.advancedVertexBlending = advancedBlending;
                }

                EditorGUI.indentLevel++;
                _advancedBlendingReadMore = EditorGUILayout.Foldout(_advancedBlendingReadMore, "Read more");
                if (_advancedBlendingReadMore)
                {
                    EditorGUILayout.HelpBox(_advancedBlendingText, MessageType.Info);
                }

                EditorGUI.indentLevel--;
                if (!script.HasGradientShader())
                {
                    //Default vertex blend modes
                    EditorGUI.BeginChangeCheck();
                    DefaultBlendModes defaultblendMode = (DefaultBlendModes)EditorGUILayout.EnumPopup("Blend Mode", script.defaultblendMode);
                    float defaultModeOpacity = EditorGUILayout.Slider("Opacity", script.defaultModeOpacity, 0, 1);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, script.gameObject.name + " blend mode");
                        script.defaultblendMode = defaultblendMode;
                        script.defaultModeOpacity = defaultModeOpacity;
                    }
                    EditorGUILayout.Space();
                }
                else
                {
                    //Advanced Blending options
                    EditorGUI.BeginChangeCheck();
                    BlendModes blendMode = (BlendModes)EditorGUILayout.EnumPopup("Blend Mode", script.blendMode);
                    float opacity = EditorGUILayout.Slider("Opacity", script.opacity, 0, 1);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, script.gameObject.name + " blend mode");
                        script.blendMode = blendMode;
                        script.opacity = opacity;
                    }
                }
            }
            //Fragment Gradient
            else
            {
                GUI.enabled = script.HasGradientShader();
                //Gradient style: The gradient style enum contains 
                EditorGUI.BeginChangeCheck();
                if (EditorApplication.isPlaying)
                {
                    //Since _fragmentEnum is only fragment gradient and in gradient style fragment enum starts from index 1
                    int fragmentGradientIndex = (int)script.gradientStyle - 1;
                    _fragmentStyle = (FragmentGradientStyles)(fragmentGradientIndex < 0 ? 0 : fragmentGradientIndex);
                }
                _fragmentStyle = (FragmentGradientStyles)(EditorGUILayout.EnumPopup("Style", _fragmentStyle));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, script.gameObject.name + " fragment/pixel style");
                    int val = (int)_fragmentStyle;
                    script.gradientStyle = (GradientStyle)(val + 1);
                }

                //Unity Gradient editor
                EditorGUI.BeginChangeCheck();
                SerializedProperty colorGradient = serializedObject.FindProperty("_gradient");
                EditorGUILayout.PropertyField(colorGradient, true, null);
                if (EditorGUI.EndChangeCheck())
                {
                    //MUST DO: gradient does not update if the following is not done
                    serializedObject.ApplyModifiedProperties();
                    script.UpdateGradientMap();
                }

                EditorGUI.BeginChangeCheck();
                int resolution = EditorGUILayout.IntField("Resolution", Mathf.Max(script.minResolution, script.resolution));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, script.gameObject.name + " resolution");
                    script.resolution = resolution;
                }

                DrawUILine(Color.gray);

                //Blend modes
                EditorGUI.BeginChangeCheck();
                BlendModes blendMode = (BlendModes)EditorGUILayout.EnumPopup("Blend Mode", script.blendMode);
                float opacity = EditorGUILayout.Slider("Opacity", script.opacity, 0, 1);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, script.gameObject.name + " blend mode");
                    script.blendMode = blendMode;
                    script.opacity = opacity;
                }
                DrawUILine(Color.gray);

                //Transformations :
                //For linear and Reflected gradient, offset makes sense to be in only in one direction i.e gradient direction.
                //Though internally using the same offset vector2 by setting only the x component.
                if (script.gradientStyle == GradientStyle.Linear || script.gradientStyle == GradientStyle.Reflected)
                {
                    EditorGUI.BeginChangeCheck();
                    _linearOffset = EditorGUILayout.FloatField("Offset", _linearOffset);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, script.gameObject.name + " offset");
                        script.offset = new Vector2(_linearOffset, script.offset.y);
                    }
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    Vector2 offset = EditorGUILayout.Vector2Field("Offset", script.offset);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, script.gameObject.name + " offset");
                        script.offset = offset;
                    }
                }

                EditorGUI.BeginChangeCheck();
                float scale = EditorGUILayout.FloatField("Scale", script.scale);
                float angle = EditorGUILayout.Slider("Angle", script.angle, 0, 360);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, script.gameObject.name + " transformation");
                    script.scale = scale;
                    script.angle = angle;
                }
                GUI.enabled = true;
            }

            DrawUILine(Color.gray);
            GUI.enabled = script.HasGradientShader();

            _showSettings = EditorGUILayout.Foldout(_showSettings, "Settings");
            if (_showSettings)
            {
                EditorGUI.BeginChangeCheck();
                bool isMaskingSupported = EditorGUILayout.Toggle("Mask support", script.isMaskingSupported);
                DrawUILine(Color.gray);
                EditorGUILayout.LabelField("Canvas Settings", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Following settings apply to the Parent canvas and all it's children UI Gradients");
                bool useTexcoord1 = EditorGUILayout.Toggle("Use Texcoord1", script.useTexcoord1);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, script.gameObject.name + " setting");
                    script.isMaskingSupported = isMaskingSupported;
                    script.useTexcoord1 = useTexcoord1;
                }
            }
            GUI.enabled = true;

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
            {
                if (!EditorApplication.isPlaying)
                {
                    EditorUtility.SetDirty(target);
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
            }
        }

        /// <summary>
        ///Draws a horizontal line in the inspector
        ///From https://forum.unity.com/threads/horizontal-line-in-editor-window.520812/
        /// </summary>
        public static void DrawUILine(Color color, int thickness = 1, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
    }
}