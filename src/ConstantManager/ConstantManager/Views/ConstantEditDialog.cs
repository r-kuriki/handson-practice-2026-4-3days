using System;
using System.Windows.Forms;
using ConstantManager.Models;
using ConstantManager.Services;

namespace ConstantManager.Views
{
    /// <summary>
    /// 定数編集用ダイアログフォーム。
    /// 新規追加または既存データの編集を行います。
    /// 仕様書 4.3, 5.3 に準拠。
    /// </summary>
    public partial class ConstantEditDialog : Form
    {
        private ConstantItem _originalItem;
        private bool _isEditMode;

        /// <summary>
        /// 編集結果の ConstantItem を取得します。
        /// OKボタンで閉じた場合のみ有効です。
        /// </summary>
        public ConstantItem ResultItem { get; private set; }

        /// <summary>
        /// ConstantEditDialog を初期化します。
        /// </summary>
        /// <param name="originalItem">編集対象のアイテム。null なら新規モード。</param>
        public ConstantEditDialog(ConstantItem originalItem = null)
        {
            _originalItem = originalItem;
            _isEditMode = originalItem != null;

            InitializeComponent();
            
            // フォームタイトルをモードに応じて設定
            this.Text = _isEditMode ? "定数編集" : "定数新規追加";

            if (_isEditMode)
            {
                SetupEditMode();
            }
        }

        /// <summary>
        /// 編集モードの初期化を行います。
        /// 既存データを各フィールドに設定し、PhysicalName を読み取り専用にします。
        /// </summary>
        private void SetupEditMode()
        {
            if (_originalItem == null)
                return;

            txtPhysicalName.Text = _originalItem.PhysicalName;
            txtLogicalName.Text = _originalItem.LogicalName;
            txtValue.Text = _originalItem.Value;
            txtUnit.Text = _originalItem.Unit;
            txtDescription.Text = _originalItem.Description;

            // PhysicalName は読み取り専用（仕様書 5.3 参照）
            txtPhysicalName.ReadOnly = true;
            txtPhysicalName.BackColor = SystemColors.Control;
        }

        /// <summary>
        /// OKボタン押下時の処理。
        /// 仕様書 5.3 入力バリデーションフロー に準拠。
        /// </summary>
        private void BtnOk_Click(object sender, EventArgs e)
        {
            // 各項目を検証（仕様書 5.3 参照）
            var physicalNameValidation = Validator.ValidatePhysicalName(txtPhysicalName.Text);
            if (!physicalNameValidation.IsValid && _isEditMode == false)
            {
                // 新規モードのみ PhysicalName を検証
                MessageBox.Show(
                    physicalNameValidation.ErrorMessage,
                    "検証エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                txtPhysicalName.Focus();
                return;
            }

            var logicalNameValidation = Validator.ValidateLogicalName(txtLogicalName.Text);
            if (!logicalNameValidation.IsValid)
            {
                MessageBox.Show(
                    logicalNameValidation.ErrorMessage,
                    "検証エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                txtLogicalName.Focus();
                return;
            }

            var valueValidation = Validator.ValidateValue(txtValue.Text);
            if (!valueValidation.IsValid)
            {
                MessageBox.Show(
                    valueValidation.ErrorMessage,
                    "検証エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                txtValue.Focus();
                return;
            }

            var unitValidation = Validator.ValidateUnit(txtUnit.Text);
            if (!unitValidation.IsValid)
            {
                MessageBox.Show(
                    unitValidation.ErrorMessage,
                    "検証警告",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            var descriptionValidation = Validator.ValidateDescription(txtDescription.Text);
            if (!descriptionValidation.IsValid)
            {
                MessageBox.Show(
                    descriptionValidation.ErrorMessage,
                    "検証警告",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            // 全検証OK：ResultItem を生成
            try
            {
                if (_isEditMode)
                {
                    // 編集モード：既存のPhysicalName を保持
                    ResultItem = new ConstantItem(
                        physicalName: _originalItem.PhysicalName,
                        logicalName: txtLogicalName.Text,
                        value: txtValue.Text,
                        unit: txtUnit.Text,
                        description: txtDescription.Text
                    );
                }
                else
                {
                    // 新規モード：PhysicalName から生成
                    ResultItem = new ConstantItem(
                        physicalName: txtPhysicalName.Text,
                        logicalName: txtLogicalName.Text,
                        value: txtValue.Text,
                        unit: txtUnit.Text,
                        description: txtDescription.Text
                    );
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(
                    $"アイテム生成エラー: {ex.Message}",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// キャンセルボタン押下時の処理。
        /// </summary>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
