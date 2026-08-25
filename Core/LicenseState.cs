using System;
using System.IO;

namespace UninstallTool
{
    /// <summary>
    /// 無料版/有料版(Pro)の機能ゲート。
    /// 通常のライセンスキー検証はまだ実装していないため、一般ユーザーには常にfalse(無料版)を返す。
    ///
    /// 開発者(作者)自身が動作確認のためにPro機能を使えるよう、実行ファイルと同じフォルダに
    /// "dev_unlock.flag" という空ファイルを置くと、ローカルでのみPro扱いになる抜け道を用意する。
    /// これは配布物には含めない前提(自分の開発機にだけ置くファイル)。
    /// 将来、実際のライセンスキー検証に置き換える際もこの判定窓口(IsProUnlocked)は変えずに済む設計。
    /// </summary>
    public static class LicenseState
    {
        private const string DevUnlockFlagFileName = "dev_unlock.flag";

        /// <summary>Pro版機能(複数アプリ一括処理、スキャン結果エクスポート等)が有効かどうか。</summary>
        public static bool IsProUnlocked => HasDevUnlockFlag();

        private static bool HasDevUnlockFlag()
        {
            try
            {
                var exeDir = AppContext.BaseDirectory;
                var flagPath = Path.Combine(exeDir, DevUnlockFlagFileName);
                return File.Exists(flagPath);
            }
            catch
            {
                return false;
            }
        }
    }
}
