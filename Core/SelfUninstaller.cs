using System;
using System.Diagnostics;
using System.IO;

namespace UninstallTool
{
    /// <summary>
    /// 実行中の自分自身(exe)を含め、アプリの痕跡を完全に消去する機能。
    /// 実行中プロセスは自分自身のexeファイルを直接削除できない(Windowsがロックする)ため、
    /// 一時batファイルを生成し、「本体プロセスの終了を待つ→exe削除→bat自身を削除」という
    /// 定番の手法を使う。batは自分自身をdelコマンドで削除してから終了する。
    /// </summary>
    public static class SelfUninstaller
    {
        /// <summary>
        /// 自己アンインストールを開始する。この呼び出し後、呼び出し側は速やかに
        /// Environment.Exit や Application.Shutdown でプロセスを終了させる必要がある
        /// (batはプロセスのPIDを見て終了を待つため)。
        /// </summary>
        public static void BeginSelfUninstall(OperationLog log, string? exePath = null)
        {
            exePath ??= Process.GetCurrentProcess().MainModule?.FileName;
            if (string.IsNullOrEmpty(exePath))
            {
                log.Error("SelfUninstall", "実行ファイルパスの取得に失敗したため自己削除を中止");
                return;
            }

            var currentPid = Environment.ProcessId;
            var batPath = Path.Combine(Path.GetTempPath(), $"uninstall_self_{Guid.NewGuid():N}.bat");

            log.Info("SelfUninstall", "自己削除用batファイルを生成", batPath);

            // タスクバーやエクスプローラーへの解放猶予を含め、PIDの終了をポーリング待機してから削除する。
            // "del \"%~f0\"" は実行中の自分自身(batファイル)を最後に削除する定番のイディオム。
            var batContent = $"""
                @echo off
                :waitloop
                tasklist /FI "PID eq {currentPid}" 2>NUL | find "{currentPid}" >NUL
                if not errorlevel 1 (
                    timeout /t 1 /nobreak >NUL
                    goto waitloop
                )
                del /f /q "{exePath}"
                rmdir "{Path.GetDirectoryName(exePath)}" 2>NUL
                del /f /q "%~f0"
                """;

            File.WriteAllText(batPath, batContent);

            var psi = new ProcessStartInfo
            {
                FileName = batPath,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            Process.Start(psi);
            log.Info("SelfUninstall", "削除用batを起動、プロセス終了待機に入りました", batPath);
        }
    }
}
