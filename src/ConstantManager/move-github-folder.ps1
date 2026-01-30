# Move .github folder to Git repository root
# このスクリプトは、src\ConstantManager\.github フォルダをリポジトリルートに移動させます

# ========================================
# 1. Git リポジトリルートの特定
# ========================================

try {
    # Git リポジトリルートを動的に取得
    $gitRoot = git rev-parse --show-toplevel
    
    if (-not $gitRoot) {
        Write-Error "? Git リポジトリが見つかりません。Git リポジトリのディレクトリで実行してください。"
        exit 1
    }
    
    Write-Host "? Git リポジトリルート: $gitRoot" -ForegroundColor Green
}
catch {
    Write-Error "? Git コマンド実行エラー: $_"
    exit 1
}

# ========================================
# 2. 移動元・移動先パスの定義
# ========================================

$sourcePath = Join-Path $gitRoot "src\ConstantManager\.github"
$destinationPath = Join-Path $gitRoot ".github"

Write-Host ""
Write-Host "?? 移動元: $sourcePath" -ForegroundColor Cyan
Write-Host "?? 移動先: $destinationPath" -ForegroundColor Cyan
Write-Host ""

# ========================================
# 3. 事前チェック
# ========================================

# 移動元フォルダの存在確認
if (-not (Test-Path $sourcePath -PathType Container)) {
    Write-Error "? 移動元フォルダが見つかりません: $sourcePath"
    exit 1
}

Write-Host "? 移動元フォルダが確認されました: $sourcePath" -ForegroundColor Green

# 移動先フォルダの存在確認（既に存在する場合の警告）
if (Test-Path $destinationPath -PathType Container) {
    Write-Host ""
    Write-Warning "??  移動先フォルダが既に存在します: $destinationPath"
    Write-Host ""
    
    # ユーザーに確認
    $response = Read-Host "既存のフォルダを置き換えますか？ (y/n)"
    
    if ($response -ne "y" -and $response -ne "Y") {
        Write-Host "? キャンセルされました。移動処理を中止します。" -ForegroundColor Red
        exit 1
    }
    
    # 既存フォルダを削除
    Write-Host "既存フォルダを削除中..." -ForegroundColor Yellow
    Remove-Item -Path $destinationPath -Recurse -Force -ErrorAction Stop
    Write-Host "? 既存フォルダを削除しました。" -ForegroundColor Green
}

# ========================================
# 4. フォルダの移動
# ========================================

try {
    Write-Host ""
    Write-Host "?? フォルダを移動中..." -ForegroundColor Yellow
    
    # Move-Item を使用してフォルダを移動
    Move-Item -Path $sourcePath -Destination $destinationPath -ErrorAction Stop
    
    Write-Host "? フォルダの移動が完了しました！" -ForegroundColor Green
}
catch {
    Write-Error "? フォルダの移動に失敗しました: $_"
    exit 1
}

# ========================================
# 5. 移動後の確認
# ========================================

if (Test-Path $destinationPath -PathType Container) {
    Write-Host ""
    Write-Host "? 確認: 移動先フォルダが存在します: $destinationPath" -ForegroundColor Green
    
    # 移動先フォルダの内容を表示
    Write-Host ""
    Write-Host "?? 移動後のフォルダ構成:" -ForegroundColor Cyan
    Get-ChildItem -Path $destinationPath -Recurse | ForEach-Object {
        $indent = "  " * ($_.FullName.Split('\').Count - $destinationPath.Split('\').Count)
        Write-Host "$indent?? $($_.Name)"
    }
}
else {
    Write-Error "? 確認エラー: 移動先フォルダが見つかりません。"
    exit 1
}

# ========================================
# 6. Git ステージング（オプション）
# ========================================

Write-Host ""
Write-Host "次のステップ:" -ForegroundColor Cyan
Write-Host "  1. 変更内容を確認: git status"
Write-Host "  2. ステージング: git add .github/"
Write-Host "  3. コミット: git commit -m 'chore: Move .github to repository root'"
Write-Host "  4. プッシュ: git push origin test/v1.0.0"
Write-Host ""

# オプション: 自動でステージングするかどうか
$stageResponse = Read-Host "変更を自動的にステージングしますか？ (y/n)"

if ($stageResponse -eq "y" -or $stageResponse -eq "Y") {
    try {
        Write-Host "ステージング中..." -ForegroundColor Yellow
        git add ".github/"
        Write-Host "? .github/ フォルダがステージされました。" -ForegroundColor Green
        
        # Git ステータスを表示
        Write-Host ""
        Write-Host "?? Git ステータス:" -ForegroundColor Cyan
        git status
    }
    catch {
        Write-Warning "??  Git ステージング中にエラーが発生しました: $_"
    }
}

Write-Host ""
Write-Host "? 処理が完了しました！" -ForegroundColor Green
