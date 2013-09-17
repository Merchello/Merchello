using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class TransactionFactory : IEntityFactory<ITransaction, TransactionDto>
    {
        public ITransaction BuildEntity(TransactionDto dto)
        {


            var transaction = new Transaction(
                dto.PaymentId, 
                dto.InvoiceId, 
                dto.TransactionTypeFieldKey)
            {
                Id = dto.Id,
                Description = dto.Description,
                Amount = dto.Amount,
                Exported = dto.Exported,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            transaction.ResetDirtyProperties();

            return transaction;
        }

        public TransactionDto BuildDto(ITransaction entity)
        {
            var dto = new TransactionDto()
            {
                Id = entity.Id,
                PaymentId = entity.PaymentId,
                InvoiceId = entity.InvoiceId,
                TransactionTypeFieldKey = entity.TransactionTypeFieldKey,
                Description = entity.Description,
                Amount = entity.Amount,
                Exported = entity.Exported,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate                
            };

            return dto;
        }
    }
}
