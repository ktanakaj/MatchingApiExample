// ================================================================================================
// <summary>
//      エラー処理gRPCインターセプタークラスソース</summary>
//
// <copyright file="ErrorHandlingInterceptor.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Service
{
    using System;
    using System.Threading.Tasks;
    using Grpc.Core;
    using Grpc.Core.Interceptors;
    using Microsoft.Extensions.Logging;
    using Honememo.MatchingApiExample.Exceptions;

    /// <summary>
    /// エラー処理gRPCインターセプタークラス。
    /// </summary>
    public class ErrorHandlingInterceptor : Interceptor
    {
        #region メンバー変数

        /// <summary>
        /// ロガー。
        /// </summary>
        private readonly ILogger<ErrorHandlingInterceptor> logger;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 渡されたロガーを用いてインターセプターを生成する。
        /// </summary>
        /// <param name="logger">ロガー。</param>
        public ErrorHandlingInterceptor(ILogger<ErrorHandlingInterceptor> logger)
        {
            this.logger = logger;
        }

        #endregion

        #region 継承メソッド

        /// <summary>
        /// サーバーサイドのgRPC呼び出しのハンドラー。
        /// </summary>
        /// <typeparam name="TRequest">リクエストの型。</typeparam>
        /// <typeparam name="TResponse">レスポンスの型。</typeparam>
        /// <param name="request">リクエスト。</param>
        /// <param name="context">実行コンテキスト。</param>
        /// <param name="continuation">処理。</param>
        /// <returns>レスポンス。</returns>
        public async override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (Exception ex)
            {
                throw this.OnException(ex);
            }
        }

        #endregion

        #region 内部メソッド

        /// <summary>
        /// 例外を処理して、対応するgRPC例外を返す。
        /// </summary>
        /// <param name="exception">発生した例外。</param>
        /// <returns>レスポンスに返すgRPC例外。</returns>
        private RpcException OnException(Exception exception)
        {
            // RpcExceptionが直接投げられた場合はそのまま
            if (exception is RpcException rpcEx)
            {
                return rpcEx;
            }

            // 例外の種類に応じたステータスを用いる
            var status = StatusCode.Unknown;

            // TODO: gRPCステータスコードとの対応は、ちゃんとやるならエラーコードマスタとか定義してそこから取る。
            //       マスタ定義するなら、通常例外の業務例外への変換とかもやる。
            if (exception is AppException appEx)
            {
                switch (appEx.Code)
                {
                    case "INVALID_ARGUMENT":
                        status = StatusCode.InvalidArgument;
                        break;
                    case "NOT_FOUND":
                        status = StatusCode.NotFound;
                        break;
                    case "ALREADY_EXISTS":
                        status = StatusCode.AlreadyExists;
                        break;
                    case "PERMISSION_DENIED":
                        status = StatusCode.PermissionDenied;
                        break;
                    case "FAILED_PRECONDITION":
                        status = StatusCode.FailedPrecondition;
                        break;
                }
            }

            // エラーログを出力
            // TODO: エラーの種類に応じてログレベルを切り替え。エラーマスタにログレベルも持たせる
            this.logger.LogError(0, exception, string.Empty);

            // TODO: エラーメッセージは本番環境ではそのまま返さないようにする
            return new RpcException(new Status(status, exception.Message, exception));
        }

        #endregion
    }
}
