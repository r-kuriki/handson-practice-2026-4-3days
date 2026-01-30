# .github フォルダ移動スクリプト

## ?? 概要

このスクリプトは、`src\ConstantManager\.github` フォルダを Git リポジトリのルートディレクトリに移動させます。

### 現在の構造

```
handson-practice-2026-4-3days/  ← Git リポジトリルート
├── src/
│   ├── ConstantManager/
│   │   ├── .github/             ← ここにある
│   │   │   ├── workflows/
│   │   │   │   ├── build-and-test.yml
│   │   │   │   └── README.md
│   │   │   └── ...
│   │   ├── Models/
│   │   ├── Services/
│   │   └── ...
├── README.md
└── ...
```

### 移動後の構造（目標）

```
handson-practice-2026-4-3days/  ← Git リポジトリルート
├── .github/                     ← ここに移動
│   ├── workflows/
│   │   ├── build-and-test.yml
│   │   └── README.md
│   └── ...
├── src/
│   ├── ConstantManager/
│   │   ├── Models/
│   │   ├── Services/
│   │   └── ...
├── README.md
└── ...
```

---

## ?? 使用方法

### 前提条件

- **PowerShell 5.0 以上** がインストールされていること
- **Git** がインストールされていること
- スクリプト実行ユーザーに読み書き権限があること

### 実行手順

#### 1?? PowerShell を開く

```powershell
# PowerShell を管理者として実行（推奨）
# または、通常の PowerShell でも可能
```

#### 2?? リポジトリディレクトリに移動

```powershell
cd "C:\Users\kumor\OneDrive\デスクトップ\Dev\workspace_2026\20260121_github_handson\handson-practice-2026-4-3days"
```

または（任意の下位ディレクトリからでも動作）:

```powershell
cd "src\ConstantManager"
# スクリプトは自動的に Git ルートを特定します
```

#### 3?? スクリプトを実行

```powershell
.\move-github-folder.ps1
```

---

## ?? スクリプトの処理フロー

### フロー図

```
┌─────────────────────────────────┐
│ スクリプト開始                   │
└──────────────┬──────────────────┘
               │
               ↓
┌─────────────────────────────────┐
│ 1. Git リポジトリルート特定      │
│    $ git rev-parse --show-toplevel
└──────────────┬──────────────────┘
               │
               ↓
        Git ルート取得成功？
        ├─ YES ↓
        │   ┌──────────────────────────────┐
        │   │ ? パスを表示                 │
        │   └──────┬───────────────────────┘
        │          │
        │          ↓
        │   ┌──────────────────────────────┐
        │   │ 2. 移動元パスの確認           │
        │   │    src\ConstantManager\.github
        │   └──────┬───────────────────────┘
        │          │
        │          ↓
        │   フォルダ存在？
        │   ├─ YES ↓
        │   │   ┌──────────────────────────────┐
        │   │   │ ? フォルダを確認            │
        │   │   └──────┬───────────────────────┘
        │   │          │
        │   │          ↓
        │   │   ┌──────────────────────────────┐
        │   │   │ 3. 移動先の確認              │
        │   │   │    リポジトルート\.github
        │   │   └──────┬───────────────────────┘
        │   │          │
        │   │          ↓
        │   │   既存フォルダ存在？
        │   │   ├─ YES ↓
        │   │   │   ┌──────────────────────┐
        │   │   │   │ ユーザーに確認        │
        │   │   │   │ (置き換えますか？)   │
        │   │   │   └────┬─────────────────┘
        │   │   │        │
        │   │   │        ├─ YES ↓
        │   │   │        │   ┌──────────────┐
        │   │   │        │   │ 既存削除      │
        │   │   │        │   └────┬─────────┘
        │   │   │        │        │
        │   │   │        └────┬───┘
        │   │   │             ↓
        │   │   ├─ NO ↓
        │   │   └──────┐
        │   │          ↓
        │   │   ┌──────────────────────────────┐
        │   │   │ 4. フォルダを移動            │
        │   │   │    Move-Item               │
        │   │   └──────┬───────────────────────┘
        │   │          │
        │   │          ↓
        │   │   移動成功？
        │   │   ├─ YES ↓
        │   │   │   ┌──────────────────────────────┐
        │   │   │   │ ? 移動完了メッセージ表示    │
        │   │   │   └──────┬───────────────────────┘
        │   │   │          │
        │   │   │          ↓
        │   │   │   ┌──────────────────────────────┐
        │   │   │   │ 5. 移動後の確認              │
        │   │   │   │    フォルダ構成表示          │
        │   │   │   └──────┬───────────────────────┘
        │   │   │          │
        │   │   │          ↓
        │   │   │   ┌──────────────────────────────┐
        │   │   │   │ 6. Git ステージング（オプション）
        │   │   │   │    ユーザーに確認            │
        │   │   │   └──────┬───────────────────────┘
        │   │   │          │
        │   │   │          ↓
        │   │   │   ┌──────────────────────────────┐
        │   │   │   │ ? 処理完了                  │
        │   │   │   └──────────────────────────────┘
        │   │   │
        │   │   └─ NO ↓
        │   │       ┌──────────────────┐
        │   │       │ ? エラー終了     │
        │   │       └──────────────────┘
        │   │
        │   └─ NO ↓
        │       ┌──────────────────────────┐
        │       │ ? フォルダなし エラー   │
        │       └──────────────────────────┘
        │
        └─ NO ↓
            ┌──────────────────────────┐
            │ ? Git ルート特定失敗    │
            └──────────────────────────┘
```

---

## ?? スクリプトの主要セクション

### セクション 1??: Git リポジトリルートの特定

```powershell
$gitRoot = git rev-parse --show-toplevel
```

**役割:** Git リポジトリのルートディレクトリを動的に取得

**例:**
```
C:\Users\kumor\OneDrive\...\handson-practice-2026-4-3days
```

**メリット:** スクリプトをどの深さのディレクトリから実行しても機能

---

### セクション 2??: パスの定義

```powershell
$sourcePath = Join-Path $gitRoot "src\ConstantManager\.github"
$destinationPath = Join-Path $gitRoot ".github"
```

**役割:** 移動元・移動先のフルパスを構築

**例:**
```
sourcePath:
C:\...\handson-practice-2026-4-3days\src\ConstantManager\.github

destinationPath:
C:\...\handson-practice-2026-4-3days\.github
```

---

### セクション 3??: 事前チェック

```powershell
if (-not (Test-Path $sourcePath -PathType Container)) {
    Write-Error "? 移動元フォルダが見つかりません"
    exit 1
}
```

**役割:** 移動元フォルダが存在するか確認

**効果:** 存在しない場合、安全にエラー終了

---

### セクション 4??: 既存フォルダの処理

```powershell
if (Test-Path $destinationPath -PathType Container) {
    # ユーザーに確認
    $response = Read-Host "既存のフォルダを置き換えますか？ (y/n)"
    
    if ($response -eq "y") {
        Remove-Item -Path $destinationPath -Recurse -Force
    }
}
```

**役割:** 移動先に既にフォルダがある場合、ユーザーに確認

**安全性:** 既存データの喪失を防止

---

### セクション 5??: フォルダの移動

```powershell
Move-Item -Path $sourcePath -Destination $destinationPath -ErrorAction Stop
```

**役割:** フォルダを実際に移動

**エラー処理:** 失敗時は即座に終了

---

### セクション 6??: 移動後の確認

```powershell
if (Test-Path $destinationPath -PathType Container) {
    Get-ChildItem -Path $destinationPath -Recurse | ForEach-Object {
        # フォルダ構成を視覚的に表示
    }
}
```

**役割:** 移動が成功したか確認し、フォルダ構成を表示

---

### セクション 7??: Git ステージング（オプション）

```powershell
$stageResponse = Read-Host "変更を自動的にステージングしますか？ (y/n)"

if ($stageResponse -eq "y") {
    git add ".github/"
}
```

**役割:** Git に変更内容を自動的にステージング

**オプション:** ユーザーが手動で行うこともできます

---

## ?? 重要なポイント

### ? PowerShell の文字エンコーディング設定

実行前に、必要に応じて以下を実行：

```powershell
# UTF-8 エンコーディングを設定（日本語対応）
[System.Text.Encoding]::Default = [System.Text.Encoding]::UTF8
```

### ? スクリプト実行ポリシーの確認

スクリプトが実行されない場合：

```powershell
# 現在のポリシーを確認
Get-ExecutionPolicy

# 必要に応じて変更（管理者権限が必須）
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### ? エラーが発生した場合

```powershell
# スクリプトを詳細ログモードで実行
powershell -Command "Set-PSDebug -Trace 1; .\move-github-folder.ps1"
```

---

## ?? 実行例

### 正常系の実行

```powershell
PS> .\move-github-folder.ps1
? Git リポジトリルート: C:\Users\kumor\...\handson-practice-2026-4-3days

?? 移動元: C:\Users\kumor\...\handson-practice-2026-4-3days\src\ConstantManager\.github
?? 移動先: C:\Users\kumor\...\handson-practice-2026-4-3days\.github

? 移動元フォルダが確認されました: C:\Users\kumor\...\handson-practice-2026-4-3days\src\ConstantManager\.github

?? フォルダを移動中...
? フォルダの移動が完了しました！

? 確認: 移動先フォルダが存在します: C:\Users\kumor\...\handson-practice-2026-4-3days\.github

?? 移動後のフォルダ構成:
  ?? workflows
    ?? build-and-test.yml
    ?? README.md

次のステップ:
  1. 変更内容を確認: git status
  2. ステージング: git add .github/
  3. コミット: git commit -m 'chore: Move .github to repository root'
  4. プッシュ: git push origin test/v1.0.0

変更を自動的にステージングしますか？ (y/n): y
ステージング中...
? .github/ フォルダがステージされました。

?? Git ステータス:
On branch test/v1.0.0
Changes to be committed:
  (use "git restore --staged <file>..." to unstage)
        new file:   .github/workflows/build-and-test.yml
        new file:   .github/workflows/README.md
        deleted:    src/ConstantManager/.github/workflows/build-and-test.yml
        deleted:    src/ConstantManager/.github/workflows/README.md

? 処理が完了しました！
```

### エラー系の実行

```powershell
PS> .\move-github-folder.ps1
? Git リポジトリが見つかりません。Git リポジトリのディレクトリで実行してください。
```

---

## ?? クリーンアップ

移動後、古いディレクトリが削除されていることを確認：

```powershell
# src\ConstantManager\.github が削除されているか確認
Test-Path "src\ConstantManager\.github"
# 結果: False（削除されている）

# \.github が新しく作成されているか確認
Test-Path ".github"
# 結果: True（存在している）
```

---

## ?? トラブルシューティング

### Q: スクリプトが実行されない

**A:** 実行ポリシーを確認・変更してください：
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Q: 「Git コマンド実行エラー」が出る

**A:** Git がインストールされているか、PATH に登録されているか確認：
```powershell
git --version
```

### Q: 「移動元フォルダが見つかりません」が出る

**A:** フォルダパスが正しいか確認：
```powershell
Test-Path "src\ConstantManager\.github"
```

### Q: 「アクセス拒否」エラーが出る

**A:** PowerShell を管理者として実行してください

---

## ?? 実行後の確認コマンド

```powershell
# 1. フォルダが正しく移動されているか確認
Get-Item ".github" -Force
Get-Item "src\ConstantManager\.github" -ErrorAction SilentlyContinue

# 2. Git の変更内容を確認
git status

# 3. ワークフローファイルが存在するか確認
Test-Path ".github\workflows\build-and-test.yml"
Test-Path ".github\workflows\README.md"
```

---

## ? 次のステップ

移動完了後の作業：

```powershell
# 1. 変更をコミット
git commit -m "chore: Move .github to repository root"

# 2. ブランチにプッシュ
git push origin test/v1.0.0

# 3. PR を作成（GitHub UI で）
# または
gh pr create --base main --head test/v1.0.0 --title "Move .github to repository root"
```

---

**スクリプト作成日:** 2026-01-21  
**対応 Git バージョン:** 2.0 以上  
**対応 PowerShell バージョン:** 5.0 以上
