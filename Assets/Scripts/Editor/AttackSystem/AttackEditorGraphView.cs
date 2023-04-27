using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class AttackEditorGraphView : GraphView {
    public AttackEditorGraphView() {
        AddManipulators();
        AddGridBackground();

        CreateNode();
        
        AddStyles();
    }

    private void CreateNode() {
        AttackNode attackNode = new AttackNode();
        AddElement(attackNode);
    }

    private void AddManipulators() {
        
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        this.AddManipulator(new ContentDragger());
    }

    private void AddGridBackground() {
        GridBackground gridBackground = new GridBackground();
        gridBackground.StretchToParentSize();
        Insert(0, gridBackground);
    }

    private void AddStyles() {
        StyleSheet styleSheet = (StyleSheet) EditorGUIUtility.Load("AttackEditorSystem/AttackGraphViewStyles.uss");
        styleSheets.Add(styleSheet);
    }
}