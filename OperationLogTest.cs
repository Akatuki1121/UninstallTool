using System;

namespace UninstallTool
{
    /// <summary>
    /// OperationLogが意図通り動くか手動確認するためのテスト。
    /// Main()から呼び出して動作を目視確認する。
    /// </summary>
    public static class OperationLogTest
    {
        public static void Run()
        {
            var log = new OperationLog();

            log.Info("AppList", "インストール済みアプリ一覧を取得開始");
            log.Info("AppList", "レジストリUninstallキーを列挙", @"HKLM\Software\Microsoft\Windows\CurrentVersion\Uninstall");
            log.Info("AppUninstall", "対象アプリを選択", "SampleApp v1.2.3");
            log.Info("RegistryScan", "残存レジストリキーを検索中", @"HKCU\Software\SampleApp");
            log.Info("MftSearch", "MFT高速検索を開始", "検索語: SampleApp");
            log.Warning("MftSearch", "USN Journalが無効なボリュームを検出", "D:\\");

            try
            {
                // わざと例外を起こして、エラーレポートの中身を確認する
                throw new InvalidOperationException("USN Journalへのアクセスに失敗しました(管理者権限が必要な可能性があります)");
            }
            catch (Exception ex)
            {
                log.Error("MftSearch", "USN Journalアクセスでエラー発生");
                var report = log.BuildErrorReport(ex);
                Console.WriteLine(report);
            }
        }
    }
}
