using Xunit;
using ConstantManager.Services;
using System.Collections.Generic;

namespace ConstantManager.Tests
{
    public class ValidationServiceTests
    {
        // ========================================================================
        // UT-VS-01～05: PhysicalName バリデーション　
        // ========================================================================

        [Fact]
        public void UT_VS_01_PhysicalName_Valid_AlphanumericUnderscore()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidatePhysicalName("VALID_NAME_123");

            // Assert
            Assert.True(isValid);
            Assert.Null(errorMessage);
        }

        [Fact]
        public void UT_VS_02_PhysicalName_Invalid_Empty()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidatePhysicalName("");

            // Assert
            Assert.False(isValid);
            Assert.Contains("定数名(物理名)は必須", errorMessage);
        }

        [Fact]
        public void UT_VS_03_PhysicalName_Invalid_SpecialCharacters()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidatePhysicalName("INVALID@NAME");

            // Assert
            Assert.False(isValid);
            Assert.Contains("形式が不正", errorMessage);
        }

        [Fact]
        public void UT_VS_04_PhysicalName_Invalid_StartsWithNumber()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidatePhysicalName("123_INVALID");

            // Assert
            Assert.False(isValid);
            Assert.Contains("形式が不正", errorMessage);
        }

        [Fact]
        public void UT_VS_05_PhysicalName_Invalid_TooLong()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidatePhysicalName(new string('A', 33));

            // Assert
            Assert.False(isValid);
            Assert.Contains("長すぎます", errorMessage);
        }

        // ========================================================================
        // UT-VS-06～10: LogicalName バリデーション
        // ========================================================================

        [Fact]
        public void UT_VS_06_LogicalName_Valid()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidateLogicalName("論理名テスト");

            // Assert
            Assert.True(isValid);
            Assert.Null(errorMessage);
        }

        [Fact]
        public void UT_VS_07_LogicalName_Invalid_Empty()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidateLogicalName("");

            // Assert
            Assert.False(isValid);
            Assert.Contains("[E006]", errorMessage);
        }

        [Fact]
        public void UT_VS_08_LogicalName_Invalid_TooLong()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidateLogicalName(string.Empty.PadRight(65, 'あ'));

            // Assert
            Assert.False(isValid);
            Assert.Contains("長すぎます", errorMessage);
        }

        [Fact]
        public void UT_VS_09_LogicalName_Valid_MaxLength()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidateLogicalName(string.Empty.PadRight(64, 'あ'));

            // Assert
            Assert.True(isValid);
            Assert.Null(errorMessage);
        }

        [Fact]
        public void UT_VS_10_LogicalName_Valid_WithSpecialCharacters()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidateLogicalName("論理名(テスト)【重要】");

            // Assert
            Assert.True(isValid);
            Assert.Null(errorMessage);
        }

        // ========================================================================
        // UT-VS-11～15: Value バリデーション
        // ========================================================================

        [Fact]
        public void UT_VS_11_Value_Valid()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidateValue("100");

            // Assert
            Assert.True(isValid);
            Assert.Null(errorMessage);
        }

        [Fact]
        public void UT_VS_12_Value_Invalid_Empty()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidateValue("");

            // Assert
            Assert.False(isValid);
            Assert.Contains("値は必須", errorMessage);
        }

        [Fact]
        public void UT_VS_13_Value_Invalid_TooLong()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidateValue(new string('1', 257));

            // Assert
            Assert.False(isValid);
            Assert.Contains("長すぎます", errorMessage);
        }

        [Fact]
        public void UT_VS_14_Value_Valid_MaxLength()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidateValue(new string('1', 256));

            // Assert
            Assert.True(isValid);
            Assert.Null(errorMessage);
        }

        [Fact]
        public void UT_VS_15_Value_Valid_SpecialCharacters()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidateValue("値(100)@テスト");

            // Assert
            Assert.True(isValid);
            Assert.Null(errorMessage);
        }

        // ========================================================================
        // UT-VS-16～20: Unit バリデーション
        // ========================================================================

        [Fact]
        public void UT_VS_16_Unit_Valid()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidateUnit("km/h");

            // Assert
            Assert.True(isValid);
            Assert.Null(errorMessage);
        }

        [Fact]
        public void UT_VS_17_Unit_Valid_Empty()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidateUnit("");

            // Assert
            Assert.True(isValid); // 単位は任意項目
            Assert.Null(errorMessage);
        }

        [Fact]
        public void UT_VS_18_Unit_Invalid_TooLong()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidateUnit(new string('u', 17));

            // Assert
            Assert.False(isValid);
            Assert.Contains("長すぎます", errorMessage);
        }

        [Fact]
        public void UT_VS_19_Unit_Valid_MaxLength()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidateUnit(new string('u', 16));

            // Assert
            Assert.True(isValid);
            Assert.Null(errorMessage);
        }

        [Fact]
        public void UT_VS_20_Unit_Valid_SpecialCharacters()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidateUnit("℃/分");

            // Assert
            Assert.True(isValid);
            Assert.Null(errorMessage);
        }

        // ========================================================================
        // UT-VS-21～25: Description バリデーション
        // ========================================================================

        [Fact]
        public void UT_VS_21_Description_Valid()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidateDescription("説明文テスト");

            // Assert
            Assert.True(isValid);
            Assert.Null(errorMessage);
        }

        [Fact]
        public void UT_VS_22_Description_Valid_Empty()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidateDescription("");

            // Assert
            Assert.True(isValid); // 説明は任意項目
            Assert.Null(errorMessage);
        }

        [Fact]
        public void UT_VS_23_Description_Invalid_TooLong()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidateDescription(string.Empty.PadRight(257, '説'));

            // Assert
            Assert.False(isValid);
            Assert.Contains("長すぎます", errorMessage);
        }

        [Fact]
        public void UT_VS_24_Description_Valid_MaxLength()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidateDescription(string.Empty.PadRight(256, '説'));

            // Assert
            Assert.True(isValid);
            Assert.Null(errorMessage);
        }

        [Fact]
        public void UT_VS_25_Description_Valid_MultiLine()
        {
            // Arrange & Act
            var (isValid, errorMessage) = Validator.ValidateDescription("行1\n行2\n行3");

            // Assert
            Assert.True(isValid);
            Assert.Null(errorMessage);
        }

        // ========================================================================
        // UT-VS-26～28: 統合バリデーション
        // ========================================================================

        [Fact]
        public void UT_VS_26_ValidateConstantItem_AllValid()
        {
            // Arrange - ConstantItem コンストラクタを使用
            var item = new Models.ConstantItem("TEST_ITEM", "テスト項目", "100", "unit", "説明");

            // Act - 各フィールドを個別にバリデーション
            var physicalNameResult = Validator.ValidatePhysicalName(item.PhysicalName);
            var logicalNameResult = Validator.ValidateLogicalName(item.LogicalName);
            var valueResult = Validator.ValidateValue(item.Value);
            var unitResult = Validator.ValidateUnit(item.Unit);
            var descriptionResult = Validator.ValidateDescription(item.Description);

            // Assert
            Assert.True(physicalNameResult.IsValid);
            Assert.True(logicalNameResult.IsValid);
            Assert.True(valueResult.IsValid);
            Assert.True(unitResult.IsValid);
            Assert.True(descriptionResult.IsValid);
        }

        [Fact]
        public void UT_VS_27_ValidateConstantItem_MultipleErrors()
        {
            // Arrange - 不正な値でテスト
            var errors = new List<string>();

            // Act
            var physicalNameResult = Validator.ValidatePhysicalName("");
            var logicalNameResult = Validator.ValidateLogicalName("");
            var valueResult = Validator.ValidateValue("");

            if (!physicalNameResult.IsValid) errors.Add(physicalNameResult.ErrorMessage);
            if (!logicalNameResult.IsValid) errors.Add(logicalNameResult.ErrorMessage);
            if (!valueResult.IsValid) errors.Add(valueResult.ErrorMessage);

            // Assert
            Assert.True(errors.Count >= 3); // PhysicalName, LogicalName, Value
        }

        [Fact]
        public void UT_VS_28_ValidateConstantItem_BoundaryValues()
        {
            // Arrange
            var item = new Models.ConstantItem(
                new string('A', 32),
                new string('あ', 64),
                new string('1', 256),
                new string('u', 16),
                new string('説', 256)
            );

            // Act
            var physicalNameResult = Validator.ValidatePhysicalName(item.PhysicalName);
            var logicalNameResult = Validator.ValidateLogicalName(item.LogicalName);
            var valueResult = Validator.ValidateValue(item.Value);
            var unitResult = Validator.ValidateUnit(item.Unit);
            var descriptionResult = Validator.ValidateDescription(item.Description);

            // Assert
            Assert.True(physicalNameResult.IsValid);
            Assert.True(logicalNameResult.IsValid);
            Assert.True(valueResult.IsValid);
            Assert.True(unitResult.IsValid);
            Assert.True(descriptionResult.IsValid);
        }
    }
}