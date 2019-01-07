using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Marketplace.ClassifiedAd
{
    public static class Queries
    {
        public static async Task<IEnumerable<ReadModels.PublicClassifiedAdListItem>> Query(
            this DbConnection connection,
            QueryModels.GetPublishedClassifiedAds query)
        {
            await connection.OpenAsync();
            return await connection.QueryAsync<ReadModels.PublicClassifiedAdListItem>(
                "SELECT \"ClassifiedAdId\", \"Price_Amount\" price, \"Title_Value\" title " +
                "FROM \"ClassifiedAds\" WHERE \"State\"=@State LIMIT @PageSize OFFSET @Offset",
                new
                {
                    State = (int)Domain.ClassifiedAd.ClassifiedAd.ClassifiedAdState.Active,
                    PageSize = query.PageSize,
                    Offset = Offset(query.Page, query.PageSize)
                });
        }

        public static async Task<IEnumerable<ReadModels.PublicClassifiedAdListItem>> Query(
            this DbConnection connection,
            QueryModels.GetOwnersClassifiedAd query)
        {
            await connection.OpenAsync();
            return await connection.QueryAsync<ReadModels.PublicClassifiedAdListItem>(
                "SELECT \"ClassifiedAdId\", \"Price_Amount\" price, \"Title_Value\" title " +
                "FROM \"ClassifiedAds\" WHERE \"OwnerId_Value\"=@OwnerId LIMIT @PageSize OFFSET @Offset",
                    new
                    {
                        OwnerId = query.OwnerId,
                        PageSize = query.PageSize,
                        Offset = Offset(query.Page, query.PageSize)
                    });
        }

        public static async Task<ReadModels.ClassifiedAdDetails> Query(
            this DbConnection connection,
            QueryModels.GetPublicClassifiedAd query)
        {
            await connection.OpenAsync();
            return await connection.QuerySingleOrDefaultAsync<ReadModels.ClassifiedAdDetails>(
                "SELECT \"ClassifiedAdId\", \"Price_Amount\" price, \"Title_Value\" title, " +
                "\"Text_Value\" description, \"DisplayName_Value\" sellersdisplayname " +
                "FROM \"ClassifiedAds\", \"UserProfiles\" " +
                "WHERE \"ClassifiedAdId\" = @Id AND \"OwnerId_Value\"=\"UserProfileId\"",
                    new { Id = query.ClassifiedAdId });
        }

        private static int Offset(int page, int pageSize) => page * pageSize;
    }
}