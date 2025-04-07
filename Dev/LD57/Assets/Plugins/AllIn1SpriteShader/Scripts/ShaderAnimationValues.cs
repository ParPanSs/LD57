using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ShaderAnimationValues : MonoBehaviour
{
    [SerializeField]
    private Material m_Material;

    public Color protectionColor;
    public Color healingColor;

    [Header("1. Glow")]
    public Color glowColor;
    private Color _glowColorActualValue;
    private Color _glowColorStartValue;
    [Range(0f, 100f)]
    public float glow;
    private float _glowActualValue;
    private float _glowStartValue;

    [Header("2. Fade")]
    [Range(-0.1f, 1f)]
    public float fade;
    private float _fadeActualValue;
    private float _fadeStartValue;
    public Color fadeColor;
    private Color _fadeColorActualValue;
    private Color _fadeColorStartValue;

    [Header("3. Outline")]
    [Range(0f, 1f)]
    // Alpha
    public float outlineAlpha;
    private float _outlineActualValue;
    private float _outlineAlphaStartValue;
    // Color
    public Color outlineColor;
    private Color _outlineColorActualValue;
    private Color _outlineColorStartValue;

    [Header("5. Inner Outline")]
    [Range(0f, 250f)]
    // Glow
    public float innerOutlineGlow;
    private float _innerOutlineGlowActualValue;
    private float _innerOutlineGlowAlphaStartValue;

    [Header("12. Negative")]
    [Range(0, 1)]
    public float negative;
    private float _negativeActualValue;
    private float _negativeStartValue;

    [Header("24. Shine")]
    // Color
    public Color shineColor;
    private Color _shineColorActualValue;
    private Color _shineColorStartValue;
    // Location
    [Range(0f, 1f)]
    public float shineLocation;
    private float _shineLocationActualValue;
    private float _shineLocationStartValue;

    [Header("30. Grass Movement / Wind")]
    // Manual Animation
    [Range(-1f, 1f)]
    public float grassManualAnim;
    private float _grassManualAnimActualValue;
    private float _grassManualAnimStartValue;

    [Header("38. Distortion")]
    // Amount
    [Range(0, 2)]
    public float distortAmount;
    private float _distortAmountAnimActualValue;
    private float _distortAmountAnimStartValue;
    // X Speed
    [Range(-50, 50)]
    public float distortTexXSpeed;
    private float _distortTexXAnimActualValue;
    private float _distortTexXAnimStartValue;
    // Y Speed
    [Range(-50, 50)]
    public float distortTexYSpeed;
    private float _distortTexYAnimActualValue;
    private float _distortTexYAnimStartValue;

    private Image _image;

    private void Awake()
    {
        if (GetComponent<Image>())
        {
            _image = GetComponent<Image>();
            if(m_Material == null)
            {
            _image.material = new Material(_image.material);
            }
            else
            {
                _image.material = new Material(m_Material);
            }
        }

        // GLOW
        
        _glowColorActualValue = _image.material.GetColor("_GlowColor");
        _glowColorStartValue = glowColor;
        _glowActualValue = _image.material.GetFloat("_Glow");
        _glowStartValue = glow;

        // FADE
        _fadeActualValue = _image.material.GetFloat("_FadeAmount");
        _fadeStartValue = fade;
        _fadeColorActualValue = _image.material.GetColor("_FadeBurnColor");
        _fadeColorStartValue = fadeColor;

        // OUTLINE
        // Alpha
        _outlineActualValue = _image.material.GetFloat("_OutlineAlpha");
        _outlineAlphaStartValue = outlineAlpha;
        // Color
        _outlineColorActualValue = _image.material.GetColor("_OutlineColor");
        _outlineColorStartValue = outlineColor;

        // INNER OUTLINE
        // Glow
        _innerOutlineGlowActualValue = _image.material.GetFloat("_InnerOutlineGlow");
        _innerOutlineGlowAlphaStartValue = innerOutlineGlow;

        // NEGATIVE
        _negativeActualValue = _image.material.GetFloat("_ColorRampBlend");
        _negativeStartValue = negative;

        // SHINE
        // Color
        _shineColorActualValue = _image.material.GetColor("_ShineColor");
        _shineColorStartValue = shineColor;
        // Location
        _shineLocationActualValue = _image.material.GetFloat("_ShineLocation");
        _shineLocationStartValue = shineLocation;

        // GRASS MOVEMENT / WIND
        _grassManualAnimActualValue = _image.material.GetFloat("_GrassManualAnim");
        _grassManualAnimStartValue = grassManualAnim;

        // DISTORTION
        // Amount
        _distortAmountAnimActualValue = _image.material.GetFloat("_DistortAmount");
        _distortAmountAnimStartValue = distortAmount;
        // X Speed
        _distortTexXAnimActualValue = _image.material.GetFloat("_DistortTexXSpeed");
        _distortTexXAnimStartValue = distortTexXSpeed;
        // Y Speed
        _distortTexYAnimActualValue = _image.material.GetFloat("_DistortTexYSpeed");
        _distortTexYAnimStartValue = distortTexYSpeed;
    }

    private void LateUpdate()
    {
        // GLOW
        if (glowColor != _glowColorActualValue)
        {
            _glowColorActualValue = glowColor;

            if (_image != null)
            {
                _image.material.SetColor("_GlowColor", _glowColorActualValue);
            }
        }
        if (glow != _glowActualValue)
        {
            _glowActualValue = glow;

            if (_image != null) 
            {
                _image.material.SetFloat("_Glow", _glowActualValue);
            }
        }

        // FADE
        if (fade != _fadeActualValue)
        {
            _fadeActualValue = fade;

            if (_image != null)
            {
                _image.material.SetFloat("_FadeAmount", _fadeActualValue);
            }       
        }
        if (fadeColor != _fadeColorActualValue)
        {
            _fadeColorActualValue = fadeColor;

            if (_image != null)
            {
                _image.material.SetColor("_FadeBurnColor", _fadeColorActualValue);
            }
        }

        // OUTLINE
        // Alpha
        if (outlineAlpha != _outlineActualValue)
        {
            _outlineActualValue = outlineAlpha;

            if (_image != null)
            {
                _image.material.SetFloat("_OutlineAlpha", _outlineActualValue);
            }
        }
        // Color
        if (outlineColor != _outlineColorActualValue)
        {
            _outlineColorActualValue = outlineColor;

            if (_image != null)
            {
                _image.material.SetColor("_OutlineColor", _outlineColorActualValue);
            }
        }

        // INNER OUTLINE
        // Glow
        if (innerOutlineGlow != _innerOutlineGlowActualValue)
        {
            _innerOutlineGlowActualValue = innerOutlineGlow;

            if (_image != null)
            {
                _image.material.SetFloat("_InnerOutlineGlow", _innerOutlineGlowActualValue);
            }
        }

        // NEGATIVE
        if (negative != _negativeActualValue)
        {
            _negativeActualValue = negative;

            if (_image != null)
            {
                _image.material.SetFloat("_ColorRampBlend", _negativeActualValue);
            }
        }

        // SHINE
        // Color 
        if (shineColor != _shineColorActualValue)
        {
            _shineColorActualValue = shineColor;

            if (_image != null)
            {
                _image.material.SetColor("_ShineColor", _shineColorActualValue);
            }
        }

        // Location
        if (shineLocation != _shineLocationActualValue)
        {
            _shineLocationActualValue = shineLocation;

            if (_image != null)
            {
                _image.material.SetFloat("_ShineLocation", _shineLocationActualValue);
            }        
        }

        // GRASS MOVEMENT / WIND
        if (grassManualAnim != _grassManualAnimActualValue)
        {
            _grassManualAnimActualValue = grassManualAnim;

            if (_image != null)
            {
                _image.material.SetFloat("_GrassManualAnim", _grassManualAnimActualValue);
            }
        }

        // DISTORTION
        // Amount
        if (distortAmount != _distortAmountAnimActualValue)
        {
            _distortAmountAnimActualValue = distortAmount;

            if (_image != null)
            {
                _image.material.SetFloat("_DistortAmount", _distortAmountAnimActualValue);
            }
        }
        // X Speed
        if (distortTexXSpeed != _distortTexXAnimActualValue)
        {
            _distortTexXAnimActualValue = distortTexXSpeed;

            if (_image != null)
            {
                _image.material.SetFloat("_DistortTexXSpeed", _distortTexXAnimActualValue);
            }
        }
        // Y Speed
        if (distortTexYSpeed != _distortTexYAnimActualValue)
        {
            _distortTexYAnimActualValue = distortTexYSpeed;

            if (_image != null)
            {
                _image.material.SetFloat("_DistortTexYSpeed", _distortTexYAnimActualValue);
            }
        }
    }

    private void OnApplicationQuit()
    {
        Reset();
    }

    public void ShineColorHealing()
    {
        shineColor = healingColor;
    }

    public void ShineColorProtection()
    {
        shineColor = protectionColor;
    }

    public void Reset()
    {
        fade = _fadeStartValue;
        fadeColor = _fadeColorStartValue;
        glow = _glowStartValue;
        glowColor = _glowColorStartValue;
        outlineAlpha = _outlineAlphaStartValue;
        outlineColor = _outlineColorStartValue;
        shineLocation = _shineLocationStartValue;
        shineColor = _shineColorStartValue;
        grassManualAnim = _grassManualAnimStartValue;
        distortAmount = _distortAmountAnimStartValue;
        distortTexXSpeed = _distortTexXAnimStartValue;
        distortTexYSpeed = _distortTexYAnimStartValue;
        negative = _negativeStartValue;
    }
}
