namespace ModSetting.Config.Data {
    public interface IConfigData {
        string Key { get;}
        string Description { get;}
        UIType UIType { get; }
        object GetValue();
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