using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConstantManager.Models;

namespace ConstantManager.Services
{
    /// <summary>
    /// 定数管理ロジックの中核となるサービスクラス。
    /// CSV読み込み・保存、マージ・置換処理、変更追跡を担当します。
    /// 仕様書 5.1, 6.2, 6.3 に準拠。
    /// </summary>
    public class ConstantManagerService
    {
        private readonly CsvService _csvService;
        private List<ConstantItem> _items;
        private bool _isDirty;

        /// <summary>
        /// ConstantManagerService を初期化します。
        /// </summary>
        public ConstantManagerService()
        {
            _csvService = new CsvService();
            _items = new List<ConstantItem>();
            _isDirty = false;
        }

        /// <summary>
        /// 現在管理している定数リストを取得します。
        /// </summary>
        public List<ConstantItem> Items => _items;

        /// <summary>
        /// 未保存の変更があるかどうかを判定します。
        /// _isDirty フラグ OR 任意の Items の IsModified フラグをチェック。
        /// 仕様書 6.2 参照。
        /// </summary>
        public bool IsDirty => _isDirty || _items.Any(x => x.IsModified);

        /// <summary>
        /// 指定されたCSVファイルを読み込み、マージ・置換処理を行います。
        /// 仕様書 5.1 CSVインポート・マージフロー参照。
        /// </summary>
        /// <param name="filePath">読み込むCSVファイルのパス</param>
        /// <param name="isMergeMode">true: マージモード、false: 置換モード</param>
        /// <exception cref="InvalidOperationException">CSV形式エラー（E003）</exception>
        /// <exception cref="IOException">ファイルI/O エラー</exception>
        public void Load(string filePath, bool isMergeMode)
        {
            // CsvService でファイルを読み込み
            var loadedItems = _csvService.Load(filePath);

            if (isMergeMode)
            {
                // マージモード（仕様書 5.1 参照）
                MergeItems(loadedItems);
            }
            else
            {
                // 置換モード（仕様書 5.1 参照）
                ReplaceItems(loadedItems);
            }

            // インポート完了、変更フラグをセット
            _isDirty = true;
        }

        /// <summary>
        /// Items を指定したパスにCSVとして保存します。
        /// 保存後、全アイテムの IsModified フラグと _isDirty をリセットします。
        /// 仕様書 5.2 保存確認フロー参照。
        /// </summary>
        /// <param name="filePath">保存先ファイルのパス</param>
        /// <exception cref="IOException">ファイルI/O エラー</exception>
        public void Save(string filePath)
        {
            // CsvService でファイルに保存
            _csvService.Save(filePath, _items);

            // 保存完了後、全アイテムの IsModified フラグをリセット
            foreach (var item in _items)
            {
                item.MarkClean();
            }

            // 内部の変更フラグもリセット
            _isDirty = false;
        }

        /// <summary>
        /// 新規定数アイテムをリストに追加します。
        /// 変更フラグを true にセットします。
        /// </summary>
        /// <param name="item">追加するConstantItem</param>
        public void AddItem(ConstantItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            // 重複チェック（PhysicalName で判定）
            if (_items.Any(x => x.PhysicalName == item.PhysicalName))
            {
                throw new InvalidOperationException(
                    $"定数 '{item.PhysicalName}' は既に存在します");
            }

            _items.Add(item);
            _isDirty = true;
        }

        /// <summary>
        /// 既存の定数アイテムを更新します。
        /// PhysicalName が一致するアイテムを探し、プロパティを更新します。
        /// 仕様書 6.3 CSVマージ処理参照。
        /// </summary>
        /// <param name="newItem">更新内容を含むConstantItem</param>
        /// <exception cref="KeyNotFoundException">PhysicalName が見つからない場合</exception>
        public void UpdateItem(ConstantItem newItem)
        {
            if (newItem == null)
            {
                throw new ArgumentNullException(nameof(newItem));
            }

            // PhysicalName で既存アイテムを検索
            var existingItem = _items.FirstOrDefault(x => x.PhysicalName == newItem.PhysicalName);
            if (existingItem == null)
            {
                throw new KeyNotFoundException(
                    $"定数 '{newItem.PhysicalName}' が見つかりません");
            }

            // プロパティを更新（LogicalName, Value, Unit, Description）
            // 仕様書 6.3 参照：「LogicalName, Value, Unit, Description を上書き」
            existingItem.LogicalName = newItem.LogicalName;
            existingItem.Value = newItem.Value;
            existingItem.Unit = newItem.Unit;
            existingItem.Description = newItem.Description;

            // IsModified は LogicalName 等の setter で自動的に true になる
        }

        /// <summary>
        /// 定数アイテムを削除します。
        /// 変更フラグを true にセットします。
        /// </summary>
        /// <param name="item">削除するConstantItem</param>
        public void DeleteItem(ConstantItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            // PhysicalName で削除
            var existingItem = _items.FirstOrDefault(x => x.PhysicalName == item.PhysicalName);
            if (existingItem != null)
            {
                _items.Remove(existingItem);
                _isDirty = true;
            }
        }

        /// <summary>
        /// 未保存の変更があるかどうかを判定します。
        /// IsDirty プロパティと同等の結果を返します。
        /// 仕様書 6.2 変更追跡メカニズム参照。
        /// </summary>
        /// <returns>未保存の変更がある場合は true</returns>
        public bool HasUnsavedChanges()
        {
            return IsDirty;
        }

        /// <summary>
        /// マージモードでのアイテム統合を行います。
        /// 既存リストに新規データを統合。既存データの消失を防止します。
        /// 仕様書 6.3 CSVマージ処理参照。
        /// </summary>
        /// <param name="loadedItems">読み込んだConstantItem のリスト</param>
        private void MergeItems(List<ConstantItem> loadedItems)
        {
            // 既存データを辞書化（キー: PhysicalName）
            var existingDict = _items.ToDictionary(x => x.PhysicalName, x => x);

            // 読み込んだデータを1つずつ処理
            foreach (var loadedItem in loadedItems)
            {
                if (existingDict.ContainsKey(loadedItem.PhysicalName))
                {
                    // 既存データを更新
                    var existingItem = existingDict[loadedItem.PhysicalName];
                    existingItem.LogicalName = loadedItem.LogicalName;
                    existingItem.Value = loadedItem.Value;
                    existingItem.Unit = loadedItem.Unit;
                    existingItem.Description = loadedItem.Description;
                    // IsModified は setter で自動的に true になる
                }
                else
                {
                    // 新規データを追加
                    _items.Add(loadedItem);
                }
            }
        }

        /// <summary>
        /// 置換モードでのアイテム全置換を行います。
        /// 既存リストをクリアし、読み込んだデータに全置換します。
        /// 仕様書 5.1 CSVインポート・マージフロー参照。
        /// </summary>
        /// <param name="loadedItems">読み込んだConstantItem のリスト</param>
        private void ReplaceItems(List<ConstantItem> loadedItems)
        {
            _items.Clear();
            _items.AddRange(loadedItems);
        }
    }
}
