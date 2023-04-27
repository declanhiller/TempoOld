namespace Managers {
    public class NonDestructivePersistentData<T> : IPersistentData<T> {
        
        private T data;
        
        public T LoadNewData(T currentData) {
            if (currentData == null) return data;
            return currentData;
        }
    }
}