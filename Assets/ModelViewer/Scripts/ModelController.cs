using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using System;

[RequireComponent(typeof(Animator))]
public class ModelController : MonoBehaviour {
   
    public Animator m_Animator;
    protected AnimatorControllerParameter[] parameters;
    private Dictionary<string, AnimatorControllerParameter> paramByName;
   
    private void Awake()
    {

        if (m_Animator==null){
            m_Animator = gameObject.GetComponent<Animator>();                    
        }

        parameters = m_Animator.parameters;
        paramByName = new Dictionary<string, AnimatorControllerParameter>();

        foreach (AnimatorControllerParameter param in parameters)
        {
            paramByName[param.name] = param;
        }

	}

    // set animator parameters
    public void SetParam(string p_name, float p_val)
    {
        // set animator controller parameter. 
        // @p_name: parameter name as show in Animator Parameter list
        // @p_val: value to set param to; if bool use 1 or 0

        AnimatorControllerParameter parameter = paramByName[p_name];
        AnimatorControllerParameterType p_type = parameter.type;

        switch (p_type)
        {

            case AnimatorControllerParameterType.Trigger:
                m_Animator.SetTrigger(p_name);
                break;

            case AnimatorControllerParameterType.Float:
                m_Animator.SetFloat(p_name, p_val);
                break;

            case AnimatorControllerParameterType.Int:
                int state = (int)p_val;
                m_Animator.SetInteger(p_name, state);

                break;

            case AnimatorControllerParameterType.Bool:
                bool b = (Mathf.Approximately(p_val, 1)) ? true : false;
                m_Animator.SetBool(p_name, b);
                break;

            default:
                break;

        }

        switch (p_name)
        {
            case "START":
                Reset();
                break;
            
            default:
                break;

        }

        //BroadcastMessage("ParameterUpdated" , p_name);

    }

    public void SetLayer(string animLayer, float amt)
    {
        //@animLayer - name of animation layer
        //@amt - amount to set weight to. {0...1}
        //  smooth transition using coroutine
        StartCoroutine(LerpSetLayer(animLayer, Mathf.Clamp(amt, 0, 1), 1));
    }

    private IEnumerator LerpSetLayer(string layerN, float end, float dT)
    {
        // @layerN : layer name
        // @end : float between 0, 1 for layer amount 
        // @dT : delta time to go from current amount to end amount

        int n = m_Animator.GetLayerIndex(layerN);
        float start = m_Animator.GetLayerWeight(n);

        float elapseT = 0;

        while (elapseT < dT)
        {

            m_Animator.SetLayerWeight(n, Mathf.Lerp(start, end, elapseT / dT));
            elapseT += Time.deltaTime;

            yield return null;

        }

    }

    public void SetSkinBlendAmount(float i){
        // @i 0(worse) - 1(initial) - 2(better), amount to blend skin materials from Materal 1 to Material 2
        // require "cn_body_geGeo"'s materials to use "Custom/SkinBlend" shader (asset in SharedMaterials SSS Skin Blend)

        float amt = i / 2; // Debug.Log(i + ", " + amt);
        Shader.SetGlobalFloat("_SkinTextureBlend", Mathf.Clamp(amt, 0, 1) );

    }

    // ==== RESETS ==== //
    public void Reset()
    {
        BroadcastMessage("ResetAll", SendMessageOptions.DontRequireReceiver);

        // reset animator
        ResetParameters();
        m_Animator.SetTrigger("RESET");

    }

    private void ResetParameters()
    {

        for (int i = 0; i < parameters.Length; i++)
        {
            AnimatorControllerParameter parameter = parameters[i];
            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Int:
                    m_Animator.SetInteger(parameter.name, parameter.defaultInt);
                    break;
                case AnimatorControllerParameterType.Float:
                    m_Animator.SetFloat(parameter.name, parameter.defaultFloat);
                    break;
                case AnimatorControllerParameterType.Bool:
                    m_Animator.SetBool(parameter.name, parameter.defaultBool);
                    break;
            }
        }
    }

}

