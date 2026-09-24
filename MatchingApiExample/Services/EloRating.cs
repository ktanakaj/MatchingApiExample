// ================================================================================================
// <summary>
//      Eloレーティング計算クラスソース</summary>
//
// <copyright file="EloRating.cs">
//      Copyright (C) 2026 Koichi Tanaka. All rights reserved.</copyright>
// <author>
//      Koichi Tanaka</author>
// ================================================================================================

namespace Honememo.MatchingApiExample.Services;

/// <summary>
/// 多人数のEloレーティングを計算するクラス。
/// </summary>
/// <remarks>
/// 各参加者を他の全員と総当たりし、得点差の平均にKファクターを掛けて更新する。
/// 2人のときは通常のEloと同じになる。
/// </remarks>
public static class EloRating
{
    /// <summary>
    /// 1回の対局での最大変動幅。
    /// </summary>
    public const int KFactor = 32;

    /// <summary>
    /// レート差を期待勝率に換算する尺度。400差で期待勝率が約10倍になる。
    /// </summary>
    public const int Scale = 400;

    /// <summary>
    /// 対局後のレーティングを計算する。
    /// </summary>
    /// <param name="ratings">参加者の現在レーティング。2人以上。</param>
    /// <param name="winnerIndex">勝者のインデックス。引き分けの場合はnull。</param>
    /// <returns>同じ順序の新しいレーティング。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="ratings"/>がnullの場合。</exception>
    /// <exception cref="ArgumentException">参加者が2人未満の場合。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="winnerIndex"/>が範囲外の場合。</exception>
    public static ushort[] Calculate(IReadOnlyList<ushort> ratings, int? winnerIndex)
    {
        if (ratings == null)
        {
            throw new ArgumentNullException(nameof(ratings));
        }

        if (ratings.Count < 2)
        {
            throw new ArgumentException($"Players must be greater than 1 (count={ratings.Count})", nameof(ratings));
        }

        if (winnerIndex.HasValue && (winnerIndex.Value < 0 || winnerIndex.Value >= ratings.Count))
        {
            throw new ArgumentOutOfRangeException(nameof(winnerIndex));
        }

        int opponents = ratings.Count - 1;
        var next = new ushort[ratings.Count];
        for (int i = 0; i < ratings.Count; i++)
        {
            double deltaSum = 0;
            for (int j = 0; j < ratings.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }

                double expected = ExpectedScore(ratings[i], ratings[j]);
                deltaSum += Score(i, j, winnerIndex) - expected;
            }

            double updated = ratings[i] + (KFactor * (deltaSum / opponents));
            int rounded = (int)Math.Round(updated, MidpointRounding.AwayFromZero);
            rounded = Math.Clamp(rounded, 0, (int)ushort.MaxValue);
            next[i] = (ushort)rounded;
        }

        return next;
    }

    /// <summary>
    /// iから見たjへの期待勝率を計算する。
    /// </summary>
    /// <param name="rating">自分のレーティング。</param>
    /// <param name="opponentRating">相手のレーティング。</param>
    /// <returns>期待勝率。</returns>
    private static double ExpectedScore(ushort rating, ushort opponentRating)
    {
        return 1.0 / (1.0 + Math.Pow(10.0, (opponentRating - rating) / (double)Scale));
    }

    /// <summary>
    /// iから見たjへの得点を返す。
    /// </summary>
    /// <param name="index">自分のインデックス。</param>
    /// <param name="opponentIndex">相手のインデックス。</param>
    /// <param name="winnerIndex">勝者のインデックス。引き分けの場合はnull。</param>
    /// <returns>勝利は1、敗北は0、敗者同士または引き分けは0.5。</returns>
    private static double Score(int index, int opponentIndex, int? winnerIndex)
    {
        if (index == winnerIndex)
        {
            return 1;
        }

        if (opponentIndex == winnerIndex)
        {
            return 0;
        }

        // 引き分け、または勝者以外どうしは引き分け扱いにする
        return 0.5;
    }
}
