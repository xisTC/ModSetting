using Duckov.Modding;

namespace ModSetting.Extensions {
    public static class ModInfoExtension {
        public static string GetModId(this ModInfo modInfo) {
            return $"name:{modInfo.name};publishedFileId:{modInfo.publishedFileId}";
        }

        public static bool IsEmpty(this ModInfo modInfo) => string.IsNullOrEmpty(modInfo.name);
    }
}