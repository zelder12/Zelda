using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour
{
    public Character CharacterScriptable;

    public Sprite leftHand;
    public Sprite rightHand;
    public Sprite leftHandUsing;
    public Sprite rightHandUsing;
    void Awake()
    {
        if (CharacterScriptable != null)
        {
            leftHand = CharacterScriptable.leftHand;
            rightHand = CharacterScriptable.rightHand;
            leftHandUsing = CharacterScriptable.leftHandUsing;
            rightHandUsing = CharacterScriptable.rightHandUsing;
        }
    }
    void Start()
    {
        
    }
    void Update()
    {

    }

    Sprite cambiarLeftHand(bool utilizado)
    {
        if (utilizado)
        {
            return leftHandUsing;
        }
        else
        {
            return leftHand;
        }
    }
    Sprite cambiarRightHand(bool utilizado)
    {
        if (utilizado)
        {
            return rightHandUsing;
        }
        else
        {
            return rightHand;
        }
    }
}
