using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Applies Gradient effect to UI.
/// Gradient Types:
/// Default : Vertex (Corner, Linear)
/// Advanced : Linear, Radial, Angle, Reflected, Diamond
/// Blend Modes : default and advanced
/// </summary>
namespace PolyAndCode.UI.effect
{
    [AddComponentMenu("UI/Effects/Gradient")]
    public class UIGradient : BaseMeshEffect
    {
        //For editor to do repaint when required
        public event System.Action requireRepaint;

        //Constants
        public const string GradientShaderPath = "PolyAndCode/UI/Gradient";
        const string Uv1keyword = "USE_UV1";
        Shader gradientShader;

        /// <summary>
        /// Enum for Shader properties.
        /// </summary>
        private enum ShaderProperties
        {
            _AsOverlay,
            _MainColor,
            _Opacity,
            _Angle,
            _OffsetX,
            _OffsetY,
            _Scale,
            _GradientTex,
            _AspectRatio,
        }

        //General
        [SerializeField]
        private bool _asOverlay;

        private Material _mat;
        private Sprite _sprite;
        private Texture _texture;
        private bool _verticesCached;
        private int _currentVertCount;
        private Vector2 _length, _minBounds;
        private List<VertexCornerMap> _vertexCornerMaps;
        private Vector2 _imageSize;
        private Texture2D _gradientMap;
        [SerializeField]
        private bool _isMaskingSupported = true;
        private bool _isMasked;
        [SerializeField]
        private bool _advancedVertexBlending = false;
        [SerializeField]
        private bool _useTexcoord1 = true;

        //Vertex Gradient
        [SerializeField]
        Color[] _vertexColors = new Color[] { Color.green, Color.white, Color.red, Color.blue }; // 0 - Bottom left, 1 - Top left, 2- Top right left, 3 - Bottom right
        [SerializeField]
        Color _startColor = Color.black;
        [SerializeField]
        Color _endColor = Color.white;
        [SerializeField]
        private DefaultBlendModes _defaultblendMode;
        [SerializeField]
        private float _defaultModeOpacity = 1;
        [SerializeField]
        private VertexGradientDirection _vertexGradientDirection;
        [SerializeField]
        private VertexGradientStyle _vertexGradientStyle;

        //Gradient generation
        [SerializeField]
        private Gradient _gradient;
        [HideInInspector]
        public int minResolution //Accessed by Editor script
        {
            get
            {
                return 8;
            }
        }
        [SerializeField]
        private int _resolution = 10;
        private int _texHeight = 10;

        //Fragment(Pixel) Gradient
        [SerializeField]
        private GradientStyle _gradientStyle;
        [SerializeField]
        private BlendModes _blendMode;

        //Fragment(Pixel) Gradient Transformation
        [SerializeField]
        private Vector2 _offset;
        [SerializeField]
        private float _scale = 1;
        [SerializeField]
        private float _opacity = 1;
        [SerializeField]
        private float _angle;

        //Properties to set most of the private fields.
        //Reason : Dependent function calls and condition checks are done while setting fields.
        //(e.g : Set corner color only when vertex gradient is set and call setverticesdirty())
        //This also ensures that changes at runtime don't change unapplicable fields.
        #region PROPERTIES
        /// <summary>
        /// Gradient style
        /// Default and advanced are in the same enum
        /// </summary>
        /// <value></value>
        
        public bool AsOverlay
        {
            set
            {
                _asOverlay = value;
                if (HasGradientShader())
                {
                    SetMaterialFloat(ShaderProperties._AsOverlay, _asOverlay ? 1 : 0);
                }

                if (gradientStyle == GradientStyle.Vertex)
                {
                    SetVerticesDirty();
                }
            }
            get
            {
                return _asOverlay;
            }
        }

        public GradientStyle gradientStyle
        {
            set
            {
                //Do nothing if trying to set the same value to avoid calling further operations.
                if (_gradientStyle == value)
                {
                    return;
                }
                //If vertex gradient, disable existing style keyword in gradient shader material
                if (value == GradientStyle.Vertex)
                {
                    if (HasGradientShader())
                    {
                        SetMaterialKeyword(_gradientStyle, false);
                        //If returned from Fragment(Pixel) type gradient and  advancedBlending is false, switch back to default gradient
                        if (!advancedVertexBlending)
                        {
                            graphic.material = null;
                        }
                    }
                    SetVerticesDirty();
                }
                else
                {
                    if (HasGradientShader())
                    {
                        SetMaterialKeyword(_gradientStyle, false);
                        SetMaterialKeyword(value, true);
                    }
                }

                //If current style is vertex, update the gradient map and material
                if (gradientStyle == GradientStyle.Vertex)
                {
                    _gradientStyle = value;
                    UpdateGradientMap();
                    UpdateMaterial();
                }
                else
                {
                    _gradientStyle = value;
                }

            }
            get
            {
                return _gradientStyle;
            }
        }

        /// <summary>
        /// Style when Gradient Style is Default(Vertex)
        /// Corner and Linear
        /// </summary>
        /// <value></value>
        public VertexGradientStyle vertexGradientStyle
        {
            set
            {
                //Do nothing if trying to set the same value to avoid calling further operations.
                if (gradientStyle == GradientStyle.Vertex && _vertexGradientStyle != value)
                {
                    _vertexGradientStyle = value;
                    SetVerticesDirty();
                }
            }
            get
            {
                return _vertexGradientStyle;
            }
        }

        /// <summary>
        /// Direction Default(Vertex) - Linear style
        /// </summary>
        /// <value></value>
        public VertexGradientDirection vertexGradientDirection
        {
            set
            {
                //Do nothing if trying to set the same value to avoid calling further operations.
                if (gradientStyle == GradientStyle.Vertex && vertexGradientStyle == VertexGradientStyle.Linear)
                {
                    if (_vertexGradientDirection != value)
                    {
                        _vertexGradientDirection = value;
                        SetVerticesDirty();
                    }
                }
            }
            get
            {
                return _vertexGradientDirection;
            }
        }

        /// <summary>
        /// Default mode Blend Mode in Default(Vertex) gradient
        /// Default blend mode does not blend gradient with source image
        /// </summary>
        /// <value></value>
        public DefaultBlendModes defaultblendMode
        {
            set
            {
                //Do nothing if trying to set the same value to avoid calling further operations.
                if (gradientStyle == GradientStyle.Vertex && _defaultblendMode != value)
                {
                    _defaultblendMode = value;
                    SetVerticesDirty();
                }
            }
            get
            {
                return _defaultblendMode;
            }
        }

        /// <summary>
        /// Blend modes
        /// Uses Gradient shader
        /// </summary>
        /// <value></value>
        public BlendModes blendMode
        {
            set
            {
                //Do nothing if trying to set the same value to avoid calling further operations.
                if (_blendMode == value)
                {
                    return;
                }
                if (HasGradientShader())
                {
                    SetMaterialKeyword(_blendMode, false);
                    SetMaterialKeyword(value, true);
                }
                _blendMode = value;
            }
            get
            {
                return _blendMode;
            }
        }

        /// <summary>
        /// Advanced blending for Default(Vertex) type gradient
        /// This uses Gradient shader.
        /// </summary>
        /// <value></value>
        public bool advancedVertexBlending
        {
            set
            {
                //Do nothing if trying to set the same value to avoid calling further operations.
                if (_advancedVertexBlending == value)
                {
                    return;
                }
                _advancedVertexBlending = value;

                //Apply gradient shader, update material and vertices
                if (_advancedVertexBlending)
                {
                    UpdateMaterial(false);
                }
                else if (gradientStyle == GradientStyle.Vertex)
                {
                    graphic.material = null;
                }
            }
            get
            {
                return _advancedVertexBlending;
            }
        }

        /// <summary>
        /// Opacity in Default(Vertex) mode and  advanced blending OFF
        /// </summary>
        /// <value></value>
        public float defaultModeOpacity
        {
            set
            {
                if (gradientStyle == GradientStyle.Vertex && _defaultModeOpacity != value)
                {
                    SetVerticesDirty();
                    _defaultModeOpacity = value;
                }
            }
            get
            {
                return _defaultModeOpacity;
            }
        }

        /// <summary>
        /// Start color for Vertex-Linear gradient
        /// </summary>
        /// <value></value>
        public Color startColor
        {
            set
            {
                if (gradientStyle == GradientStyle.Vertex && vertexGradientStyle == VertexGradientStyle.Linear && _startColor != value)
                {
                    _startColor = value;
                    SetVerticesDirty();
                }
            }
            get
            {
                return _startColor;
            }
        }

        /// <summary>
        /// End color for Vertex-Linear gradient
        /// </summary>
        /// <value></value>
        public Color endColor
        {
            set
            {
                if (gradientStyle == GradientStyle.Vertex && vertexGradientStyle == VertexGradientStyle.Linear && _endColor != value)
                {
                    _endColor = value;
                    SetVerticesDirty();
                }
            }
            get
            {
                return _endColor;
            }
        }

        /// <summary>
        /// Top left for Vertex-Corner gradient
        /// </summary>
        /// <value></value>
        public Color topLeftColor
        {
            set
            {
                if (gradientStyle == GradientStyle.Vertex && vertexGradientStyle == VertexGradientStyle.Corners)
                {
                    if (_vertexColors[1] != value)
                    {
                        _vertexColors[1] = value;
                        SetVerticesDirty();
                    }
                }
            }
            get
            {
                return _vertexColors[1];
            }
        }

        /// <summary>
        /// Top right color for Vertex-Corner gradient
        /// </summary>
        /// <value></value>
        public Color topRightColor
        {
            set
            {
                if (gradientStyle == GradientStyle.Vertex && vertexGradientStyle == VertexGradientStyle.Corners)
                {
                    if (_vertexColors[2] != value)
                    {
                        _vertexColors[2] = value;
                        SetVerticesDirty();
                    }
                }
            }
            get
            {
                return _vertexColors[2];
            }
        }

        /// <summary>
        /// Bottom left color for Vertex-Corner gradient
        /// </summary>
        /// <value></value>
        public Color bottomLeftColor
        {
            set
            {
                if (gradientStyle == GradientStyle.Vertex && vertexGradientStyle == VertexGradientStyle.Corners)
                {
                    if (_vertexColors[0] != value)
                    {
                        _vertexColors[0] = value;
                        SetVerticesDirty();
                    }
                }
            }
            get
            {
                return _vertexColors[0];
            }
        }

        /// <summary>
        /// Bottom right color for Vertex-Corner gradient
        /// </summary>
        /// <value></value>
        public Color bottomRightColor
        {
            set
            {
                if (gradientStyle == GradientStyle.Vertex && vertexGradientStyle == VertexGradientStyle.Corners)
                {
                    if (_vertexColors[3] != value)
                    {
                        _vertexColors[3] = value;
                        SetVerticesDirty();
                    }
                }
            }
            get
            {
                return _vertexColors[3];
            }
        }

        /// <summary>
        /// Gradient for Fragment(Pixel) types
        /// </summary>
        /// <value></value>
        public Gradient gradient
        {
            set
            {
                if (gradientStyle != GradientStyle.Vertex)
                {
                    _gradient = value;
                    UpdateGradientMap();
                }
            }
            get
            {
                return _gradient;
            }
        }

        /// <summary>
        /// Resolution of gradient 
        /// When gradient is "fixed" type, higher resolution is required 
        /// </summary>
        /// <value></value>
        public int resolution
        {
            set
            {
                if (gradientStyle != GradientStyle.Vertex && _resolution != value)
                {
                    _resolution = value;
                    UpdateGradientMap();
                }
            }
            get
            {
                return _resolution;
            }
        }

        /// <summary>
        /// Sets opacity of the gradient for both types.
        /// (advanced blending in Vertex type)
        /// </summary>
        /// <value></value>
        public float opacity
        {
            set
            {
                if (HasGradientShader())
                {
                    if (_opacity == value)
                    {
                        return;
                    }
                    _opacity = Mathf.Clamp(value, 0, 1);
                    SetMaterialFloat(ShaderProperties._Opacity, _opacity);
                }

            }
            get
            {
                return _opacity;
            }
        }

        /// <summary>
        /// Set angle for Fragment(Pixel) type gradients
        /// </summary>
        /// <value></value>
        public float angle
        {
            set
            {
                if (_angle == value)
                {
                    return;
                }
                if (gradientStyle != GradientStyle.Vertex && HasGradientShader())
                {
                    _angle = value;
                    SetMaterialFloat(ShaderProperties._Angle, value);
                }

            }
            get
            {
                return _angle;
            }
        }

        /// <summary>
        /// Set scale for the Fragment(Pixel) type gradients
        /// </summary>
        /// <value></value>
        public float scale
        {
            set
            {
                if (_scale == value)
                {
                    return;
                }
                if (gradientStyle != GradientStyle.Vertex && HasGradientShader())
                {
                    _scale = value;
                    SetMaterialFloat(ShaderProperties._Scale, value);
                }

            }
            get
            {
                return _scale;
            }
        }

        /// <summary>
        /// Offset for the gradient
        /// </summary>
        /// <value></value>
        public Vector2 offset
        {
            set
            {
                //Only for Fragment(Pixel) type gradients
                if (_offset == value)
                {
                    return;
                }
                if (gradientStyle != GradientStyle.Vertex && HasGradientShader())
                {
                    _offset = value;
                    SetMaterialFloat(ShaderProperties._OffsetX, value.x);
                    SetMaterialFloat(ShaderProperties._OffsetY, value.y);

                }
            }
            get
            {
                return _offset;
            }
        }

        /// <summary>
        /// Should the gradient support masking.
        /// Unsupported gradients won't do parent mask checks.
        /// </summary>
        /// <value></value>
        public bool isMaskingSupported
        {
            set
            {
                _isMaskingSupported = value;

                //If true check if parent has mask
                if (value)
                {
                    if (GetComponentInParent(typeof(Mask)))
                    {
                        isMasked = true;
                        UpdateMaterial(false);
                    }
                    else
                    {
                        isMasked = false;
                    }

                }
            }
            get
            {
                return _isMaskingSupported;
            }
        }

        /// <summary>
        /// Is the gradient under an active Mask
        /// </summary>
        /// <value></value>
        public bool isMasked
        {
            set
            {
                _isMasked = value;
                if (_isMasked)
                {
                    UpdateMaterial(false);
                }
            }
            get
            {
                return _isMasked;
            }
        }

        /// <summary>
        /// Canvas level setting.
        /// If Canvas texcoord1 should be used or not
        /// If true, sets it in canvas and for all the child gradients
        /// If false , don't undo since some other non gradient UI child might require texcoord1.
        ///</summary>
        /// <value></value>
        public bool useTexcoord1
        {
            set
            {
                if (_useTexcoord1 == value)
                {
                    return;
                }
                _useTexcoord1 = value;

                //Set in canvas
                if (_useTexcoord1)
                {
                    graphic.canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
                }

                //Set in all child gradients
                foreach (var item in graphic.canvas.GetComponentsInChildren<UIGradient>())
                {
                    item.useTexcoord1 = useTexcoord1;
                }

                if (HasGradientShader())
                {
                    SetMaterialKeyword(Uv1keyword, useTexcoord1);
                }
            }
            get
            {
                return _useTexcoord1;
            }
        }

        #endregion

        #region MAIN

        /// <summary>
        /// Set up UI Gradient : Set all material properties
        /// </summary>
        protected override void Awake()
        {
            //Awake
            if (graphic == null)
            {
                Debug.LogWarning("UI Gradient: Graphic component Missing");
                return;
            }
            else
            {
                gradientShader = Shader.Find(GradientShaderPath);
                if (_gradient == null)
                {
                    InitGradient();
                }

#if UNITY_EDITOR
                if (isMaterialAlreadyUsed())
                {
                    ApplyGradientshader();
                }
#endif
                base.Awake();
                SetVerticesDirty();
                UpdateGradientMap();
                UpdateMaterial();
                _mat = graphic.material;
            }
        }

        /// <summary>
        /// Applies gradient shader to the graphic
        /// </summary>
        private void ApplyGradientshader()
        {
            var material = new Material(gradientShader);
            graphic.material = material;
            //Set it to check material changed conditions in ModifyMesh
            _mat = graphic.material;
        }

        /// <summary>
        /// Check dependent properties in OnEnable
        /// </summary>
        protected override void OnEnable()
        {
            //To make the gradient update under masks
            //Comment if you are not planning to use gradient under masks at all
            if (isMaskingSupported)
            {
                if (GetComponentInParent(typeof(Mask)))
                {
                    isMasked = true;
                }
            }

            if (graphic.canvas != null)
            {
                if (useTexcoord1 && graphic.canvas.additionalShaderChannels != AdditionalCanvasShaderChannels.TexCoord1)
                {
                    graphic.canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
                }
            }
        }

        /// <summary>
        /// To set vertices dirty 
        /// </summary>
        public void SetVerticesDirty()
        {
            graphic.SetVerticesDirty();
        }

        /// <summary>
        /// Default value for the gradient
        /// </summary>
        private void InitGradient()
        {
            //Not setting through property since property does not allow setting gradient in non-fragment mode
            _gradient = new Gradient();
            int Length = 3;
            GradientColorKey[] colorKeys = new GradientColorKey[Length];
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[Length];
            Color[] colors = new Color[] { Color.white, Color.cyan, Color.yellow };
            float[] times = new float[] { 0, 0.5f, 1 };

            for (int i = 0; i < Length; i++)
            {
                colorKeys[i].color = colors[i];
                colorKeys[i].time = times[i];
                alphaKeys[i].alpha = 1.0F;
            }
            _gradient.SetKeys(colorKeys, alphaKeys);
        }

        /// <summary>
        /// For setting vertex type gradients
        /// - Determines the corner vertices (refered to as caching vertices)
        /// - Updates aspect ratio in the material
        /// - Updates second UV channel for gradient UV
        /// </summary>
        public override void ModifyMesh(VertexHelper vh)
        {
            if (graphic == null)
            {
                Debug.LogWarning("UI Gradient: Graphic component Missing");
                return;
            }
            //Must check first. Is size Negative?
            Vector2 presentSize = RectTransformUtility.PixelAdjustRect(graphic.rectTransform, graphic.canvas).size;
            if (presentSize.x < 0 || presentSize.y < 0)
            {
                return;
            }

            //MUST DO everytime : Texcoord1 UVs reset every time ModifyMesh is called.
            var vertex = new UIVertex();
            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);
                vertex.uv1 = new Vector2((vertex.position.x - _minBounds.x) / _length.x, (vertex.position.y - _minBounds.y) / _length.y);
                vh.SetUIVertex(vertex, i);
            }

            //If material is changed , not from this script.
            if (!_mat.Equals(graphic.material))
            {
                UpdateGradientMap();
                UpdateMaterial();
                _mat = graphic.material;
            }

            //Required for both type of gradients
            SetMaterialColor(ShaderProperties._MainColor, graphic.color);

            //If image size has changed, update the aspect ratio in material.
            //Need vertex caching also for determining corner vertices and recalculating size for uv1.
            if (_imageSize != presentSize)
            {
                _imageSize = RectTransformUtility.PixelAdjustRect(graphic.rectTransform, graphic.canvas).size;
                SetMaterialFloat(ShaderProperties._AspectRatio, _imageSize.y / _imageSize.x);
                _verticesCached = false;
            }
            //Whenever the vertex count changes , Cache Corner vertices again.
            else if (_currentVertCount != vh.currentVertCount)
            {
                _verticesCached = false;
                _currentVertCount = vh.currentVertCount;
            }
            else
            {
                //At last check if the ui component's source texture has changed
                RawImage rawImage = GetComponent<RawImage>();
                Image image = GetComponent<Image>();
                if (image != null && _sprite != image.sprite)
                {
                    _verticesCached = false;
                    _sprite = image.sprite;
                }
                else if (rawImage != null && _texture != rawImage.texture)
                {
                    _verticesCached = false;
                    _texture = rawImage.texture;
                }
            }

            //Fragment(Pixel) gradients do not need anything further
            if (_verticesCached && gradientStyle != GradientStyle.Vertex)
            {
                return;
            }

            //Cache vertices here : Caching vertices basically means saving the closest corner for every vertex
            if (!_verticesCached)
            {
                Vector3[] cornerPositions = new Vector3[4];
                graphic.rectTransform.GetLocalCorners(cornerPositions);

                //Reset Vertex-Corner Maps
                if (_vertexCornerMaps == null)
                {
                    _vertexCornerMaps = new List<VertexCornerMap>();
                }
                else
                {
                    _vertexCornerMaps.Clear();
                }

                //Save these to avoid recalculation at the time of uv1 resetting
                _minBounds.x = cornerPositions[0].x;
                _minBounds.y = cornerPositions[0].y;
                _length.x = cornerPositions[2].x - cornerPositions[0].x;
                _length.y = cornerPositions[1].y - cornerPositions[0].y;

                //Identify the closest corner for each vertex and map.
                int cornerIndex = 0;
                for (int i = 0; i < vh.currentVertCount; i++)
                {
                    float minDist = float.MaxValue;
                    vh.PopulateUIVertex(ref vertex, i);
                    for (int j = 0; j < cornerPositions.Length; j++)
                    {
                        if (Vector3.Distance(cornerPositions[j], vertex.position) < minDist)
                        {
                            cornerIndex = j;
                            minDist = Vector3.Distance(cornerPositions[j], vertex.position);
                        }
                    }
                    //Mapping closest corner
                    _vertexCornerMaps.Add(new VertexCornerMap(i, cornerIndex));
                    //Setting uv1
                    vertex.uv1 = new Vector2((vertex.position.x - _minBounds.x) / _length.x, (vertex.position.y - _minBounds.y) / _length.y);
                    vh.SetUIVertex(vertex, i);
                }
                _verticesCached = true;
            }

            if (_verticesCached)
            {
                // Assign colors to the vertex according to their respective corners
                // 0 - Top left, 1 - Top right, 2- Bottom left, 4 - Bottom right
                for (int i = 0; i < _vertexCornerMaps.Count; i++)
                {
                    vh.PopulateUIVertex(ref vertex, _vertexCornerMaps[i].vertexIndex);

                    if (vertexGradientStyle == VertexGradientStyle.Corners)
                    {
                        vertex.color = GetColorWithDefaultBlending(graphic.color, _vertexColors[_vertexCornerMaps[i].cornerIndex]);
                        if (!AsOverlay) {
                            var alpha = _vertexColors[_vertexCornerMaps[i].cornerIndex].a;
                            vertex.color.a = (byte)(alpha * 255);
                        }
                    }
                    else
                    {
                        //Assign start and end color to corners Based on direction in Linear mode.
                        if (vertexGradientDirection == VertexGradientDirection.Horizontal)
                        {
                            vertex.color = (_vertexCornerMaps[i].cornerIndex == 0 || _vertexCornerMaps[i].cornerIndex == 1) ?
                                            GetColorWithDefaultBlending(graphic.color, startColor) : GetColorWithDefaultBlending(graphic.color, endColor);
                            if (!AsOverlay)
                            {
                                var alpha = (_vertexCornerMaps[i].cornerIndex == 0 || _vertexCornerMaps[i].cornerIndex == 1) ? startColor.a : endColor.a;
                                vertex.color.a = (byte)(alpha * 255);
                            }
                        }
                        else
                        {
                            vertex.color = (_vertexCornerMaps[i].cornerIndex == 1 || _vertexCornerMaps[i].cornerIndex == 2) ?
                                            GetColorWithDefaultBlending(graphic.color, startColor) : GetColorWithDefaultBlending(graphic.color, endColor);
                            if (!AsOverlay)
                            {
                                var alpha = (_vertexCornerMaps[i].cornerIndex == 1 || _vertexCornerMaps[i].cornerIndex == 2) ? startColor.a : endColor.a;
                                vertex.color.a = (byte)(alpha * 255);
                            }   

                        }
                    }
                    vh.SetUIVertex(vertex, _vertexCornerMaps[i].vertexIndex);
                }
            }

        }

        /// <summary>
        /// Updates material properties
        /// </summary>
        /// <param name="changed"></param>
        public void UpdateMaterial(bool resetKeywords = true)
        {
            if (resetKeywords)
            {
                ResetMaterialKeywords();
            }

            if (HasGradientShader())
            {
                SetMaterialFloat(ShaderProperties._AsOverlay, AsOverlay ? 1 : 0);
                SetMaterialColor(ShaderProperties._MainColor, graphic.color);
                SetMaterialFloat(ShaderProperties._Opacity, opacity);
                SetMaterialKeyword(_blendMode, true);
                SetMaterialFloat(ShaderProperties._AspectRatio, _imageSize.y / _imageSize.x);
                SetMaterialKeyword(Uv1keyword, useTexcoord1);

                if (_gradientStyle != GradientStyle.Vertex)
                {
                    SetMaterialKeyword(_gradientStyle, true);
                    SetMaterialFloat(ShaderProperties._Angle, _angle);
                    SetMaterialFloat(ShaderProperties._OffsetX, offset.x);
                    SetMaterialFloat(ShaderProperties._OffsetY, offset.y);
                    SetMaterialFloat(ShaderProperties._Scale, _scale);
                    SetMaterialTexture(ShaderProperties._GradientTex, _gradientMap);
                }
            }
            else
            {
                _gradientStyle = GradientStyle.Vertex;
            }
        }

        /// <summary>
        /// Assign and update material
        /// </summary>
        /// <param name="material"></param>
        public void UpdateMaterial(Material material)
        {
            graphic.material = material;
            UpdateMaterial();
        }

        /// <summary>
        /// Generates the texture map from the gradient field and assigns to the material
        /// </summary>
        public void UpdateGradientMap()
        {
            if (gradientStyle != GradientStyle.Vertex)
            {
                ForceUpdateGradientMat();
            }
        }

        /// <summary>
        /// Update gradient map regardless of style
        /// </summary>
        private void ForceUpdateGradientMat()
        {
            if (!HasGradientShader())
            {
                return;
            }
            resolution = Mathf.Max(minResolution, resolution);

            if (_gradientMap == null)
            {
                _gradientMap = new Texture2D(resolution, _texHeight);
                _gradientMap.wrapMode = TextureWrapMode.Clamp;
            }
            else
            {
                _gradientMap.Reinitialize(resolution, _texHeight);
            }

            if (gradient == null)
            {
                SetMaterialTexture(ShaderProperties._GradientTex, null);
                return;
            }
            else
            {
                for (int x = 0; x < resolution; x++)
                {
                    Color color = gradient.Evaluate((float)x / resolution);
                    for (int y = 0; y < _texHeight; y++)
                    {
                        _gradientMap.SetPixel(x, y, color);

                    }
                }
                _gradientMap.Apply();
            }

            //Saves map if required
#if UNITY_EDITOR
            bool saveMap = false;
            if (saveMap)
            {

                SaveMap(_gradientMap, graphic.material);
                return;
            }
#endif
            SetMaterialTexture(ShaderProperties._GradientTex, _gradientMap);
        }

        #endregion

        #region  HELPERS
        /// <summary>
        /// Get the final color with default blending for vertex gradient
        /// </summary>
        /// <param name="baseColor"></param>
        /// <param name="overlay"></param>
        /// <returns></returns>
        private Color GetColorWithDefaultBlending(Color baseColor, Color overlay)
        {
            Color color = overlay;
            if (!HasGradientShader())
            {
                overlay.a = Mathf.Lerp(0, overlay.a, _defaultModeOpacity);
                switch (defaultblendMode)
                {
                    case DefaultBlendModes.Normal:
                        color = Color.Lerp(baseColor, overlay, overlay.a);
                        break;
                    case DefaultBlendModes.Add:
                        color = Color.Lerp(baseColor, baseColor + overlay, overlay.a);
                        break;

                    case DefaultBlendModes.Multiply:
                        color = Color.Lerp(baseColor, baseColor * overlay, overlay.a);
                        break;
                }

                color.a = ((Color32)baseColor).a;
            }
            return color;
        }

        /// <summary>
        /// Get the shader keyword for gradient style from enum value
        /// </summary>
        /// <param name="gradientStyle"></param>
        /// <returns></returns>
        private string GetShaderKeyword(GradientStyle gradientStyle)
        {
            return "GRADIENT_" + gradientStyle.ToString().ToUpper();
        }

        /// <summary>
        /// Get the shader keyword for Blend mode from enum value
        /// </summary>
        /// <param name="blendMode"></param>
        /// <returns></returns>
        private string GetShaderKeyword(BlendModes blendMode)
        {
            return "BM_" + blendMode.ToString().ToUpper();
        }

        /// <summary>
        /// Is the current material using Gradient shader.
        /// Also responsible for applying and removing Gradient shader when required.
        /// </summary>
        /// <returns></returns>
        public bool HasGradientShader()
        {
            if (graphic == null)
            {
                Debug.LogWarning("UI Gradient: Graphic component Missing");
                return false;
            }

            //Gradientshader is cached in awake.
            //When disabled from the start, Triying to access this method will throw null refrence in applygradientshader
            //Example case: custom editor calls this method when gameobject is disabled
            if (gradientShader == null)
            {
                gradientShader = Shader.Find(GradientShaderPath);
            }

            //Checking the existence of a property to determine if it is gradient shader
            if (graphic.material.shader == gradientShader)
            {
                return true;
            }
            else
            {
                //If Fragment(Pixel) shader, then apply gradient shader
                if (gradientStyle != GradientStyle.Vertex || advancedVertexBlending)
                {
                    ApplyGradientshader();
                    return true;
                }
                //If advanced blending is turned off and style is vertex, change mat to default
                else if (!advancedVertexBlending & gradientStyle == GradientStyle.Vertex)
                {
                    if (graphic.material.shader != Shader.Find("UI/Default"))
                    {
                        graphic.material = null;
                        _mat = graphic.material;
                    }
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if this material is already used by another Gradient
        /// Two gradients using the same material can result in unexpected results
        /// </summary>
        /// <returns></returns>
        private bool isMaterialAlreadyUsed()
        {
            if (HasGradientShader())
            {
                UIGradient[] allGradients = FindObjectsOfType<UIGradient>();

                foreach (var item in allGradients)
                {
                    if (item.GetComponent<Graphic>().material == graphic.material && item != this)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Sets float property in material
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        private void SetMaterialFloat(ShaderProperties property, float value)
        {
            graphic.material.SetFloat(property.ToString(), value);
            if (isMaskingSupported && isMasked)
            {
                graphic.materialForRendering.SetFloat(property.ToString(), value);
            }
        }

        /// <summary>
        /// Sets color property in material
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        private void SetMaterialColor(ShaderProperties property, Color value)
        {
            graphic.material.SetColor(property.ToString(), value);
            if (isMaskingSupported && isMasked)
            {
                graphic.materialForRendering.SetColor(property.ToString(), value);
            }
        }

        /// <summary>
        /// Enables/disables blendmode keyword in material
        /// </summary>
        /// <param name="blendMode"></param>
        /// <param name="status"></param>
        private void SetMaterialKeyword(BlendModes blendMode, bool status)
        {
            string keyword = GetShaderKeyword(blendMode);
            SetMaterialKeyword(keyword, status);
        }

        /// <summary>
        /// Enables/disables Gradient style keyword in material
        /// </summary>
        /// <param name="gradientStyle"></param>
        /// <param name="status"></param>
        private void SetMaterialKeyword(GradientStyle gradientStyle, bool status)
        {
            string keyword = GetShaderKeyword(gradientStyle);
            SetMaterialKeyword(keyword, status);
        }

        /// <summary>
        /// Enables/disables a keyword in material
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="status"></param>
        public void SetMaterialKeyword(string keyword, bool status)
        {
            if (status)
            {
                graphic.material.EnableKeyword(keyword);
                if (isMaskingSupported && isMasked)
                {
                    graphic.materialForRendering.EnableKeyword(keyword);
                }
            }
            else
            {
                graphic.material.DisableKeyword(keyword);
                if (isMaskingSupported && isMasked)
                {
                    graphic.materialForRendering.DisableKeyword(keyword);
                }
            }
        }

        /// <summary>
        /// Sets texture in the material
        /// </summary>
        /// <param name="property"></param>
        /// <param name="texture"></param>
        private void SetMaterialTexture(ShaderProperties property, Texture texture)
        {
            graphic.material.SetTexture(property.ToString(), texture);
            if (isMaskingSupported && isMasked)
            {
                graphic.materialForRendering.SetTexture(property.ToString(), texture);
            }
        }
        #endregion

        /// <summary>
        /// Resets Material keywords to the selected one.
        /// </summary>
        public void ResetMaterialKeywords()
        {
            if (!HasGradientShader())
            {
                return;
            }
            string[] shaderGradientStyles = System.Array.ConvertAll(System.Enum.GetNames(typeof(GradientStyle)), d => "GRADIENT_" + d.ToUpper());
            string[] shaderBlendModes = System.Array.ConvertAll(System.Enum.GetNames(typeof(BlendModes)), d => "BM_" + d.ToUpper());

            //Disabling all the keywords except the select ones.
            bool status = false;
            for (int i = 0; i < shaderGradientStyles.Length; i++)
            {
                status = false;
                if (i == (int)_gradientStyle && _gradientStyle != GradientStyle.Vertex)
                {
                    status = true;
                }
                SetMaterialKeyword(shaderGradientStyles[i], status);
            }

            for (int i = 0; i < shaderBlendModes.Length; i++)
            {
                status = false;
                if (i == (int)_blendMode)
                {
                    status = true;
                }
                SetMaterialKeyword(shaderBlendModes[i], status);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Saves the gradient map in the material location
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="mat"></param>
        /// <returns></returns>
        private void SaveMap(Texture2D texture, Material mat)
        {
            string matPath = UnityEditor.AssetDatabase.GetAssetPath(mat);
            string alternatePath = "Assets/";
            if (string.IsNullOrEmpty(matPath))
            {
                matPath = alternatePath;
            }
            string path = matPath.Substring(0, matPath.LastIndexOf("/")) + "/" + gameObject.name + ".png";
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            UnityEditor.AssetDatabase.Refresh();

            if (HasGradientShader())
            {
                SetMaterialTexture(ShaderProperties._GradientTex, (Texture2D)UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)));
            }
        }

        /// <summary>
        /// Context menu
        /// Reset UI gradient component. 
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            if (gradientStyle == GradientStyle.Vertex && !advancedVertexBlending)
            {
                graphic.material = null;
            }
            InitGradient();
            SetVerticesDirty();
        }

        /// <summary>
        /// Context menu
        /// Paste component values custom implementation
        /// </summary>
        [ContextMenu("Paste Component Values")]
        void PasteComponentValues()
        {
            UnityEditorInternal.ComponentUtility.PasteComponentValues(this);
            UpdateGradientMap();
            UpdateMaterial();
            SetVerticesDirty();
            requireRepaint();
        }

        /// <summary>
        /// Context menu
        /// Refresh the material. Basically sets all the component values again.
        /// </summary>
        [ContextMenu("Refresh Material")]
        void RefreshMaterial()
        {
            UpdateMaterial();
        }



#endif
    }

    /// <summary>
    /// Enum for Gradient Styles
    /// </summary>
    public enum GradientStyle
    {
        Vertex,
        Linear,
        Radial,
        Angle,
        Reflected,
        Diamond
    }

    /// <summary>
    /// Enum for Blend Modes
    /// </summary>
    public enum BlendModes
    {
        Normal,
        Darken,
        Multiply,
        LinearBurn,
        ColorBurn,
        DarkerColor,
        Lighten,
        Screen,
        ColorDodge,
        LinearDodge,
        LightenColor,
        overlay,
        SoftLight,
        HardLight,
        VividLight,
        LinearLight,
        PinLight,
        HardMix,
        Difference,
        Exclusion,
        Subtract,
        Divide
    }

    /// <summary>
    /// Enum for Blend Modes in Default(Vertex) gradient with default UI shader
    /// </summary>
    public enum DefaultBlendModes
    {
        Normal,
        Multiply,
        Add
    }

    /// <summary>
    /// Gradient styles for Default(Vertex) gradients
    /// Corners : Apply different colors to all four corners.
    /// Linear : Start and End color, Direction : Horizontal. Vertical.
    /// </summary>
    public enum VertexGradientStyle
    {
        Corners,
        Linear
    }

    /// <summary>
    /// Gradient direction for Default(Vertex) Linear style
    /// </summary>
    public enum VertexGradientDirection
    {
        Horizontal,
        Vertical
    }

    /// <summary>
    /// Vertex mapped with the closest corner
    /// Used to color a vertex when in Default(Vertex) corner style.
    /// </summary>
    [System.Serializable]
    public class VertexCornerMap
    {
        //index of the vertex in vertex stream
        public int vertexIndex;
        //Closest corner
        public int cornerIndex;

        public VertexCornerMap(int vertexIndex, int cornerIndex)
        {
            this.vertexIndex = vertexIndex;
            this.cornerIndex = cornerIndex;
        }
    }
}