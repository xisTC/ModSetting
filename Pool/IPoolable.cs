namespace ModSetting.Pool {
    public interface IPoolable {
        void OnGet();
        void OnRelease();
        void OnDestroyObject();
    }
}