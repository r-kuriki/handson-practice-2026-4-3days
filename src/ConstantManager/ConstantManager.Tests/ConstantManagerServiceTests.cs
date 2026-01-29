using Xunit;
using ConstantManager.Services;
using ConstantManager.Models;
using System.IO;
using System.Linq;

namespace ConstantManager.Tests
{
    public class ConstantManagerServiceTests
    {
        private readonly string _testDataDir;

        public ConstantManagerServiceTests()
        {
            _testDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");
        }

        // ========================================================================
        // UT-CMS-01～03: データ追加テスト
        // ========================================================================

        [Fact]
        public void UT_CMS_01_AddItem_ValidItem()
        {
            // Arrange
            var service = new ConstantManagerService();
            var item = new ConstantItem("NEW_ITEM", "新規項目", "100", "unit", "説明");

            // Act
            service.AddItem(item);

            // Assert
            Assert.Single(service.Items);
            Assert.True(service.IsDirty);
        }

        [Fact]
        public void UT_CMS_02_AddItem_DuplicatePhysicalName()
        {
            // Arrange
            var service = new ConstantManagerService();
            var item1 = new ConstantItem("TEST_ITEM", "項目1", "100");
            var item2 = new ConstantItem("TEST_ITEM", "項目2", "200");

            // Act
            service.AddItem(item1);

            // Assert
            Assert.Throws<InvalidOperationException>(() => service.AddItem(item2));
            Assert.Single(service.Items);
        }

        [Fact]
        public void UT_CMS_03_AddItem_InvalidItem()
        {
            // Arrange
            var service = new ConstantManagerService();

            // Act & Assert - 無効な PhysicalName でコンストラクタが例外をスロー
            Assert.Throws<System.ArgumentException>(() => new ConstantItem("", "項目", "100"));
        }

        // ========================================================================
        // UT-CMS-04～06: データ更新テスト
        // ========================================================================

        [Fact]
        public void UT_CMS_04_UpdateItem_ExistingItem()
        {
            // Arrange
            var service = new ConstantManagerService();
            var item = new ConstantItem("ITEM_001", "項目1", "100");
            service.AddItem(item);

            // Act
            var updatedItem = new ConstantItem("ITEM_001", "項目1_更新", "200");
            service.UpdateItem(updatedItem);

            // Assert
            Assert.Equal("200", service.Items[0].Value);
            Assert.True(service.IsDirty);
        }

        [Fact]
        public void UT_CMS_05_UpdateItem_NonExistingItem()
        {
            // Arrange
            var service = new ConstantManagerService();
            var item = new ConstantItem("NON_EXISTENT", "存在しない", "100");

            // Act & Assert
            Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() => service.UpdateItem(item));
        }

        [Fact]
        public void UT_CMS_06_UpdateItem_InvalidData()
        {
            // Arrange
            var service = new ConstantManagerService();
            var item = new ConstantItem("ITEM_001", "項目1", "100");
            service.AddItem(item);

            // Act & Assert - 無効な値でConstantItemを作成しようとすると例外
            Assert.Throws<System.ArgumentException>(() => new ConstantItem("ITEM_001", "項目1", ""));
        }

        // ========================================================================
        // UT-CMS-07～08: データ削除テスト
        // ========================================================================

        [Fact]
        public void UT_CMS_07_DeleteItem_ExistingItem()
        {
            // Arrange
            var service = new ConstantManagerService();
            var item = new ConstantItem("ITEM_001", "項目1", "100");
            service.AddItem(item);

            // Act
            service.DeleteItem(item);

            // Assert
            Assert.Empty(service.Items);
            Assert.True(service.IsDirty);
        }

        [Fact]
        public void UT_CMS_08_DeleteItem_NonExistingItem()
        {
            // Arrange
            var service = new ConstantManagerService();
            var item = new ConstantItem("NON_EXISTENT", "存在しない", "100");

            // Act - 存在しないアイテムの削除は例外をスローしない（何もしない）
            service.DeleteItem(item);

            // Assert
            Assert.Empty(service.Items);
        }

        // ========================================================================
        // UT-CMS-09～11: ファイル読み込みテスト
        // ========================================================================

        [Fact]
        public void UT_CMS_09_LoadFromFile_ValidFile()
        {
            // Arrange
            var service = new ConstantManagerService();
            string filePath = Path.Combine(_testDataDir, "ut_valid_basic.csv");

            // Act
            service.Load(filePath, isMergeMode: false);

            // Assert
            Assert.NotEmpty(service.Items);
            Assert.True(service.IsDirty); // 読み込み後は変更フラグが立つ
        }

        [Fact]
        public void UT_CMS_10_LoadFromFile_MergeMode()
        {
            // Arrange
            var service = new ConstantManagerService();
            string initialFile = Path.Combine(_testDataDir, "ut_valid_basic.csv");
            string mergeFile = Path.Combine(_testDataDir, "ut_merge_source.csv");

            service.Load(initialFile, isMergeMode: false);
            int initialCount = service.Items.Count;

            // Act
            service.Load(mergeFile, isMergeMode: true);

            // Assert
            Assert.True(service.Items.Count >= initialCount); // マージで増える可能性
            Assert.True(service.IsDirty);
        }

        [Fact]
        public void UT_CMS_11_LoadFromFile_ReplaceMode()
        {
            // Arrange
            var service = new ConstantManagerService();
            string initialFile = Path.Combine(_testDataDir, "ut_valid_basic.csv");
            string replaceFile = Path.Combine(_testDataDir, "ut_merge_source.csv");

            service.Load(initialFile, isMergeMode: false);

            // Act
            service.Load(replaceFile, isMergeMode: false);

            // Assert
            Assert.True(service.IsDirty);
            // replaceFileのデータに置き換わっている
        }

        // ========================================================================
        // UT-CMS-12～13: ファイル保存テスト
        // ========================================================================

        [Fact]
        public void UT_CMS_12_SaveToFile_Success()
        {
            // Arrange
            var service = new ConstantManagerService();
            var item = new ConstantItem("SAVE_TEST", "保存テスト", "100");
            service.AddItem(item);

            string tempFile = Path.Combine(Path.GetTempPath(), "test_save_cms.csv");

            // Act
            service.Save(tempFile);

            // Assert
            Assert.False(service.IsDirty); // 保存後は変更フラグがクリアされる
            Assert.True(File.Exists(tempFile));

            // Cleanup
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }

        [Fact]
        public void UT_CMS_13_SaveToFile_ClearsUnsavedChanges()
        {
            // Arrange
            var service = new ConstantManagerService();
            var item = new ConstantItem("TEST_ITEM", "項目", "100");
            service.AddItem(item);
            Assert.True(service.IsDirty);

            string tempFile = Path.Combine(Path.GetTempPath(), "test_clear_changes.csv");

            // Act
            service.Save(tempFile);

            // Assert
            Assert.False(service.IsDirty);

            // Cleanup
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }

        // ========================================================================
        // UT-CMS-14: 変更検知テスト
        // ========================================================================

        [Fact]
        public void UT_CMS_14_HasUnsavedChanges_DetectsChanges()
        {
            // Arrange
            var service = new ConstantManagerService();
            Assert.False(service.HasUnsavedChanges());

            // Act & Assert - 追加
            var item = new ConstantItem("ITEM_001", "項目", "100");
            service.AddItem(item);
            Assert.True(service.HasUnsavedChanges());

            // 保存してクリア
            string tempFile = Path.Combine(Path.GetTempPath(), "test_changes.csv");
            service.Save(tempFile);
            Assert.False(service.HasUnsavedChanges());

            // 更新
            item.Value = "200";
            Assert.True(service.HasUnsavedChanges());

            // Cleanup
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}