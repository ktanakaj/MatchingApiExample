// ================================================================================================
// <summary>
//      バリデーションgRPCインターセプタークラスソース</summary>
//
// <copyright file="ValidationInterceptor.cs">
//      Copyright (C) 2020 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Interceptors
{
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using Grpc.Core;
    using Grpc.Core.Interceptors;

    /// <summary>
    /// バリデーションgRPCインターセプタークラス。
    /// </summary>
    public class ValidationInterceptor : Interceptor
    {
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
            // バリデーターに渡せるパラメータの場合、バリデータを通す
            if (request != null && request.GetType().IsClass)
            {
                Validator.ValidateObject(request, new ValidationContext(request));
            }

            return await continuation(request, context);
        }

        #endregion
    }
}
