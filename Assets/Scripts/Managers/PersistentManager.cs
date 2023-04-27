using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers {
    public abstract class PersistentManager<T> : MonoBehaviour where T : PersistentManager<T> {

        static T _instance;
        public static T INSTANCE { 
            get
            {
                if (_instance == null) {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null) {
                        Debug.LogError("There is no instance of " + typeof(T));
                    }
                }
                return _instance;
            }
        }

        
        //TODO: Fancy reflection shit... I don't like reflection a lot but this approach can work... I think I'll stick with regular method extension for now
        //TLDR: basically finds all fields that are persistent data and trys to call the IPersistentData.LoadNewData method
        //using reflection and passes all the values of the instance it wants to transfer over
        //the more and more I think about this, this is a terrible way of doing it compared to having an abstract method
        //however, this does support private fields so could be an option in the future
        //on the other hand though, if your transfering data making it private doesn't seem great
        //as maybe you don't want a public get... whatever that's not a huge issue
        public void LoadNewData(PersistentManager<T> currentInstance, PersistentManager<T> instanceWithData) {
            List<FieldInfo> fieldInfos = currentInstance.GetType().GetFields().Where(f =>
                f.FieldType.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPersistentData<>))).ToList();
            foreach (FieldInfo fieldInfo in fieldInfos) {
                fieldInfo.GetValue(currentInstance).GetType().GetMethod("LoadNewData")
                    .Invoke(currentInstance, new []{fieldInfo.GetValue(instanceWithData)});
            }
        }

        protected virtual void LoadNewData(T newInstance) {
            
        }

        void Awake() {
            
            if (_instance != null) {
                // We've loaded back to a scene that already includes this
                // object! Destroy ourselves to let the existing instance stay.
                
                _instance.LoadNewData((T) this);
                
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            _instance = (T)this;
            
        }
    }
}