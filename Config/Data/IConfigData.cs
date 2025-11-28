namespace ModSetting.Config.Data {
    public interface IConfigData {
        string Key { get;}
        string Description { get;}
        UIType UIType { get; }
        int Liveness { get; set; }
        T GetValue<T>();
    }

    public enum UIType {
        无,
        下拉列表,
        滑块,
        开关,
        按键绑定,
        输入框,
        按钮,
        标题,
        分组
    }
}