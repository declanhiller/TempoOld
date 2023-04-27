using System;
using UnityEngine;

namespace Managers {
    
    [Serializable]
    public class DestructivePersistentData<T> : IPersistentData<T> {
        
        [SerializeField] private T data;
        
        public T LoadNewData(T currentData) {
            return data;
        }
        
    }
}