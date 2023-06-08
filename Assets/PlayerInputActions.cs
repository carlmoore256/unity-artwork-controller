//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/PlayerInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInputActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputActions"",
    ""maps"": [
        {
            ""name"": ""Camera"",
            ""id"": ""dffcbf78-7361-431d-8888-43ccd826310f"",
            ""actions"": [
                {
                    ""name"": ""Translate"",
                    ""type"": ""Value"",
                    ""id"": ""c65f4044-1637-45db-ba5b-303e6c4eccde"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""68522e9a-087d-469a-94ad-0e5dba725db5"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Speed Modifier"",
                    ""type"": ""Value"",
                    ""id"": ""c9eea75c-0d0a-49c8-aa19-bfad975643c6"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Up"",
                    ""type"": ""Button"",
                    ""id"": ""10aebb07-db31-4424-8028-b9a54b74cd97"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Down"",
                    ""type"": ""Button"",
                    ""id"": ""027e173d-1eb0-4de3-82d0-56062b69f100"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Reset"",
                    ""type"": ""Button"",
                    ""id"": ""2f1b1fa7-69d3-494e-a77f-f32d7b6c85b5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""83da7a9b-e7ad-40d4-b61f-7033ef96d103"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PS4"",
                    ""action"": ""Translate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""081c2523-9c6f-47c5-9aeb-20f1b098161f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Translate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""4cc544e3-1775-45d6-b045-1c1991dd359e"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Translate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f18a834d-cef0-40e5-a783-dd9aaf6ab03a"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Translate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""671e92db-eb62-4836-8f90-82387a62815d"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Translate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""2ab3f7bc-ec5b-4585-9fa6-a25c7ff9e706"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Translate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""431e7a99-4e21-443f-9d15-021e6d4803a0"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PS4"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ab1be88d-4762-48a8-bbf8-5c5c3df98f23"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PS4"",
                    ""action"": ""Speed Modifier"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2c874013-7599-4d6c-bef2-d5d5aa468891"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Speed Modifier"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""685c8dbf-8f9b-4edc-a286-be2ab0a80245"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ba8587c4-a358-4f3c-980b-bc07a58eba77"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1c4f6762-75ab-4c5b-9d09-2264de446017"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b0ed560e-5e56-4536-b585-b453ca6c8498"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""302bcb97-f7b7-47eb-bb9f-eae99366b243"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Reset"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""PS4"",
            ""bindingGroup"": ""PS4"",
            ""devices"": []
        },
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": []
        }
    ]
}");
        // Camera
        m_Camera = asset.FindActionMap("Camera", throwIfNotFound: true);
        m_Camera_Translate = m_Camera.FindAction("Translate", throwIfNotFound: true);
        m_Camera_Look = m_Camera.FindAction("Look", throwIfNotFound: true);
        m_Camera_SpeedModifier = m_Camera.FindAction("Speed Modifier", throwIfNotFound: true);
        m_Camera_Up = m_Camera.FindAction("Up", throwIfNotFound: true);
        m_Camera_Down = m_Camera.FindAction("Down", throwIfNotFound: true);
        m_Camera_Reset = m_Camera.FindAction("Reset", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Camera
    private readonly InputActionMap m_Camera;
    private ICameraActions m_CameraActionsCallbackInterface;
    private readonly InputAction m_Camera_Translate;
    private readonly InputAction m_Camera_Look;
    private readonly InputAction m_Camera_SpeedModifier;
    private readonly InputAction m_Camera_Up;
    private readonly InputAction m_Camera_Down;
    private readonly InputAction m_Camera_Reset;
    public struct CameraActions
    {
        private @PlayerInputActions m_Wrapper;
        public CameraActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Translate => m_Wrapper.m_Camera_Translate;
        public InputAction @Look => m_Wrapper.m_Camera_Look;
        public InputAction @SpeedModifier => m_Wrapper.m_Camera_SpeedModifier;
        public InputAction @Up => m_Wrapper.m_Camera_Up;
        public InputAction @Down => m_Wrapper.m_Camera_Down;
        public InputAction @Reset => m_Wrapper.m_Camera_Reset;
        public InputActionMap Get() { return m_Wrapper.m_Camera; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CameraActions set) { return set.Get(); }
        public void SetCallbacks(ICameraActions instance)
        {
            if (m_Wrapper.m_CameraActionsCallbackInterface != null)
            {
                @Translate.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnTranslate;
                @Translate.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnTranslate;
                @Translate.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnTranslate;
                @Look.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnLook;
                @SpeedModifier.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnSpeedModifier;
                @SpeedModifier.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnSpeedModifier;
                @SpeedModifier.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnSpeedModifier;
                @Up.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnUp;
                @Up.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnUp;
                @Up.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnUp;
                @Down.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnDown;
                @Down.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnDown;
                @Down.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnDown;
                @Reset.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnReset;
                @Reset.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnReset;
                @Reset.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnReset;
            }
            m_Wrapper.m_CameraActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Translate.started += instance.OnTranslate;
                @Translate.performed += instance.OnTranslate;
                @Translate.canceled += instance.OnTranslate;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @SpeedModifier.started += instance.OnSpeedModifier;
                @SpeedModifier.performed += instance.OnSpeedModifier;
                @SpeedModifier.canceled += instance.OnSpeedModifier;
                @Up.started += instance.OnUp;
                @Up.performed += instance.OnUp;
                @Up.canceled += instance.OnUp;
                @Down.started += instance.OnDown;
                @Down.performed += instance.OnDown;
                @Down.canceled += instance.OnDown;
                @Reset.started += instance.OnReset;
                @Reset.performed += instance.OnReset;
                @Reset.canceled += instance.OnReset;
            }
        }
    }
    public CameraActions @Camera => new CameraActions(this);
    private int m_PS4SchemeIndex = -1;
    public InputControlScheme PS4Scheme
    {
        get
        {
            if (m_PS4SchemeIndex == -1) m_PS4SchemeIndex = asset.FindControlSchemeIndex("PS4");
            return asset.controlSchemes[m_PS4SchemeIndex];
        }
    }
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    public interface ICameraActions
    {
        void OnTranslate(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnSpeedModifier(InputAction.CallbackContext context);
        void OnUp(InputAction.CallbackContext context);
        void OnDown(InputAction.CallbackContext context);
        void OnReset(InputAction.CallbackContext context);
    }
}
