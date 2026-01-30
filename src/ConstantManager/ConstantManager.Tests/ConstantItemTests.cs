using Xunit;
using ConstantManager.Models;

namespace ConstantManager.Tests
{
    public class ConstantItemTests
    {
        // ========================================================================
        // UT-CI-01～03: コンストラクタテスト
        // ========================================================================

        [Fact]
        public void UT_CI_01_Constructor_Default()
        {
            // Act - ConstantItem はコンストラクタで全パラメータを指定
            var item = new ConstantItem("TEST_ITEM", "テスト項目", "100");

            // Assert
            Assert.NotNull(item);
            Assert.Equal("TEST_ITEM", item.PhysicalName);
            Assert.Equal("テスト項目", item.LogicalName);
            Assert.Equal("100", item.Value);
            Assert.Equal("", item.Unit); // デフォルト値
            Assert.Equal("", item.Description); // デフォルト値
            Assert.False(item.IsModified);
        }

        [Fact]
        public void UT_CI_02_Constructor_WithParameters()
        {
            // Act
            var item = new ConstantItem("TEST_ITEM", "テスト項目", "100", "unit", "説明");

            // Assert
            Assert.Equal("TEST_ITEM", item.PhysicalName);
            Assert.Equal("テスト項目", item.LogicalName);
            Assert.Equal("100", item.Value);
            Assert.Equal("unit", item.Unit);
            Assert.Equal("説明", item.Description);
        }

        [Fact]
        public void UT_CI_03_Constructor_IsDirtyInitialState()
        {
            // Arrange & Act
            var item = new ConstantItem("TEST_ITEM", "テスト", "100");

            // Assert
            Assert.False(item.IsModified);
        }

        // ========================================================================
        // UT-CI-04～06: Equals テスト
        // ========================================================================

        [Fact]
        public void UT_CI_04_Equals_SamePhysicalName()
        {
            // Arrange
            var item1 = new ConstantItem("TEST_ITEM", "項目1", "100");
            var item2 = new ConstantItem("TEST_ITEM", "項目2", "200");

            // Act & Assert
            Assert.True(item1.Equals(item2));
        }

        [Fact]
        public void UT_CI_05_Equals_DifferentPhysicalName()
        {
            // Arrange
            var item1 = new ConstantItem("ITEM_A", "項目A", "100");
            var item2 = new ConstantItem("ITEM_B", "項目B", "100");

            // Act & Assert
            Assert.False(item1.Equals(item2));
        }

        [Fact]
        public void UT_CI_06_Equals_Null()
        {
            // Arrange
            var item = new ConstantItem("TEST_ITEM", "テスト", "100");

            // Act & Assert
            Assert.False(item.Equals(null));
        }

        // ========================================================================
        // UT-CI-07: GetHashCode テスト
        // ========================================================================

        [Fact]
        public void UT_CI_07_GetHashCode_SamePhysicalName()
        {
            // Arrange
            var item1 = new ConstantItem("TEST_ITEM", "項目1", "100");
            var item2 = new ConstantItem("TEST_ITEM", "項目2", "200");

            // Act & Assert
            Assert.Equal(item1.GetHashCode(), item2.GetHashCode());
        }

        // ========================================================================
        // UT-CI-08～09: IsModified / MarkClean テスト
        // ========================================================================

        [Fact]
        public void UT_CI_08_IsModified_PropertyChanged()
        {
            // Arrange
            var item = new ConstantItem("TEST_ITEM", "テスト", "100");
            item.MarkClean();
            Assert.False(item.IsModified);

            // Act
            item.Value = "新しい値";

            // Assert
            Assert.True(item.IsModified);
        }

        [Fact]
        public void UT_CI_09_MarkClean_ClearsDirtyFlag()
        {
            // Arrange
            var item = new ConstantItem("TEST_ITEM", "テスト", "100");
            
            // Valueを変更してIsModifiedをtrueにする
            item.Value = "200";
            Assert.True(item.IsModified);

            // Act
            item.MarkClean();

            // Assert
            Assert.False(item.IsModified);
        }
    }
}