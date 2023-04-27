using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


public class AttackNode : Node {
    
    public string AttackName { get; set; }
    public string ActionName { get; set; }


    public void Initialize() {
        AttackName = "AttackName";
        ActionName = "Not Assigned";
    }

    public void Draw() {
        TextField attackNameTextField = new TextField() {
            value = AttackName
        };
        
        titleContainer.Insert(0, attackNameTextField);

        Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        inputPort.portName = "Attack Connection";
        inputContainer.Add(inputPort);
        


    }
    
}