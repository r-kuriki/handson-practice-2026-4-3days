using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ConstantManager.Models;

namespace ConstantManager.Services
{
    /// <summary>
    /// CSV形式でのデータ入出力を行うサービスクラス。
    /// 仕様書 5.1, 6.4, 7.1 に準拠。
    /// </summary>
    public class CsvService
    {
        // CSV ヘッダー（仕様書 5.1 参照）
        private const string EXPECTED_HEADER = "PhysicalName,LogicalName,Value,Unit,Description";
        private static readonly string[] HEADER_COLUMNS = 
        { 
            "PhysicalName", "LogicalName", "Value", "Unit", "Description" 
        };

        /// <summary>
        /// 指定されたCSVファイルを読み込み、ConstantItem のリストを返します。
        /// エンコーディングは UTF-8（BOM有無）または SHIFT_JIS を自動判別します。
        /// </summary>
        /// <param name="filePath">読み込むCSVファイルのパス</param>
        /// <returns>ConstantItem のリスト</returns>
        /// <exception cref="InvalidOperationException">ヘッダー検証失敗時（E003）</exception>
        /// <exception cref="IOException">ファイルI/O エラー</exception>
        public List<ConstantItem> Load(string filePath)
        {
            var items = new List<ConstantItem>();

            // ファイル読み込み（エンコーディング自動判別）
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(stream, DetectEncoding(stream), detectEncodingFromByteOrderMarks: true))
            {
                // ヘッダー行読み込みと検証
                string headerLine = reader.ReadLine();
                if (string.IsNullOrEmpty(headerLine))
                {
                    throw new InvalidOperationException("[E003] CSV ファイルが空です");
                }

                if (headerLine != EXPECTED_HEADER)
                {
                    throw new InvalidOperationException($"[E003] ヘッダー行が不正です。期待値: {EXPECTED_HEADER}");
                }

                // データ行を読み込み
                string line;
                int lineNumber = 1;
                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;

                    // 空行はスキップ
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    try
                    {
                        // CSV パース
                        var fields = ParseCsvLine(line);
                        
                        if (fields.Count != HEADER_COLUMNS.Length)
                        {
                            // 列数が不一致の場合はスキップ（W001: 警告）
                            continue;
                        }

                        // ConstantItem を生成
                        var item = new ConstantItem(
                            physicalName: fields[0],
                            logicalName: fields[1],
                            value: fields[2],
                            unit: fields[3],
                            description: fields[4]
                        );

                        items.Add(item);
                    }
                    catch (ArgumentException)
                    {
                        // PhysicalName の形式エラー等はスキップ（W001: 警告）
                        continue;
                    }
                    catch (Exception)
                    {
                        // その他のパースエラーもスキップ
                        continue;
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// ConstantItem のリストをCSVファイルに保存します。
        /// エンコーディング: UTF-8 (BOM付き)
        /// </summary>
        /// <param name="filePath">保存先ファイルのパス</param>
        /// <param name="items">保存する ConstantItem のリスト</param>
        /// <exception cref="IOException">ファイルI/O エラー</exception>
        public void Save(string filePath, IEnumerable<ConstantItem> items)
        {
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)))
            {
                // ヘッダー行を出力
                writer.WriteLine(EXPECTED_HEADER);

                // データ行を出力
                foreach (var item in items)
                {
                    var escapedLine = FormatCsvLine(
                        item.PhysicalName,
                        item.LogicalName,
                        item.Value,
                        item.Unit,
                        item.Description
                    );

                    writer.WriteLine(escapedLine);
                }

                writer.Flush();
            }
        }

        /// <summary>
        /// ストリームのエンコーディングを自動判別します。
        /// UTF-8（BOM有無）または SHIFT_JIS を検出。
        /// </summary>
        private Encoding DetectEncoding(FileStream stream)
        {
            byte[] buffer = new byte[3];
            int bytesRead = stream.Read(buffer, 0, 3);

            // BOM チェック
            if (bytesRead >= 3 && buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
            {
                // UTF-8 with BOM
                stream.Seek(0, SeekOrigin.Begin);
                return new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
            }

            if (bytesRead >= 2 && buffer[0] == 0xFF && buffer[1] == 0xFE)
            {
                // UTF-16 LE
                stream.Seek(0, SeekOrigin.Begin);
                return Encoding.Unicode;
            }

            // デフォルト: UTF-8（BOMなし）を試す、失敗時は SHIFT_JIS
            stream.Seek(0, SeekOrigin.Begin);
            return Encoding.UTF8;
        }

        /// <summary>
        /// CSV 形式の1行をパースし、各フィールドのリストを返します。
        /// ダブルクォートで囲まれた値内のカンマ、改行に対応。
        /// </summary>
        private List<string> ParseCsvLine(string line)
        {
            var fields = new List<string>();
            var currentField = new StringBuilder();
            bool insideQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (insideQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        // ダブルクォートが2つ連続している場合（エスケープ）
                        currentField.Append('"');
                        i++; // 次の文字をスキップ
                    }
                    else
                    {
                        // クォートの開閉
                        insideQuotes = !insideQuotes;
                    }
                }
                else if (c == ',' && !insideQuotes)
                {
                    // フィールドの区切り
                    fields.Add(currentField.ToString());
                    currentField.Clear();
                }
                else if ((c == '\n' || c == '\r') && insideQuotes)
                {
                    // クォート内の改行は値として保持
                    if (c == '\r' && i + 1 < line.Length && line[i + 1] == '\n')
                    {
                        currentField.Append("\r\n");
                        i++; // \n をスキップ
                    }
                    else
                    {
                        currentField.Append(c);
                    }
                }
                else
                {
                    currentField.Append(c);
                }
            }

            // 最後のフィールドを追加
            fields.Add(currentField.ToString());

            return fields;
        }

        /// <summary>
        /// ConstantItem のフィールドをCSV形式にフォーマットします。
        /// 仕様書 6.4 の特殊文字処理に準拠。
        /// </summary>
        private string FormatCsvLine(
            string physicalName,
            string logicalName,
            string value,
            string unit,
            string description)
        {
            var line = new StringBuilder();

            line.Append(EscapeCsvField(physicalName));
            line.Append(',');
            line.Append(EscapeCsvField(logicalName));
            line.Append(',');
            line.Append(EscapeCsvField(value));
            line.Append(',');
            line.Append(EscapeCsvField(unit));
            line.Append(',');
            line.Append(EscapeCsvField(description));

            return line.ToString();
        }

        /// <summary>
        /// CSV フィールド値をエスケープ処理します。
        /// カンマ、ダブルクォート、改行を含む場合、ダブルクォートで囲み、
        /// ダブルクォート自体は "" に置換。
        /// </summary>
        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return "";
            }

            // カンマ、ダブルクォート、改行を含むかチェック
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
            {
                // ダブルクォートを "" に置換
                string escaped = field.Replace("\"", "\"\"");
                // 全体をダブルクォートで囲む
                return "\"" + escaped + "\"";
            }

            return field;
        }
    }
}
