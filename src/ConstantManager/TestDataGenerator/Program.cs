using System;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        // 1. レガシーエンコーディング（Shift-JIS, EUC-JP等）の登録
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        // 出力ディレクトリの設定（ConstantManager.Tests/TestData に動的解決）
        string currentDir = Directory.GetCurrentDirectory();
        
        // 実行パスから相対パスで ConstantManager.Tests/TestData を解決
        // 想定: bin/Debug/net10.0 から ../../../../ConstantManager.Tests/TestData
        string outputDir = Path.Combine(currentDir, "../../../../ConstantManager.Tests/TestData");
        outputDir = Path.GetFullPath(outputDir); // 正規化されたフルパスに変換
        
        Directory.CreateDirectory(outputDir);
        Console.WriteLine($"✓ 保存先: {outputDir}");

        // 既存のCSVファイルをすべて削除（古いデータとの混在を防ぐ）
        Console.WriteLine("✓ 既存のCSVファイルを削除中...");
        foreach (string filePath in Directory.GetFiles(outputDir, "*.csv"))
        {
            try
            {
                // 読み取り専用属性を解除してから削除
                FileInfo fileInfo = new FileInfo(filePath);
                if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    fileInfo.Attributes &= ~FileAttributes.ReadOnly;
                }
                File.Delete(filePath);
                Console.WriteLine($"  - 削除: {Path.GetFileName(filePath)}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"  ✗ 削除失敗: {Path.GetFileName(filePath)} - {ex.Message}");
            }
        }
        Console.WriteLine();

        // 2. エンコーディング変数の定義
        var utf8Bom = new UTF8Encoding(true);                          // BOM付きUTF-8
        var shiftJis = Encoding.GetEncoding("Shift_JIS");              // Shift-JIS
        var eucJp = Encoding.GetEncoding("euc-jp");                    // EUC-JP

        Console.WriteLine("--- Generating Unit Test Data (TD-UT-xx) ---");

        // ========================================================================
        // 単体テスト用ファイル生成 (TD-UT-01 ～ TD-UT-08)
        // ========================================================================

        // TD-UT-01: ut_valid_basic.csv (UTF-8 BOM付・基本データ3行)
        File.WriteAllText(
            Path.Combine(outputDir, "ut_valid_basic.csv"),
            "PhysicalName,LogicalName,Value,Unit,Description\n" +
            "ENGINE_TEMP,エンジン温度,120,℃,エンジンの最大許容温度\n" +
            "SPEED_LIMIT,速度制限,100,km/h,最高速度リミット\n" +
            "BATTERY_VOLTAGE,バッテリー電圧,12,V,バッテリー標準電圧",
            utf8Bom
        );
        Console.WriteLine("  ✓ Created: ut_valid_basic.csv");

        // TD-UT-02: ut_valid_utf8bom.csv (UTF-8 BOM付・日本語データ2行)
        File.WriteAllText(
            Path.Combine(outputDir, "ut_valid_utf8bom.csv"),
            "PhysicalName,LogicalName,Value,Unit,Description\n" +
            "ENGINE_RPM,エンジン回転数,3000,rpm,正常時の回転数\n" +
            "AUDIO_VOLUME,オーディオ音量,50,%,デフォルト音量レベル",
            utf8Bom
        );
        Console.WriteLine("  ✓ Created: ut_valid_utf8bom.csv");

        // TD-UT-03: ut_valid_sjis.csv (★Shift-JIS★・1行)
        File.WriteAllText(
            Path.Combine(outputDir, "ut_valid_sjis.csv"),
            "PhysicalName,LogicalName,Value,Unit,Description\n" +
            "SENSOR_TEMP,センサー温度,25,℃,室温センサーの値",
            shiftJis
        );
        Console.WriteLine("  ✓ Created: ut_valid_sjis.csv (Shift-JIS)");

        // TD-UT-04: ut_valid_special.csv (UTF-8 BOM付・特殊文字含む)
        File.WriteAllText(
            Path.Combine(outputDir, "ut_valid_special.csv"),
            "PhysicalName,LogicalName,Value,Unit,Description\n" +
            "SENSOR_RANGE,正常範囲,\"0-100\",\"%\",\"正常値: 0～100\"\n" +
            "MULTI_LINE,複数行値,\"Line1\nLine2\",,改行を含むテスト",
            utf8Bom
        );
        Console.WriteLine("  ✓ Created: ut_valid_special.csv");

        // TD-UT-05: ut_invalid_header.csv (UTF-8 BOM付・不正ヘッダー、PhysicalName欠落)
        File.WriteAllText(
            Path.Combine(outputDir, "ut_invalid_header.csv"),
            "LogicalName,Value,Unit,Description\n" +
            "テスト,100,unit,必須カラムPhysicalNameが欠落",
            utf8Bom
        );
        Console.WriteLine("  ✓ Created: ut_invalid_header.csv");

        // TD-UT-06: ut_invalid_encoding.csv (★EUC-JP★・1行)
        File.WriteAllText(
            Path.Combine(outputDir, "ut_invalid_encoding.csv"),
            "PhysicalName,LogicalName,Value,Unit,Description\n" +
            "TEST_ITEM,テストアイテム,99,unit,EUC-JPで保存されたファイル",
            eucJp
        );
        Console.WriteLine("  ✓ Created: ut_invalid_encoding.csv (EUC-JP)");

        // TD-UT-07: ut_merge_source.csv (UTF-8 BOM付・マージ用データ2行)
        File.WriteAllText(
            Path.Combine(outputDir, "ut_merge_source.csv"),
            "PhysicalName,LogicalName,Value,Unit,Description\n" +
            "ENGINE_TEMP,エンジン温度,130,℃,更新用データ\n" +
            "NEW_ITEM,新規項目,200,deg,新規追加用データ",
            utf8Bom
        );
        Console.WriteLine("  ✓ Created: ut_merge_source.csv");

        // TD-UT-08: ut_empty.csv (UTF-8 BOM付・ヘッダーのみ、データなし)
        File.WriteAllText(
            Path.Combine(outputDir, "ut_empty.csv"),
            "PhysicalName,LogicalName,Value,Unit,Description",
            utf8Bom
        );
        Console.WriteLine("  ✓ Created: ut_empty.csv");

        // ========================================================================
        // 結合テスト用ファイル生成 (TD-IT-01 ～ TD-IT-06)
        // ========================================================================
        Console.WriteLine("\n--- Generating Integration Test Data (TD-IT-xx) ---");

        // TD-IT-01: it_valid_initial.csv (UTF-8 BOM付・初期表示用5行)
        File.WriteAllText(
            Path.Combine(outputDir, "it_valid_initial.csv"),
            "PhysicalName,LogicalName,Value,Unit,Description\n" +
            "CONST_001,定数001,100,unit1,定数001の説明\n" +
            "CONST_002,定数002,200,unit2,定数002の説明\n" +
            "CONST_003,定数003,300,unit3,定数003の説明\n" +
            "CONST_004,定数004,400,unit4,定数004の説明\n" +
            "CONST_005,定数005,500,unit5,定数005の説明",
            utf8Bom
        );
        Console.WriteLine("  ✓ Created: it_valid_initial.csv");

        // TD-IT-02: it_merge_new.csv (UTF-8 BOM付・マージ読込用2行)
        File.WriteAllText(
            Path.Combine(outputDir, "it_merge_new.csv"),
            "PhysicalName,LogicalName,Value,Unit,Description\n" +
            "CONST_001,定数001,150,unit1,定数001の更新データ\n" +
            "CONST_NEW,新規定数,999,unitNew,新規定数の説明",
            utf8Bom
        );
        Console.WriteLine("  ✓ Created: it_merge_new.csv");

        // TD-IT-03: it_replace_full.csv (UTF-8 BOM付・置換読込用2行)
        File.WriteAllText(
            Path.Combine(outputDir, "it_replace_full.csv"),
            "PhysicalName,LogicalName,Value,Unit,Description\n" +
            "REPLACE_A,置換定数A,1000,unitA,置換用データA\n" +
            "REPLACE_B,置換定数B,2000,unitB,置換用データB",
            utf8Bom
        );
        Console.WriteLine("  ✓ Created: it_replace_full.csv");

        // TD-IT-04: it_error_header.csv (UTF-8 BOM付・エラー確認用、ヘッダー不正)
        File.WriteAllText(
            Path.Combine(outputDir, "it_error_header.csv"),
            "ID,Name,Value,Unit\n" +
            "1,テスト,100,unit",
            utf8Bom
        );
        Console.WriteLine("  ✓ Created: it_error_header.csv");

        // TD-IT-05: it_perf_1000.csv (UTF-8 BOM付・性能検証用1000行)
        var sb = new StringBuilder();
        sb.AppendLine("PhysicalName,LogicalName,Value,Unit,Description");
        for (int i = 1; i <= 1000; i++)
        {
            sb.AppendLine($"PERF_{i:D4},パラメータ{i:D4},{i * 10},unit,パフォーマンステスト用データ{i}");
        }
        File.WriteAllText(
            Path.Combine(outputDir, "it_perf_1000.csv"),
            sb.ToString(),
            utf8Bom
        );
        Console.WriteLine("  ✓ Created: it_perf_1000.csv (1000 data rows)");

        // TD-IT-06: it_readonly.csv (UTF-8 BOM付・読取専用属性テスト用)
        string readonlyFilePath = Path.Combine(outputDir, "it_readonly.csv");
        File.WriteAllText(
            readonlyFilePath,
            "PhysicalName,LogicalName,Value,Unit,Description\n" +
            "READONLY_ITEM,読取専用項目,111,unit,読取専用属性を持つファイル",
            utf8Bom
        );
        Console.WriteLine("  ✓ Created: it_readonly.csv");

        // ファイルを読取専用に設定
        try
        {
            FileInfo fileInfo = new FileInfo(readonlyFilePath);
            fileInfo.Attributes |= FileAttributes.ReadOnly;
            Console.WriteLine("  ✓ Set read-only attribute: it_readonly.csv");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"  ✗ Warning: Failed to set read-only attribute: {ex.Message}");
        }

        // ========================================================================
        // 完了メッセージ
        // ========================================================================
        Console.WriteLine("\n✓ Test data generation completed successfully!");
        Console.WriteLine($"✓ Output directory: {outputDir}");
        Console.WriteLine($"✓ Generated 14 CSV files (8 unit test + 6 integration test)");
    }
}
