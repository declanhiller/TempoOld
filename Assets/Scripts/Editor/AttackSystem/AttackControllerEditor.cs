using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UIElements;

public class AttackControllerEditor : EditorWindow {
    
    private SerializedProperty combos;

    private AttackController selectedAttackController;
    
    [SerializeField] private VisualTreeAsset inspectorXML;
    private List<string> actionNames;

    private void OnEnable() {
        // combos = serializedObject.FindProperty("combos");
        // //Load up actions
        // Component component = serializedObject.targetObject as Component;
        // if (component == null) {
        //     Debug.LogError("Could not find targeted object");
        //     return;
        // }
        //
        // GameObject attachedGameObj = component.gameObject;
        //
        // PlayerInput playerInput;
        //
        // if (!attachedGameObj.TryGetComponent<PlayerInput>(out playerInput)) {
        //     Debug.LogError("Could not find attached Player Input class on " + attachedGameObj.name);
        //     return;
        // }
        //
        // ReadOnlyArray<InputAction> readOnlyArray = playerInput.actions.actionMaps[0].actions;
        // actionNames = new List<string>();
        // foreach (InputAction action in readOnlyArray) {
        //     actionNames.Add(action.name);
        // }
        
    }

    // private void OnSelectionChange() {
    //     Transform selectedAsset = Selection.activeTransform;
    //     if (selectedAsset == null) return;
    //
    //     if (!selectedAsset.TryGetComponent(out AttackController attackController)) return;
    //
    //     selectedAttackController = attackController;
    // }

    public void GetNewAttack() {
        
    }

    [MenuItem("Window/Tempo/Attack Editor")]
    public static void ShowWindow() {
        GetWindow<AttackControllerEditor>("Attack Editor");
    }

    private void CreateGUI() {
        AddGraphView();
        AddStyles();
    }

    private void AddStyles() {
        StyleSheet styleSheet = (StyleSheet) EditorGUIUtility.Load("AttackEditorSystem/AttackEditorVariables.uss");
        rootVisualElement.styleSheets.Add(styleSheet);
    }

    private void AddGraphView() {
        AttackEditorGraphView graphView = new AttackEditorGraphView();
        
        graphView.StretchToParentSize();
        
        rootVisualElement.Add(graphView);
    }


    //
    // public override VisualElement CreateInspectorGUI() {
    //     VisualElement myInspector = new VisualElement();
    //     
    //     myInspector.Add(new Label("This is a custom inspector"));
    //
    //     myInspector.Add(new DropdownField("Action", actionNames, 0));
    //     
    //     // myInspector.Add(new UnityEve);
    //
    //     inspectorXML.CloneTree(myInspector);
    //     
    //     return myInspector;
    // }
}