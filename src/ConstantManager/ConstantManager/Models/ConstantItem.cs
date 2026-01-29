using System;
using System.Text.RegularExpressions;

namespace ConstantManager.Models
{
    /// <summary>
    /// 定数アイテムを表すデータモデルクラス。
    /// PhysicalName を主キーとして扱い、変更不可とします。
    /// </summary>
    public class ConstantItem
    {
        // 正規表現: PhysicalName の形式チェック（英大文字、数字、アンダースコア。先頭は英大文字またはアンダースコアのみ）
        private static readonly Regex PhysicalNameRegex = 
            new Regex("^[A-Z_][A-Z0-9_]*$", RegexOptions.Compiled);

        // 制約値
        private const int MaxPhysicalNameLength = 32;
        private const int MaxLogicalNameLength = 64;
        private const int MaxValueLength = 256;
        private const int MaxUnitLength = 16;
        private const int MaxDescriptionLength = 256;

        // プライベートフィールド
        private readonly string _physicalName;
        private string _logicalName;
        private string _value;
        private string _unit;
        private string _description;
        private bool _isModified;

        /// <summary>
        /// ConstantItem を初期化します。
        /// </summary>
        /// <param name="physicalName">定数名（物理名）。英大文字・アンダースコアのみ、最大32文字。</param>
        /// <param name="logicalName">日本語名（論理名）。最大64文字。</param>
        /// <param name="value">値。最大256文字。</param>
        /// <param name="unit">単位（省略可能）。最大16文字。</param>
        /// <param name="description">説明（省略可能）。最大256文字。</param>
        /// <exception cref="ArgumentException">PhysicalName が空または形式が不正な場合。</exception>
        public ConstantItem(
            string physicalName,
            string logicalName,
            string value,
            string unit = "",
            string description = "")
        {
            // PhysicalName の検証
            if (string.IsNullOrWhiteSpace(physicalName))
            {
                throw new ArgumentException(
                    "PhysicalName cannot be null or empty.",
                    nameof(physicalName));
            }

            if (physicalName.Length > MaxPhysicalNameLength)
            {
                throw new ArgumentException(
                    $"PhysicalName length exceeds maximum ({MaxPhysicalNameLength} characters).",
                    nameof(physicalName));
            }

            if (!PhysicalNameRegex.IsMatch(physicalName))
            {
                throw new ArgumentException(
                    "PhysicalName must contain only uppercase letters (A-Z) and underscores (_).",
                    nameof(physicalName));
            }

            // Value の検証
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(
                    "Value cannot be null or empty.",
                    nameof(value));
            }

            // フィールドの初期化
            _physicalName = physicalName;
            _logicalName = logicalName ?? "";
            _value = value ?? "";
            _unit = unit ?? "";
            _description = description ?? "";
            _isModified = false;
        }

        /// <summary>
        /// 定数名（物理名）を取得します。読み取り専用。
        /// 主キーとして機能し、作成後は変更できません。
        /// </summary>
        public string PhysicalName => _physicalName;

        /// <summary>
        /// 日本語名（論理名）を取得または設定します。
        /// 設定時に自動的に IsModified フラグが true になります。
        /// </summary>
        public string LogicalName
        {
            get => _logicalName;
            set
            {
                if (_logicalName != value)
                {
                    _logicalName = value ?? "";
                    _isModified = true;
                }
            }
        }

        /// <summary>
        /// 値を取得または設定します。
        /// 設定時に自動的に IsModified フラグが true になります。
        /// </summary>
        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value ?? "";
                    _isModified = true;
                }
            }
        }

        /// <summary>
        /// 単位を取得または設定します。
        /// 省略可能フィールドです。
        /// 設定時に自動的に IsModified フラグが true になります。
        /// </summary>
        public string Unit
        {
            get => _unit;
            set
            {
                if (_unit != value)
                {
                    _unit = value ?? "";
                    _isModified = true;
                }
            }
        }

        /// <summary>
        /// 説明を取得または設定します。
        /// 省略可能フィールドです。複数行対応。
        /// 設定時に自動的に IsModified フラグが true になります。
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value ?? "";
                    _isModified = true;
                }
            }
        }

        /// <summary>
        /// このアイテムが編集されたかどうかを示します。
        /// LogicalName, Value, Unit, Description の変更で自動的に true になります。
        /// 保存後は MarkClean() で false にリセットされます。
        /// </summary>
        public bool IsModified
        {
            get => _isModified;
            set => _isModified = value;
        }

        /// <summary>
        /// 編集状態をリセットします。
        /// 通常、CSV保存後に呼び出されます。
        /// </summary>
        public void MarkClean()
        {
            _isModified = false;
        }

        /// <summary>
        /// 2つの ConstantItem が同じ PhysicalName を持つかどうかを判定します。
        /// PhysicalName が主キーであるため、PhysicalName の値で同一性を判定します。
        /// </summary>
        /// <param name="obj">比較対象のオブジェクト</param>
        /// <returns>PhysicalName が同じ場合は true</returns>
        public override bool Equals(object obj)
        {
            if (obj is not ConstantItem other)
            {
                return false;
            }

            return this.PhysicalName == other.PhysicalName;
        }

        /// <summary>
        /// PhysicalName に基づいてハッシュコードを計算します。
        /// リスト内での重複排除やディクショナリのキーとして使用されます。
        /// </summary>
        /// <returns>PhysicalName のハッシュコード</returns>
        public override int GetHashCode()
        {
            return _physicalName?.GetHashCode() ?? 0;
        }

        /// <summary>
        /// このオブジェクトの文字列表現を返します。
        /// 形式: [PhysicalName] LogicalName = Value Unit
        /// </summary>
        /// <returns>文字列表現</returns>
        public override string ToString()
        {
            return $"[{_physicalName}] {_logicalName} = {_value} {_unit}";
        }
    }
}
