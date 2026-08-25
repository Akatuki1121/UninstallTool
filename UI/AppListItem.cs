using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using UninstallTool;

namespace UninstallTool.UI
{
    /// <summary>
    /// InstalledAppにアイコン(ImageSource)を付加した、AppListView表示専用のラッパー。
    ///
    /// アイコン抽出(Icon.ExtractAssociatedIcon)は1件あたり数十msかかることがあり、
    /// インストール済みアプリが100件超だと合計で数秒規模の遅延要因になる。
    /// そのため、コンストラクタでは即座には解決せず、呼び出し側がバックグラウンドスレッドから
    /// ResolveIcon() を呼んだタイミングで解決する(起動時の体感待ち時間をなくすため)。
    ///
    /// 重要: INotifyPropertyChangedのPropertyChangedはWPFが自動でUIスレッドへマーシャリングしない
    /// (ObservableCollectionのCollectionChangedとは違う)。バックグラウンドスレッドから直接発火すると
    /// バインディング更新が失敗し、アイコン欄が空のまま残ることがある。これが実際にアイコンが
    /// 表示されない事例の原因だった可能性が高いため、プロパティ設定と通知だけDispatcherでUIスレッドに戻す。
    /// </summary>
    public sealed class AppListItem : INotifyPropertyChanged
    {
        public InstalledApp App { get; }
        public ImageSource? Icon { get; private set; }

        public string DisplayName => App.DisplayName;
        public string? DisplayVersion => App.DisplayVersion;
        public string? Publisher => App.Publisher;
        public string? InstallLocation => App.InstallLocation;

        public AppListItem(InstalledApp app)
        {
            App = app;
        }

        /// <summary>
        /// アイコンを解決する。重いネイティブ呼び出し(AppIconResolver.Resolve)自体は
        /// 呼び出し元のバックグラウンドスレッドでそのまま実行されるが、
        /// プロパティへの反映とUI通知だけは必ずUIスレッドのDispatcher経由で行う。
        /// </summary>
        public void ResolveIcon()
        {
            var resolved = AppIconResolver.Resolve(App);

            var dispatcher = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher == null || dispatcher.CheckAccess())
            {
                // Dispatcherが無い(テスト実行時等)、または既にUIスレッド上ならそのまま反映
                Icon = resolved;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Icon)));
            }
            else
            {
                dispatcher.BeginInvoke(() =>
                {
                    Icon = resolved;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Icon)));
                });
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
