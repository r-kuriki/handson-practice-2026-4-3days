using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ConstantManager.Models;
using ConstantManager.Services;

namespace ConstantManager.Views
{
    /// <summary>
    /// 定数管理システムのメイン画面フォーム。
    /// CSVインポート・エクスポート、定数の追加・編集・削除を行います。
    /// 仕様書 3.1, 5.2, 5.4 に準拠。
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly ConstantManagerService _service = new ConstantManagerService();

        public MainForm()
        {
            InitializeComponent();
            SetupEventHandlers();
        }

        /// <summary>
        /// 追加のイベントハンドラーを設定します（Designer で設定できない分）。
        /// </summary>
        private void SetupEventHandlers()
        {
            // チェックボックスの値をリアルタイムでコミット
            dataGridView.CurrentCellDirtyStateChanged += DataGridView_CurrentCellDirtyStateChanged;

            // チェックボックス変更時に行の背景色を更新
            dataGridView.CellValueChanged += DataGridView_CellValueChanged;

            // Designer で既に設定されているイベント：
            // - LoadToolStripMenuItem_Click
            // - SaveToolStripMenuItem_Click
            // - ExportToolStripMenuItem_Click
            // - AddItemToolStripMenuItem_Click
            // - EditItemToolStripMenuItem_Click
            // - DeleteItemToolStripMenuItem_Click
            // - MainForm_FormClosing
            // - DataGridView_CellDoubleClick
        }

        /// <summary>
        /// DataGridView のセルが編集状態になるたびに呼ばれます。
        /// チェックボックス列の値をリアルタイムでコミットし、ユーザーが直後にボタンを押しても値が反映されるようにします。
        /// </summary>
        private void DataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView.IsCurrentCellDirty)
            {
                // 現在の編集内容をコミット（確定）
                dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        /// <summary>
        /// DataGridView のセルの値が変更されたときに呼ばれます。
        /// チェックボックス列の変更を検出し、行の背景色を更新します。
        /// </summary>
        private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // 第1列（CheckBox列）の変更のみを対象
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                var row = dataGridView.Rows[e.RowIndex];
                bool isChecked = Convert.ToBoolean(row.Cells[0].Value ?? false);

                if (isChecked)
                {
                    // チェック true → 薄い黄色
                    row.DefaultCellStyle.BackColor = Color.LightYellow;
                }
                else
                {
                    // チェック false → デフォルト（白）
                    row.DefaultCellStyle.BackColor = Color.White;
                }
            }
        }

        /// <summary>
        /// DataGridView を _service.Items と同期して更新します。
        /// 仕様書 5.4 メニュー操作フロー参照。
        /// </summary>
        private void RefreshGrid()
        {
            dataGridView.Rows.Clear();

            foreach (var item in _service.Items)
            {
                int rowIndex = dataGridView.Rows.Add(
                    false, // チェックボックス（未選択）
                    item.PhysicalName,
                    item.LogicalName,
                    item.Value,
                    item.Unit,
                    item.Description
                );

                var row = dataGridView.Rows[rowIndex];

                // 行データを タグに保存（後で参照用）
                row.Tag = item;

                // 初期状態ではチェックが false なので、背景色をデフォルトに設定
                row.DefaultCellStyle.BackColor = Color.White;
            }

            // ステータスバーを更新
            statusLabel.Text = $"全 {_service.Items.Count} 件";
        }

        /// <summary>
        /// [ファイル] - [読込] メニュー押下時の処理。
        /// 仕様書 5.1 CSVインポート・マージフロー参照。
        /// </summary>
        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV ファイル (*.csv)|*.csv|すべてのファイル (*.*)|*.*";
                openFileDialog.Title = "CSVファイルを選択";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // マージ/置換モードの選択ダイアログ
                        var result = MessageBox.Show(
                            "既存データとマージしますか？\n\n[はい] マージモード（既存データを保持）\n[いいえ] 置換モード（既存データをクリア）",
                            "インポートモード選択",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.Cancel)
                            return;

                        bool isMergeMode = (result == DialogResult.Yes);

                        // データを読み込み
                        _service.Load(openFileDialog.FileName, isMergeMode);

                        // グリッドを更新
                        RefreshGrid();

                        MessageBox.Show(
                            $"CSVを読み込みました。全 {_service.Items.Count} 件",
                            "読込成功",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(
                            ex.Message,
                            "CSV形式エラー",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"読込エラー: {ex.Message}",
                            "エラー",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// [ファイル] - [保存] メニュー押下時の処理。
        /// 仕様書 5.2 保存確認フロー参照。
        /// </summary>
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV ファイル (*.csv)|*.csv|すべてのファイル (*.*)|*.*";
                saveFileDialog.Title = "保存先を指定してください";
                saveFileDialog.DefaultExt = ".csv";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _service.Save(saveFileDialog.FileName);

                        MessageBox.Show(
                            $"CSVを保存しました。\n{saveFileDialog.FileName}",
                            "保存成功",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"保存エラー: {ex.Message}",
                            "エラー",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// [ファイル] - [エクスポート] メニュー押下時の処理。
        /// チェックボックスが選択された行をエクスポートします。
        /// 仕様書 5.4 メニュー操作フロー参照。
        /// </summary>
        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // チェックボックスがチェックされている行を取得
            var targetItems = new List<ConstantItem>();

            // 全行ループ
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                // 第1列（CheckBox）の値を取得
                bool isChecked = Convert.ToBoolean(row.Cells[0].Value);

                if (isChecked)
                {
                    // 行に紐付けられた ConstantItem を取得
                    var item = (ConstantItem)row.Tag;
                    targetItems.Add(item);
                }
            }

            if (targetItems.Count == 0)
            {
                MessageBox.Show(
                    "エクスポートする項目を選択してください。チェックボックスをチェックしてください。",
                    "選択エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV ファイル (*.csv)|*.csv|すべてのファイル (*.*)|*.*";
                saveFileDialog.Title = "エクスポート先を指定してください";
                saveFileDialog.DefaultExt = ".csv";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // 選択されたアイテムのみをCSVに書き出す（CsvService を直接使用）
                        var csvService = new CsvService();
                        csvService.Save(saveFileDialog.FileName, targetItems);

                        MessageBox.Show(
                            $"{targetItems.Count} 件をエクスポートしました。\n{saveFileDialog.FileName}",
                            "エクスポート成功",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"エクスポートエラー: {ex.Message}",
                            "エラー",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// [編集] - [新規追加] メニュー押下時の処理。
        /// 仕様書 5.4 メニュー操作フロー参照。
        /// </summary>
        private void AddItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new ConstantEditDialog(originalItem: null))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        _service.AddItem(dialog.ResultItem);
                        RefreshGrid();
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(
                            ex.Message,
                            "追加エラー",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// [編集] - [編集] メニュー押下時の処理。
        /// 現在選択されている行（青いハイライト）の1件だけを編集します。
        /// 仕様書 5.4 メニュー操作フロー参照。
        /// </summary>
        private void EditItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 選択行を取得（SelectedRows = 青いハイライト行）
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "編集する行を選択してください。",
                    "選択エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (dataGridView.SelectedRows.Count > 1)
            {
                MessageBox.Show(
                    "複数行は編集できません。1行のみ選択してください。",
                    "選択エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = dataGridView.SelectedRows[0];
            var originalItem = (ConstantItem)selectedRow.Tag;

            using (var dialog = new ConstantEditDialog(originalItem: originalItem))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        _service.UpdateItem(dialog.ResultItem);
                        RefreshGrid();
                    }
                    catch (KeyNotFoundException ex)
                    {
                        MessageBox.Show(
                            ex.Message,
                            "更新エラー",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// [編集] - [削除] メニュー押下時の処理。
        /// チェックボックスが選択された行を削除します。
        /// 仕様書 5.4 メニュー操作フロー参照。
        /// </summary>
        private void DeleteItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // チェックボックスがチェックされている行を取得
            var itemsToDelete = new List<ConstantItem>();

            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                var row = dataGridView.Rows[i];
                
                // 第1列（CheckBox）の値を取得
                bool isChecked = Convert.ToBoolean(row.Cells[0].Value);
                
                if (isChecked)
                {
                    itemsToDelete.Add((ConstantItem)row.Tag);
                }
            }

            if (itemsToDelete.Count == 0)
            {
                MessageBox.Show(
                    "削除する項目を選択してください。チェックボックスをチェックしてください。",
                    "選択エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 削除確認ダイアログ
            var result = MessageBox.Show(
                $"選択された {itemsToDelete.Count} 件のデータを削除しますか？",
                "削除確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // 対象のアイテムを全て削除
                foreach (var item in itemsToDelete)
                {
                    _service.DeleteItem(item);
                }

                // グリッドを更新
                RefreshGrid();

                MessageBox.Show(
                    $"{itemsToDelete.Count} 件を削除しました。",
                    "削除完了",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// DataGridView のセルをダブルクリックしたときの処理。
        /// 行全体の編集ダイアログを開きます。
        /// </summary>
        private void DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            // 行を選択状態にして編集メニュー実行
            dataGridView.Rows[e.RowIndex].Selected = true;
            EditItemToolStripMenuItem_Click(null, null);
        }

        /// <summary>
        /// フォームを閉じるときの処理。
        /// 仕様書 5.2 保存確認フロー参照。
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_service.HasUnsavedChanges())
            {
                var result = MessageBox.Show(
                    "保存されていない変更があります。\n保存しますか？",
                    "終了確認",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // 保存ダイアログを表示
                    using (var saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "CSV ファイル (*.csv)|*.csv|すべてのファイル (*.*)|*.*";
                        saveFileDialog.Title = "保存先を指定してください";
                        saveFileDialog.DefaultExt = ".csv";

                        if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                        {
                            try
                            {
                                _service.Save(saveFileDialog.FileName);
                                e.Cancel = false; // 保存成功、終了を続行
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(
                                    $"保存エラー: {ex.Message}",
                                    "エラー",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                e.Cancel = true; // 保存失敗、終了をキャンセル
                            }
                        }
                        else
                        {
                            e.Cancel = true; // キャンセルボタン、終了をキャンセル
                        }
                    }
                }
                else if (result == DialogResult.No)
                {
                    e.Cancel = false; // 保存しない、終了を続行
                }
                else // Cancel
                {
                    e.Cancel = true; // 終了をキャンセル
                }
            }
        }
    }
}
