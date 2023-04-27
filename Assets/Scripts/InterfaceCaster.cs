using System;
using System.Collections.Generic;

public class InterfaceCaster {


    public static O[] CastArray<I, O>(I[] inputs) {
        O[] output = new O[inputs.Length];
        
        for (int i = 0; i < inputs.Length; i++) {
            if (!(inputs[i] is O castedInput)) {
                throw new NotImplementedException("Type " + typeof(I) + " is not castable to Type " + typeof(O));
            }
            output[i] = castedInput;
        }
        
        return output;
    }
}