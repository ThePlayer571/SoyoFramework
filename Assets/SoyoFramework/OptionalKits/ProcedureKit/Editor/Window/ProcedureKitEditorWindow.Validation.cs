using System.Linq;

namespace SoyoFramework.OptionalKits.ProcedureKit.Editor.Window
{
    public partial class ProcedureKitEditorWindow
    {
        private void ValidateAll()
        {
            _validationErrors.Clear();
            _duplicateProcedureNames.Clear();
            _duplicateTagNames.Clear();
            _duplicateProcedureEnumValues.Clear();
            _duplicateTagEnumValues.Clear();
            _invalidProcedureNames.Clear();
            _invalidTagNames.Clear();

            // 检查重复的 ProcedureId 名称
            var procedureNames = _procedures.Select(p => p.Name).Where(n => !string.IsNullOrEmpty(n)).ToList();
            _duplicateProcedureNames = procedureNames
                .GroupBy(n => n)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToHashSet();

            foreach (var dup in _duplicateProcedureNames)
            {
                _validationErrors.Add($"重复的 ProcedureId 名称: \"{dup}\"");
            }

            // 检查重复的 ProcedureId EnumValue
            var procedureEnumValues = _procedures.Select(p => p.EnumValue).ToList();
            _duplicateProcedureEnumValues = procedureEnumValues
                .GroupBy(v => v)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToHashSet();

            foreach (var dup in _duplicateProcedureEnumValues)
            {
                _validationErrors.Add($"重复的 ProcedureId 枚举值: {dup}");
            }

            // 检查重复的 Tag 名称
            var tagNames = _tags.Select(t => t.Name).Where(n => !string.IsNullOrEmpty(n)).ToList();
            _duplicateTagNames = tagNames
                .GroupBy(n => n)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToHashSet();

            foreach (var dup in _duplicateTagNames)
            {
                _validationErrors.Add($"重复的 Tag 名称: \"{dup}\"");
            }

            // 检查重复的 Tag EnumValue
            var tagEnumValues = _tags.Select(t => t.EnumValue).ToList();
            _duplicateTagEnumValues = tagEnumValues
                .GroupBy(v => v)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToHashSet();

            foreach (var dup in _duplicateTagEnumValues)
            {
                _validationErrors.Add($"重复的 Tag 枚举值:  {dup}");
            }

            // 检查 ProcedureId 命名规范
            foreach (var proc in _procedures)
            {
                if (!IsValidIdentifier(proc.Name))
                {
                    _invalidProcedureNames.Add(proc.Name);
                    _validationErrors.Add($"无效的 ProcedureId:  \"{proc.Name}\" - {GetIdentifierErrorReason(proc.Name)}");
                }
            }

            // 检查 Tag 命名规范
            foreach (var tag in _tags)
            {
                if (!IsValidIdentifier(tag.Name))
                {
                    _invalidTagNames.Add(tag.Name);
                    _validationErrors.Add($"无效的 Tag:  \"{tag.Name}\" - {GetIdentifierErrorReason(tag.Name)}");
                }
            }

            // 检查 Region 3 的配置
            ValidateRegion3Settings();
        }

        private void ValidateRegion3Settings()
        {
            if (!string.IsNullOrEmpty(_namespace))
            {
                var namespaceParts = _namespace.Split('.');
                foreach (var part in namespaceParts)
                {
                    if (!IsValidIdentifier(part))
                    {
                        _validationErrors.Add($"无效的命名空间部分: \"{part}\" - {GetIdentifierErrorReason(part)}");
                    }
                }
            }

            if (!IsValidIdentifier(_procedureIdEnumName))
            {
                _validationErrors.Add(
                    $"无效的 ProcedureId 枚举名:  \"{_procedureIdEnumName}\" - {GetIdentifierErrorReason(_procedureIdEnumName)}");
            }

            if (!IsValidIdentifier(_procedureTagEnumName))
            {
                _validationErrors.Add(
                    $"无效的 ProcedureTag 枚举名: \"{_procedureTagEnumName}\" - {GetIdentifierErrorReason(_procedureTagEnumName)}");
            }

            if (_procedureIdEnumName == _procedureTagEnumName)
            {
                _validationErrors.Add("ProcedureId 枚举名和 ProcedureTag 枚举名不能相同");
            }
        }

        private bool IsValidIdentifier(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            if (!ValidIdentifierRegex.IsMatch(name))
                return false;

            if (CSharpKeywords.Contains(name))
                return false;

            return true;
        }

        private string GetIdentifierErrorReason(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "名称不能为空";

            if (char.IsDigit(name[0]))
                return "不能以数字开头";

            if (CSharpKeywords.Contains(name))
                return "不能使用C#关键字";

            if (!ValidIdentifierRegex.IsMatch(name))
                return "只能包含字母、数字和下划线，且必须以字母或下划线开头";

            return "未知错误";
        }

        private bool IsProcedureNameError(string name)
        {
            return _duplicateProcedureNames.Contains(name) || _invalidProcedureNames.Contains(name);
        }

        private bool IsProcedureEnumValueError(int enumValue)
        {
            return _duplicateProcedureEnumValues.Contains(enumValue);
        }

        private bool IsTagNameError(string name)
        {
            return _duplicateTagNames.Contains(name) || _invalidTagNames.Contains(name);
        }

        private bool IsTagEnumValueError(int enumValue)
        {
            return _duplicateTagEnumValues.Contains(enumValue);
        }
    }
}