using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Logger = ModSetting.Log.Logger;

namespace ModSetting.Config.Data {
public class ConfigDataReadConverter : JsonConverter<IConfigData>
{
    // 创建一个不包含当前转换器的静态序列化器，防止递归调用死循环
    private static readonly JsonSerializer cleanSerializer = new JsonSerializer
    {
        Formatting = Formatting.Indented,
    };
    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, IConfigData value, JsonSerializer serializer)
    {
        
    }

    public override IConfigData ReadJson(JsonReader reader, Type objectType, IConfigData existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        try
        {
            var jsonObject = JObject.Load(reader);
            
            // 方法1：使用 ToObject（推荐）
            UIType? uiType = null;
            try
            {
                uiType = jsonObject["UIType"]?.ToObject<UIType>(cleanSerializer);
            }
            catch (Exception ex)
            {
                Logger.Exception("使用 ToObject 解析 UIType 失败",ex);
            }
            
            // 方法2：手动处理（备用）
            if (!uiType.HasValue)
            {
                uiType = ParseUITypeManually(jsonObject["UIType"]);
            }
            
            // 方法3：根据属性推断（最后手段）
            if (!uiType.HasValue)
            {
                Logger.Error("转化UIType失败");
                return null;
            } 
            
            // 根据确定的 UIType 创建对象
            return CreateConfigData(uiType.Value, jsonObject, cleanSerializer);
        }
        catch (Exception ex)
        {
            Logger.Exception("反序列化 IConfigData 失败",ex);
            return null;
        }
    }
    
    private UIType? ParseUITypeManually(JToken uiTypeToken)
    {
        if (uiTypeToken == null) return null;
        
        switch (uiTypeToken.Type)
        {
            case JTokenType.Integer:
                int intValue = uiTypeToken.Value<int>();
                if (Enum.IsDefined(typeof(UIType), intValue))
                    return (UIType)intValue;
                break;
                    
            case JTokenType.String:
                string stringValue = uiTypeToken.Value<string>();
                if (Enum.TryParse(stringValue, out UIType parsedValue))
                    return parsedValue;
                break;
        }
        
        return null;
    }
    
    private IConfigData CreateConfigData(UIType uiType, JObject jsonObject, JsonSerializer serializer) {
        switch (uiType) {
            case UIType.下拉列表:
                return jsonObject.ToObject<DropDownConfigData>(serializer);
            case UIType.滑块:
                return jsonObject.ToObject<SliderConfigData>(serializer);
            case UIType.按钮:
                return jsonObject.ToObject<ToggleConfigData>(serializer);
            case UIType.按键绑定:
                return jsonObject.ToObject<KeyBindingConfigData>(serializer);
            case UIType.输入框:
                return jsonObject.ToObject<InputConfigData>(serializer);
            default:
                throw new JsonSerializationException($"未知的 UIType: {uiType}");
        }
    }
}
}