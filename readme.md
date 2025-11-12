## 0.4.0 更新：

1. 增加多语言切换
2. 修改mod保存文件的id(因为有些mod会修改displayName，所以修改id)
3. 移除dll中的ModSettingAPI类(防止有人引用dll来调用ModSettingAPI)
4. 增加SettingsBuilder作为引用dll调用
5. 调整mod名字的显示，使其不会和mod图片重叠。
6. bug:  
修复slider添加到group后调整为最大值。  
修复游戏中启用mod后输入框有多余的物体。

## 使用指南:

1. 使用ModSettingAPI.cs来添加UI  
复制ModSettingAPI.cs到项目中，调用内部函数添加UI。[示例](https://github.com/xisTC/ModSettingTest)  
**好处**:mod和ModSetting之间不是强依赖，启用任何一个都能运行,并且不关心启用顺序。  
**应用场景**：添加UI设置，并且希望能独立运行的mod
2. 引用ModSetting.dll来添加UI  
引用ModSetting.dll到项目中，调用SettingsBuilder来添加UI。[示例](https://github.com/xisTC/ModSettingTest2)  
**好处**：不使用反射调用，性能更好。  
**坏处**：mod和ModSetting之间强依赖，必须先启用ModSetting才能启用mod，也就是mod必须放在ModSetting下面  
**应用场景**：使用了ModSetting的保存系统，有极高性能要求
3. 注意:  
已知的bug：如果使用引用dll的mod的前置mod未启用，例如:HarmonyLib、引用dll使用的ModSetting。这个时候开启游戏，在mod列表启用前置mod未启用的mod(且此mod是引用的前置mod的dll)，将会报错，很正常，因为前置mod未启用，程序集中找不到mod中使用的类型，此时如果启用其他任何mod，会发现勾选不上mod，控制台报错，但是实际上mod其实已经启用了。关闭mod列表再打开mod列表就会发现mod已经勾上了，并且mod的功能也能正常使用。此bug会让用户认为你的mod导致其他所有mod都无法勾选上。