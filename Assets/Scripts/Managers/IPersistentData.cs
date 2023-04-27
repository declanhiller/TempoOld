namespace Managers {
    public interface IPersistentData<T> {
        T LoadNewData(T currentData);
    }
}