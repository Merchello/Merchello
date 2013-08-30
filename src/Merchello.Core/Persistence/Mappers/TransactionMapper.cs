using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    internal sealed class TransactionMapper : MerchelloBaseMapper
    {
        public TransactionMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<Transaction, TransactionDto>(src => src.Id, dto => dto.Id);
            CacheMap<Transaction, TransactionDto>(src => src.PaymentId, dto => dto.PaymentId);
            CacheMap<Transaction, TransactionDto>(src => src.TransactionTypeFieldKey, dto => dto.TransactionTypeFieldKey);
            CacheMap<Transaction, TransactionDto>(src => src.Description, dto => dto.Description);
            CacheMap<Transaction, TransactionDto>(src => src.Amount, dto => dto.Amount);
            CacheMap<Transaction, TransactionDto>(src => src.Exported, dto => dto.Exported);
            CacheMap<Transaction, TransactionDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<Transaction, TransactionDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            
        }
    }
}
