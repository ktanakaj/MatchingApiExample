# リポジトリガイドライン

## プロジェクト概要
- このリポジトリは **gRPC勉強用のゲームのマッチングAPIのサンプルアプリ**です。

## ディレクトリ構成
* `MatchingApiExample` - ASP.NET Coreサーバーソース
- `MatchingApiExampleClient` - .NET 10 Windows Formsアプリ
- `MatchingApiExampleSharedCode` - gRPCのprotobuf定義

## ビルド&テスト手順
- `dotnet build MatchingApiExample.slnx`: ビルド

## 実装ガイド
- 既存のソースコードの設計を参考にして、造りを合わせる。※ただし既存が露骨に間違っている場合は除く。
- 一般的なコーディングのベストプラクティス（命名, DRY, SOLID, KISS, 等）を意識する。
- ASP.NET CoreやEFCore, Windows Forms, C#のベストプラクティスを意識する。
- UIのテキストはローカライズ前提で全てリソース (resx) を使用する。

## 禁止事項
- **master 直コミット禁止**：必ずブランチを切り、PR経由でマージする。
- テストが通らない差分禁止: PR作成/更新時は修正箇所のビルドとテスト、StyleCopが通ることを確認すること。