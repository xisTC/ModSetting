using Duckov.Modding;

namespace ModSetting.Config {
    public static class ModInfoExtension {
        public static string GetModId(this ModInfo modInfo) {
            return $"displayName:{modInfo.displayName};name:{modInfo.name};publishedFileId:{modInfo.publishedFileId}";
        }
    }
}