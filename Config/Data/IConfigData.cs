namespace ModSetting.Config.Data {
    public interface IConfigData {
        string Key { get;}
        string Description { get;}
        UIType UIType { get; }
        T GetValue<T>();
    }

    public enum UIType {
        无,
        下拉列表,
        滑块,
        按钮,
        按键绑定,
        输入框
    }
}