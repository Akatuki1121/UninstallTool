# UninstallTool プロジェクト構成案

## 技術スタック
- .NET 8 / C# / WPF
- Win32 API (USN Journal, レジストリ操作)

## 実装順序
1. OperationLog (操作ログ/状態トラッカー基盤)
2. インストール済みアプリ一覧取得
3. MFT高速検索エンジン (USN Journal)
4. 残存物スキャン
5. 孤児候補相関ロジック
6. UI (WPF)

## セットアップ手順(奏星さんの環境で実行)
1. Visual Studio 2022 (Community可) をインストール、".NET デスクトップ開発" ワークロードを選択
2. `dotnet new wpf -n UninstallTool` でプロジェクト作成
3. 以下のOperationLog.csを追加してビルド確認
