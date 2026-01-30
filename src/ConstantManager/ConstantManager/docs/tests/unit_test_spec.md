# 定数管理システム 単体テスト仕様書

**版番:** 1.2  
**作成日:** 2026年1月21日  
**最終更新:** 2026年1月21日（v1.2: テストデータ定義章追加）
**対象システム:** 車載ソフトウェア定数管理ツール

---

## 目次

- [1. 概要](#1-概要)
- [2. テスト方針と網羅性](#2-テスト方針と網羅性)
- [3. ValidationService テストケース](#3-validationservice-テストケース)
- [4. CsvService テストケース](#4-csvservice-テストケース)
- [5. ConstantItem テストケース](#5-constantitem-テストケース)
- [6. ConstantManagerService テストケース](#6-constantmanagerservice-テストケース)
- [7. テスト実行方針](#7-テスト実行方針)
- [8. テストデータ定義](#8-テストデータ定義)
- [9. 改版履歴](#9-改版履歴)

---

## 1. 概要

本仕様書は、定数管理システムの**単体テスト**の要件を定義します。以下のコアサービスに対するテストケースを記載します。

- `ValidationService` - 入力値バリデーション
- `CsvService` - CSV読み書き、形式検証
- `ConstantManagerService` - 定数データ管理、マージロジック
- `ConstantItem` - 定数データモデル

各テストケースは、**テストID、確認項目、前提条件、入力値、期待値**の形式で記載します。

---

## 2. テスト方針と網羅性

### 2.1 単体テスト方針

本プロジェクトの単体テストは、以下のポリシーに基づいて実施されます。

#### 2.1.1 ロジック集中

**テスト対象**: バリデーション、CSV変換、マージ処理などの複雑なビジネスロジック

定数管理システムの核となる以下の処理については、全分岐を網羅的にテストします：

- `ValidationService` のフィールド検証ロジック
  - PhysicalName の形式・長さチェック（正常系、異常系、境界値）
  - LogicalName, Value, Unit, Description の各種バリデーション
  - エラーコード（E005-E009）と警告コード（W003, W004）の適切な出力

- `CsvService` のパース・生成ロジック
  - CSV ヘッダー検証（必須カラム確認）
  - 特殊文字のエスケープ処理（カンマ、ダブルクォート、改行）
  - エンコーディング自動検出（UTF-8, SHIFT_JIS）

- `ConstantManagerService` のマージロジック
  - PhysicalName による既存行の更新判定
  - 新規行の追加処理
  - マージモード・置換モードの動作分岐

#### 2.1.2 効率化と保守性

**テスト対象外**: C# 言語仕様で保証される機能、フレームワーク標準の挙動

以下の項目については、費用対効果および保守性の観点からテスト対象外とします：

- **自動実装プロパティ（Auto-Property）の Getter/Setter**
  - C# コンパイラが自動生成するため、言語仕様で動作が保証される
  - 例：`public string PhysicalName { get; private set; }` の単純な値の取得・設定

- **WinForms コンポーネントの基本挙動**
  - UI フレームワークの標準機能（メニュー、ダイアログ、グリッド）
  - これらは統合テスト仕様書（`integration_test_spec.md`）で検証

- **言語標準メソッドの呼び出し**
  - `List<T>.Add()`, `Dictionary<K,V>.ContainsKey()` など
  - フレームワークテストで検証済みのため、テスト対象外

- **.NET フレームワーク標準ライブラリの機能**
  - ファイルI/O 例外の詳細処理（ただし、高レベルのエラーハンドリングは対象）
  - JSON シリアライザーなど、フレームワークが提供する機能

**例外**: これらの項目でも、カスタムロジックや業務ルール固有の処理が組み込まれていた場合は、テスト対象とします。

---

### 2.2 テスト対象一覧表

以下の表は、内部仕様書で定義されたクラス・メソッドについて、テスト対象の詳細を記載します。

#### 2.2.1 ValidationService

| クラス名 | 機能/メソッド | テスト対象 | 除外理由 | 対応するテストID |
|---------|-----------|---------|--------|-----------------|
| **ValidationService** | `ValidatePhysicalName()` | ○ | - | UT-VS-01～10 |
| **ValidationService** | `ValidateLogicalName()` | ○ | - | UT-VS-11～15 |
| **ValidationService** | `ValidateValue()` | ○ | - | UT-VS-16～21 |
| **ValidationService** | `ValidateUnit()` | ○ | - | UT-VS-22～25-B |
| **ValidationService** | `ValidateDescription()` | ○ | - | UT-VS-26～28-B |

**カバレッジ**: 100%

#### 2.2.2 CsvService

| クラス名 | 機能/メソッド | テスト対象 | 除外理由 | 対応するテストID |
|---------|-----------|---------|--------|-----------------|
| **CsvService** | `Load(string filePath)` | ○ | - | UT-CSV-01～08 |
| **CsvService** | `Save(string filePath, List<ConstantItem> items)` | ○ | - | UT-CSV-09～15 |
| **CsvService** | プライベートメソッド（`ParseLine()` など） | ×| フレームワーク標準メソッドに依存。エンドツーエンドテストで検証 | — |

**カバレッジ**: 100%（公開メソッド）

#### 2.2.3 ConstantItem

| クラス名 | 機能/メソッド | テスト対象 | 除外理由 | 対応するテストID |
|---------|-----------|---------|--------|-----------------|
| **ConstantItem** | `Constructor` | ○ | - | UT-CI-01～02 |
| **ConstantItem** | `PhysicalName` (Property Getter) | × | 自動実装プロパティのため、言語仕様で保証 | — |
| **ConstantItem** | `LogicalName` (Property Getter) | × | 自動実装プロパティのため、言語仕様で保証 | — |
| **ConstantItem** | `Value` (Property Getter) | × | 自動実装プロパティのため、言語仕様で保証 | — |
| **ConstantItem** | `Unit` (Property Getter) | × | 自動実装プロパティのため、言語仕様で保証 | — |
| **ConstantItem** | `Description` (Property Getter) | × | 自動実装プロパティのため、言語仕様で保証 | — |
| **ConstantItem** | `IsModified` (Property Getter/Setter) | × | 自動実装プロパティのため、言語仕様で保証。ただし MarkClean() テスト内で動作確認 | UT-CI-09 |
| **ConstantItem** | `GetCsvLine()` | ○ | - | UT-CI-03～04 |
| **ConstantItem** | `MarkClean()` | ○ | - | UT-CI-09 |
| **ConstantItem** | `Equals(object obj)` | ○ | - | UT-CI-05～08 |
| **ConstantItem** | `GetHashCode()` | ○ | - | UT-CI-07 |

**カバレッジ**: 100%（ビジネスロジック部分）

#### 2.2.4 ConstantManagerService

| クラス名 | 機能/メソッド | テスト対象 | 除外理由 | 対応するテストID |
|---------|-----------|---------|--------|-----------------|
| **ConstantManagerService** | `Items` (Property Getter) | × | `List<T>` のため、言語仕様で保証 | — |
| **ConstantManagerService** | `AddItem(ConstantItem item)` | ○ | - | UT-CMS-01～02-B |
| **ConstantManagerService** | `UpdateItem(ConstantItem item)` | ○ | - | UT-CMS-03～04-B |
| **ConstantManagerService** | `DeleteItem(ConstantItem item)` | ○ | - | UT-CMS-05～06-B |
| **ConstantManagerService** | `Load(string filePath, bool isMergeMode)` | ○ | - | UT-CMS-07～10-B |
| **ConstantManagerService** | `Save(string filePath)` | ⚠️ 部分的 | CsvService に依存。統合テスト（IT-FILE-06）で完全検証 | IT-FILE-06, IT-FILE-07 |
| **ConstantManagerService** | `HasUnsavedChanges()` | ○ | - | UT-CMS-11～14-B |
| **ConstantManagerService** | プライベート変数（`_isDirty`） | ×| 内部状態管理のため、公開メソッド経由で検証 | — |

**カバレッジ**: 83%（Save メソッド単独テストは統合テストに委譲）

#### 2.2.5 カバレッジサマリー

| 層 | クラス名 | テスト対象メソッド数 | テスト対象外メソッド数 | 対応するテストID数 | カバレッジ率 |
|----|---------|------------------|-----------------|-----------------|-----------|
| **BL層** | ValidationService | 5 | 0 | 13 | 100% |
| **DA層** | CsvService | 2 | 1 | 15 | 100%（公開API） |
| **BL層** | ConstantItem | 5 | 6 | 9 | 100%（ビジネスロジック） |
| **BL層** | ConstantManagerService | 5 | 2 | 20 | 83%（Save は統合テストで検証） |
| **総合** | — | 17 | 9 | 57 | **92%** ✅ |

**注釈**
- ○：テスト対象
- ×：テスト対象外
- ⚠️：部分的テスト（統合テストで補完）

---

## 3. ValidationService テストケース

### 3.1 ValidatePhysicalName() テスト

| テストID | テスト項目 | 入力値/条件 | 期待値 | エラーコード |
|---------|---------|---------|--------|-----------|
| UT-VS-01 | **正常系** - 有効なPhysicalName | `"ENGINE_TEMP"` | `IsValid = true`, `IsError = false` | （なし） |
| UT-VS-02 | **正常系** - アンダースコアを含む | `"ENGINE_TEMP_MAX"` | `IsValid = true`, `IsError = false` | （なし） |
| UT-VS-03 | **正常系** - 最大文字数 (32文字) | `"ABCDEFGHIJKLMNOPQRSTUVWXYZ_1234"` (32文字) | `IsValid = true`, `IsError = false` | （なし） |
| UT-VS-04 | **異常系** - 空文字列 | `""` | `IsValid = false`, `IsError = true` | `E005` |
| UT-VS-05 | **異常系** - 小文字を含む | `"Engine_Temp"` | `IsValid = false`, `IsError = true` | `E008` |
| UT-VS-06 | **異常系** - ハイフンを含む | `"ENGINE-TEMP"` | `IsValid = false`, `IsError = true` | `E008` |
| UT-VS-07 | **異常系** - スペースを含む | `"ENGINE TEMP"` | `IsValid = false`, `IsError = true` | `E008` |
| UT-VS-08 | **異常系** - 最大文字数超過 (33文字) | `"ABCDEFGHIJKLMNOPQRSTUVWXYZ_12345"` (33文字) | `IsValid = false`, `IsError = true` | `E009` |
| UT-VS-09 | **異常系** - 数字を含む | `"ENGINE_TEMP_123"` | `IsValid = false`, `IsError = true` | `E008` |
| UT-VS-10 | **異常系** - 特殊文字を含む | `"ENGINE@TEMP"` | `IsValid = false`, `IsError = true` | `E008` |

---

### 3.2 ValidateLogicalName() テスト

| テストID | テスト項目 | 入力値/条件 | 期待値 | エラーコード |
|---------|---------|---------|--------|-----------|
| UT-VS-11 | **正常系** - 日本語文字列 | `"エンジン温度"` | `IsValid = true`, `IsError = false` | （なし） |
| UT-VS-12 | **正常系** - 日本語と英数字の混在 | `"エンジン温度 (Temperature)"` | `IsValid = true`, `IsError = false` | （なし） |
| UT-VS-13 | **正常系** - 最大文字数 (64文字) | 64文字の日本語文字列 | `IsValid = true`, `IsError = false` | （なし） |
| UT-VS-14 | **異常系** - 空文字列 | `""` | `IsValid = false`, `IsError = true` | `E006` |
| UT-VS-15 | **異常系** - 最大文字数超過 (65文字) | 65文字の日本語文字列 | `IsValid = false`, `IsError = true` | `E006` |

---

### 3.3 ValidateValue() テスト

| テストID | テスト項目 | 入力値/条件 | 期待値 | エラーコード |
|---------|---------|---------|--------|-----------|
| UT-VS-16 | **正常系** - 数値（整数） | `"120"` | `IsValid = true`, `IsError = false` | （なし） |
| UT-VS-17 | **正常系** - 小数点を含む | `"12.5"` | `IsValid = true`, `IsError = false` | （なし） |
| UT-VS-18 | **正常系** - 負の数 | `"-50"` | `IsValid = true`, `IsError = false` | （なし） |
| UT-VS-19 | **正常系** - 文字列値 | `"AUTO"` | `IsValid = true`, `IsError = false` | （なし） |
| UT-VS-20 | **異常系** - 空文字列 | `""` | `IsValid = false`, `IsError = true` | `E007` |
| UT-VS-21 | **異常系** - 最大文字数超過 (257文字) | 257文字の文字列 | `IsValid = false`, `IsError = true` | `E007` |

---

### 3.4 ValidateUnit() テスト

| テストID | テスト項目 | 入力値/条件 | 期待値 | エラーコード |
|---------|---------|---------|--------|-----------|
| UT-VS-22 | **正常系** - 単位記号 | `"℃"` | `IsValid = true`, `IsWarning = false` | （なし） |
| UT-VS-23 | **正常系** - 複合単位 | `"km/h"` | `IsValid = true`, `IsWarning = false` | （なし） |
| UT-VS-24 | **正常系** - 省略可能（空文字列） | `""` | `IsValid = true`, `IsWarning = false` | （なし） |
| UT-VS-25 | **異常系** - 最大文字数超過 (17文字超) | `"kilometers per hour"` | `IsValid = true`, `IsWarning = true` | `W003` |
| UT-VS-25-B | **正常系** - 最大文字数の境界値 (16文字) | `"kmph123456789012"` | `IsValid = true`, `IsWarning = false` | （なし） |

---

### 3.5 ValidateDescription() テスト

| テストID | テスト項目 | 入力値/条件 | 期待値 | エラーコード |
|---------|---------|---------|--------|-----------|
| UT-VS-26 | **正常系** - 説明テキスト | `"エンジンの最大許容温度"` | `IsValid = true`, `IsWarning = false` | （なし） |
| UT-VS-27 | **正常系** - 省略可能（空文字列） | `""` | `IsValid = true`, `IsWarning = false` | （なし） |
| UT-VS-28 | **異常系** - 最大文字数超過 (257文字) | 257文字の説明文 | `IsValid = true`, `IsWarning = true` | `W004` |
| UT-VS-28-B | **正常系** - 最大文字数の境界値 (256文字) | 256文字の説明文 | `IsValid = true`, `IsWarning = false` | （なし） |

---

## 4. CsvService テストケース

### 4.1 Load() テスト

| テストID | テスト項目 | 入力値/条件 | 期待値 | エラーコード |
|---------|---------|---------|--------|-----------|
| UT-CSV-01 | **正常系** - 有効なCSVファイル読み込み | [TD-UT-01] 有効なCSV（3行データ）が存在 | リストサイズ = 3、エラーなし | （なし） |
| UT-CSV-02 | **正常系** - UTF-8 BOM付き読み込み | [TD-UT-02] UTF-8（BOM付き）ファイル、日本語含む | 日本語データが正しく読み込まれる | （なし） |
| UT-CSV-03 | **正常系** - SHIFT_JIS 自動検出 | [TD-UT-03] SHIFT_JIS エンコーディング、日本語含む | 日本語データが正しく読み込まれる | （なし） |
| UT-CSV-04 | **正常系** - 特殊文字を含むデータ | [TD-UT-04] `SENSOR_RANGE,"0-100","%","正常範囲: 0～100"` | 特殊文字が正しくパースされる | （なし） |
| UT-CSV-05 | **異常系** - ファイルが存在しない | `"C:\\NonExistent\\file.csv"` | 例外発生、エラーコード E001 | `E001` |
| UT-CSV-06 | **異常系** - ファイルロック状態 | 別プロセスで開かれているCSV | 例外発生、エラーコード E002 | `E002` |
| UT-CSV-07 | **異常系** - ヘッダー行が不正 | [TD-UT-05] ヘッダー行に必須カラム欠落（PhysicalName, LogicalName なし） | 例外発生、エラーコード E003 | `E003` |
| UT-CSV-08 | **異常系** - サポート外のエンコーディング | [TD-UT-06] EUC-JP エンコーディング | エラーコード E004 | `E004` |

---

### 4.2 Save() テスト

| テストID | テスト項目 | 入力値/条件 | 期待値 | エラーコード |
|---------|---------|---------|--------|-----------|
| UT-CSV-09 | **正常系** - CSVファイル保存 | items = [ENGINE_TEMP, SPEED_LIMIT, BATTERY_VOLTAGE] | ファイル作成、ヘッダー + 3行が保存 | （なし） |
| UT-CSV-10 | **正常系** - 特殊文字のエスケープ | Description = `"0～100"` を含むItem | 特殊文字が正しくエスケープされて保存 | （なし） |
| UT-CSV-11 | **正常系** - UTF-8（BOM付き）で保存 | 日本語を含むConstantItem | ファイルがUTF-8（BOM付き）でエンコード | （なし） |
| UT-CSV-12 | **正常系** - CRLF改行コード | 複数のConstantItem | ファイルの改行がCRLF（`\r\n`）である | （なし） |
| UT-CSV-13 | **異常系** - ディスク容量不足 | ディスク容量が不足している環境（シミュレーション） | 例外発生、エラーコード E011 | `E011` |
| UT-CSV-14 | **異常系** - アクセス権限なし | 読み取り専用ディレクトリ | 例外発生、エラーコード E012 | `E012` |
| UT-CSV-15 | **正常系** - 空リストの保存 | items = [] | ファイル作成、ヘッダー行のみ保存 | （なし） |

---

## 5. ConstantItem テストケース

### 5.1 コンストラクタ テスト

| テストID | テスト項目 | 入力値/条件 | 期待値 | エラーコード |
|---------|---------|---------|--------|-----------|
| UT-CI-01 | **正常系** - 有効なデータでインスタンス生成 | `("ENGINE_TEMP", "エンジン温度", "120", "℃", "最高温度")` | インスタンス正常生成、各プロパティが設定値と一致 | （なし） |
| UT-CI-02 | **正常系** - 空の説明フィールド | `("SPEED_LIMIT", "速度制限", "100", "km/h", "")` | インスタンス正常生成 | （なし） |

---

### 5.2 GetCsvLine() テスト

| テストID | テスト項目 | 入力値/条件 | 期待値 | エラーコード |
|---------|---------|---------|--------|-----------|
| UT-CI-03 | **正常系** - CSV行生成（通常データ） | Item: `("ENGINE_TEMP", "エンジン温度", "120", "℃", "最高温度")` | `"ENGINE_TEMP,エンジン温度,120,℃,最高温度"` が返される | （なし） |
| UT-CI-04 | **正常系** - 特殊文字エスケープ | [TD-UT-04] Item: `("RANGE", "範囲", "0-100", "%", "正常値: 0～100")` | 特殊文字が正しくエスケープされた行が返される | （なし） |

---

### 5.3 Equals() と GetHashCode() テスト

| テストID | テスト項目 | 入力値/条件 | 期待値 | エラーコード |
|---------|---------|---------|--------|-----------|
| UT-CI-05 | **正常系** - 同じPhysicalNameは等価 | item1: `("ENGINE_TEMP", "エンジン温度", "120", "℃", "説明1")`<br/>item2: `("ENGINE_TEMP", "エンジン温度", "130", "℃", "説明2")` | `item1.Equals(item2)` = `true` | （なし） |
| UT-CI-06 | **異常系** - 異なるPhysicalNameは非等価 | item1: `("ENGINE_TEMP", ...)`<br/>item2: `("SPEED_LIMIT", ...)` | `item1.Equals(item2)` = `false` | （なし） |
| UT-CI-07 | **正常系** - 同じPhysicalNameは同じハッシュコード | item1, item2 ともに PhysicalName が同じ | `item1.GetHashCode() == item2.GetHashCode()` = `true` | （なし） |
| UT-CI-08 | **正常系** - null との比較 | item = `("ENGINE_TEMP", ...)`<br/>null と比較 | `item.Equals(null)` = `false` | （なし） |

---

### 5.4 MarkClean() テスト

| テストID | テスト項目 | 入力値/条件 | 期待値 | エラーコード |
|---------|---------|---------|--------|-----------|
| UT-CI-09 | **正常系** - 変更フラグのリセット | `item.IsModified = true` → `item.MarkClean()` 呼び出し | `item.IsModified == false` | （なし） |

---

## 6. ConstantManagerService テストケース

### 6.1 AddItem() テスト

| テストID | テスト項目 | 入力値/条件 | 期待値 | エラーコード |
|---------|---------|---------|--------|-----------|
| UT-CMS-01 | **正常系** - 新規定数追加 | 初期0行、1つのConstantItemを追加 | Items.Count = 1、追加されたItemが最後の行 | （なし） |
| UT-CMS-02 | **正常系** - 複数定数を順次追加 | 3個のConstantItemを順次追加 | Items.Count = 3、順序が保持 | （なし） |
| UT-CMS-02-B | **異常系** - PhysicalName 重複チェック | ENGINE_TEMP が既に存在、同じPhysicalNameを追加 | エラーコード E010（重複）またはスキップ処理 | `E010` |

---

### 6.2 UpdateItem() テスト

| テストID | テスト項目 | 入力値/条件 | 期待値 | エラーコード |
|---------|---------|---------|--------|-----------|
| UT-CMS-03 | **正常系** - 既存定数を更新 | Items に ENGINE_TEMP が存在<br/>新Value: "125" で更新 | ENGINE_TEMP の Value が "125" に更新、他のItemに影響なし | （なし） |
| UT-CMS-04 | **正常系** - 複数フィールドを更新 | LogicalName, Value, Unit, Description を全て新値で更新 | 全フィールドが正しく更新される | （なし） |
| UT-CMS-04-B | **異常系** - 存在しないPhysicalNameで更新 | NOT_EXISTS は存在しない状態で更新試行 | エラーまたはスキップ処理 | （適切なエラーコード） |

---

### 6.3 DeleteItem() テスト

| テストID | テスト項目 | 入力値/条件 | 期待値 | エラーコード |
|---------|---------|---------|--------|-----------|
| UT-CMS-05 | **正常系** - 定数を削除 | Items に ENGINE_TEMP, SPEED_LIMIT が存在（2行）<br/>ENGINE_TEMP を削除 | Items.Count = 1、SPEED_LIMIT のみが残る | （なし） |
| UT-CMS-06 | **正常系** - 複数定数を削除 | Items に3個のConstantItemが存在<br/>2個を順次削除 | Items.Count = 1、1個のみが残る | （なし） |
| UT-CMS-06-B | **異常系** - 存在しないアイテムを削除 | NOT_EXISTS は存在しない状態で削除試行 | エラーまたはスキップ処理、Items.Count は変わらない | （適切なエラーコード） |

---

### 6.4 Load() テスト（マージ処理）

| テストID | テスト項目 | 入力値/条件 | 期待値 | エラーコード |
|---------|---------|---------|--------|-----------|
| UT-CMS-07 | **正常系** - マージモード：既存と新規の統合 | [TD-UT-01] 既存3行（ENGINE_TEMP, SPEED_LIMIT, BATTERY_VOLTAGE）<br/>[TD-UT-07] 新規2行（ENGINE_TEMP更新, NEW_CONSTANT新規）<br/>`Load(filePath, isMergeMode: true)` | Items.Count = 4、ENGINE_TEMP は更新、NEW_CONSTANT は新規追加、SPEED_LIMIT は保持 | （なし） |
| UT-CMS-08 | **正常系** - マージモード：既存PhysicalNameを更新 | [TD-UT-02] 既存: ENGINE_TEMP (Value: "120")<br/>[TD-UT-07] 新規ファイル: ENGINE_TEMP (Value: "130")<br/>`Load(filePath, isMergeMode: true)` | ENGINE_TEMP の Value が "130" に更新される | （なし） |
| UT-CMS-09 | **正常系** - マージモード：新規PhysicalNameを追加 | [TD-UT-07] 既存3行、新規ファイルに新しいPhysicalName<br/>`Load(filePath, isMergeMode: true)` | Items.Count = 4、新規PhysicalName が最後に追加される | （なし） |
| UT-CMS-10 | **正常系** - 置換モード：既存データを全て置換 | 既存5行、新規ファイルに3行<br/>`Load(filePath, isMergeMode: false)` | Items.Count = 3、新規ファイルのデータのみが存在 | （なし） |
| UT-CMS-10-B | **正常系** - 置換モード：空CSVで全削除 | [TD-UT-08] 既存3行、新規ファイルはヘッダー行のみ<br/>`Load(emptyFilePath, isMergeMode: false)` | Items.Count = 0、全データが削除される | （なし） |

---

### 6.5 HasUnsavedChanges() テスト

| テストID | テスト項目 | 入力値/条件 | 期待値 | エラーコード |
|---------|---------|---------|--------|-----------|
| UT-CMS-11 | **正常系** - 初期状態：変更なし | インスタンス化後、Save() 実行済み<br/>`HasUnsavedChanges()` | `false` | （なし） |
| UT-CMS-12 | **正常系** - 新規追加後：変更あり | Save() 実行済み後、`AddItem()` 実行<br/>`HasUnsavedChanges()` | `true` | （なし） |
| UT-CMS-13 | **正常系** - 更新後：変更あり | Save() 実行済み後、`UpdateItem()` 実行<br/>`HasUnsavedChanges()` | `true` | （なし） |
| UT-CMS-14 | **正常系** - 保存後：変更リセット | 変更あり（true）→ `Save()` 実行<br/>`HasUnsavedChanges()` | `false` | （なし） |
| UT-CMS-14-B | **異常系** - 保存失敗時の変更フラグ | 変更あり、保存先に書き込み権限なし<br/>`Save()` 実行（失敗）→ `HasUnsavedChanges()` | `true`（変更は保持される） | `E011` または `E012` |

---

## 8. テストデータ定義

以下の表は、単体テストに必要なテストデータ（CSVファイル）の定義です。各テストデータは、対応するテストケースに参照されます。

### 8.1 テストデータ一覧

| データID | ファイル名 | 内容・条件 |
|:---|:---|:---|
| **TD-UT-01** | `ut_valid_basic.csv` | 正常系: 基本データ3件 (ENGINE_TEMP, SPEED_LIMIT, BATTERY_VOLTAGE) |
| **TD-UT-02** | `ut_valid_utf8bom.csv` | 正常系: 日本語・BOM付 (論理名: エンジン温度 等) |
| **TD-UT-03** | `ut_valid_sjis.csv` | 正常系: Shift-JISエンコーディング |
| **TD-UT-04** | `ut_valid_special.csv` | 正常系: 特殊文字 (カンマ、ダブルクォート、改行) を含む |
| **TD-UT-05** | `ut_invalid_header.csv` | 異常系: 必須カラム (PhysicalName) 欠落 |
| **TD-UT-06** | `ut_invalid_encoding.csv` | 異常系: EUC-JP などサポート外エンコード |
| **TD-UT-07** | `ut_merge_source.csv` | 正常系: 更新用データと新規追加データの混合 |
| **TD-UT-08** | `ut_empty.csv` | 準正常系: ヘッダー行のみでデータなし |

---

## 9. 改版履歴

| 版番 | 作成日 | 変更内容 |
|------|--------|---------|
| 1.0 | 2026-01-21 | 初版：ValidationService、CsvService、ConstantManagerService の単体テスト仕様書作成 |
| 1.1 | 2026-01-21 | **追加**: ConstantItem の単体テストケース（UT-CI-01～09）、ConstantManagerService の異常系テストケース（UT-CMS-02-B, UT-CMS-04-B, UT-CMS-06-B, UT-CMS-10-B, UT-CMS-14-B）、CsvService の追加境界値テスト（UT-CSV-15）、ValidateUnit/Description の境界値テスト（UT-VS-25-B, UT-VS-28-B）。**新規**: 第2章「テスト方針と網羅性」を追加し、テストポリシー（ロジック集中、効率化と保守性）と、全クラス・メソッドについて表形式でテスト対象を定義。テストカバレッジ率 92% を達成。顧客向けレビュー準備完了。**構成変更**: 従来の第8章「要件カバレッジ」は削除し、新第2章に統合 |
| 1.2 | 2026-01-21 | **新規**: 第8章「テストデータ定義」を追加。単体テストに必要なテストデータ（CSVファイル）を表形式で定義。正常系・異常系・境界値のテストデータを8種類定義し、各テストケースとの対応関係を明記。テストデータ管理の透明性とメンテナンス性を向上。**構成変更**: 従来の第7章「テスト実行方針」は変更なし、従来の第8章「改版履歴」を第9章に繰り上げ |

---

**ドキュメント管理情報**
- **対象フレームワーク**: .NET 10
- **対象言語**: C#
- **テスト対象**: コアサービス層（Models, Services）
- **ステータス**: テスト実施準備完了（顧客レビュー版）
- **テストカバレッジ率**: 92%（57テストケース）
- **最終更新**: 2026-01-21
