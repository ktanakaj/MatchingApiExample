# gRPC勉強用マッチングAPIサンプルアプリ
gRPCの勉強用にASP.NET Coreで作成したサンプルアプリです。

ごく簡単なマッチングAPIを提供します。

## 開発環境
* Visual Studio 2019 Community - 統合開発環境

## 実行方法
Visual Studioでのデバッグ実行、またはVisual Studioでビルドして生成したバイナリを実行できます。

`MatchingApiExample`がサーバー、`MatchingApiClient`がクライアントです。  
サーバーを起動した状態で、クライアントを実行してください。
（デバッグ実行時は「[マルチスタートアッププロジェクト](https://docs.microsoft.com/ja-jp/visualstudio/ide/how-to-set-multiple-startup-projects?view=vs-2019)」に設定すると便利です。）

クライアントは複数起動可能です。いわゆるマッチングは「部屋を探す」で実行できます。  
レーティング値でルームへのマッチングを行うため、レーティング値を変更して動作を試すことができます。  
（±100で5秒間隔で探索範囲を広げていって、10秒待って見つからなければ部屋を作成します。）

TODO: 部屋では「じゃんけん」で対戦できます。「じゃんけん」の勝敗に応じてプレイヤーのレーティング値が変動します。

## ライセンス
[MIT](https://github.com/ktanakaj/MatchingApiExample/blob/master/LICENSE)
