using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ConstantManager.Services
{
    public static class Validator
    {
        // Error and Warning Codes
        private const string ERROR_E005 = "E005";
        private const string ERROR_E006 = "E006";
        private const string ERROR_E007 = "E007";
        private const string ERROR_E008 = "E008";
        private const string ERROR_E009 = "E009";
        private const string WARNING_W003 = "W003";
        private const string WARNING_W004 = "W004";

        // Constraint Limits
        private const int PHYSICAL_NAME_MAX_LENGTH = 32;
        private const int LOGICAL_NAME_MAX_LENGTH = 64;
        private const int VALUE_MAX_LENGTH = 256;
        private const int UNIT_MAX_LENGTH = 16;
        private const int DESCRIPTION_MAX_LENGTH = 256;

        // Error Messages (仕様書 7.1 参照)
        private const string MSG_E005 = "[E005] 定数名(物理名)は必須項目です";
        private const string MSG_E006 = "[E006] 日本語名(論理名)は必須項目です、または長さが長すぎます(64文字以内)";
        private const string MSG_E007 = "[E007] 値は必須項目です、または長さが長すぎます(256文字以内)";
        private const string MSG_E008 = "[E008] 定数名(物理名)の形式が不正です(英大文字とアンダースコアのみ)";
        private const string MSG_E009 = "[E009] 定数名(物理名)が長すぎます(32文字以内)";
        private const string MSG_W003 = "[W003] 単位が長すぎます(16文字以内)";
        private const string MSG_W004 = "[W004] 説明が長すぎます(256文字以内)";

        /// <summary>
        /// 定数名(物理名)を検証します。
        /// - 必須(空文字NG) → E005
        /// - 形式(A-Z, _ のみ) → E008
        /// - 長さ(32文字以内) → E009
        /// </summary>
        public static (bool IsValid, string? ErrorMessage) ValidatePhysicalName(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return (false, MSG_E005);
            }

            if (!Regex.IsMatch(input, @"^[A-Z_]+$"))
            {
                return (false, MSG_E008);
            }

            if (input.Length > PHYSICAL_NAME_MAX_LENGTH)
            {
                return (false, MSG_E009);
            }

            return (true, null);
        }

        /// <summary>
        /// 日本語名(論理名)を検証します。
        /// - 必須 → E006
        /// - 長さ(64文字以内) → E006
        /// </summary>
        public static (bool IsValid, string? ErrorMessage) ValidateLogicalName(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return (false, MSG_E006);
            }

            if (input.Length > LOGICAL_NAME_MAX_LENGTH)
            {
                return (false, MSG_E006);
            }

            return (true, null);
        }

        /// <summary>
        /// 値を検証します。
        /// - 必須 → E007
        /// - 長さ(256文字以内) → E007
        /// </summary>
        public static (bool IsValid, string? ErrorMessage) ValidateValue(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return (false, MSG_E007);
            }

            if (input.Length > VALUE_MAX_LENGTH)
            {
                return (false, MSG_E007);
            }

            return (true, null);
        }

        /// <summary>
        /// 単位を検証します(省略可能)。
        /// - 長さ(16文字以内) → W003 (警告)
        /// </summary>
        public static (bool IsValid, string? ErrorMessage) ValidateUnit(string input)
        {
            // 単位は省略可能
            if (string.IsNullOrEmpty(input))
            {
                return (true, null);
            }

            if (input.Length > UNIT_MAX_LENGTH)
            {
                return (false, MSG_W003);
            }

            return (true, null);
        }

        /// <summary>
        /// 説明を検証します(省略可能)。
        /// - 長さ(256文字以内) → W004 (警告)
        /// </summary>
        public static (bool IsValid, string? ErrorMessage) ValidateDescription(string input)
        {
            // 説明は省略可能
            if (string.IsNullOrEmpty(input))
            {
                return (true, null);
            }

            if (input.Length > DESCRIPTION_MAX_LENGTH)
            {
                return (false, MSG_W004);
            }

            return (true, null);
        }
    }
}
