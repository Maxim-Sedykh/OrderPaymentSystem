using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Domain.Dto.Payment;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Extensions;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Interfaces.Services;
using OrderPaymentSystem.Domain.Result;

namespace OrderPaymentSystem.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBaseRepository<Payment> _paymentRepository;
        private readonly IMapper _mapper;

        public PaymentService(IBaseRepository<Payment> paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        public async Task<BaseResult> CompletePaymentAsync(long paymentId, decimal amountPaid, decimal cashChange, CancellationToken cancellationToken = default)
        {
            var payment = await _paymentRepository.GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == paymentId, cancellationToken);
            if (payment == null)
            {
                return BaseResult.Failure(666, $"Payment with ID '{paymentId}' not found.");
            }

            payment.ProcessPayment(amountPaid, cashChange);

            _paymentRepository.Update(payment);
            await _paymentRepository.SaveChangesAsync(cancellationToken);

            return BaseResult.Success();
        }

        public async Task<BaseResult> CreateAsync(CreatePaymentDto dto, CancellationToken cancellationToken = default)
        {
            var paymentExists = await _paymentRepository.GetQueryable()
                .AnyAsync(x => x.OrderId == dto.OrderId, cancellationToken);

            if (paymentExists)
            {
                return BaseResult.Failure(4001, "Payment for this order already exists");
            }

            var payment = Payment.Create(dto.OrderId, dto.AmountToPay, dto.AmountPayed, dto.CashChange, dto.Method);

            await _paymentRepository.CreateAsync(payment, cancellationToken);
            await _paymentRepository.SaveChangesAsync(cancellationToken);

            return BaseResult.Success();
        }

        public async Task<DataResult<PaymentDto>> GetByIdAsync(long paymentId, CancellationToken cancellationToken = default)
        {
            var payment = await _paymentRepository.GetQueryable()
                .Where(x => x.Id == paymentId)
                .AsProjected<Payment, PaymentDto>(_mapper)
                .FirstOrDefaultAsync(cancellationToken);

            if (payment == null)
            {
                return DataResult<PaymentDto>.Failure(4002, "Payment not found");
            }

            return DataResult<PaymentDto>.Success(payment);
        }

        public async Task<CollectionResult<PaymentDto>> GetByOrderIdAsync(long orderId, CancellationToken cancellationToken = default)
        {
            var payments = await _paymentRepository.GetQueryable()
                .Where(x => x.OrderId == orderId)
                .AsProjected<Payment, PaymentDto>(_mapper)
                .ToArrayAsync(cancellationToken);

            return CollectionResult<PaymentDto>.Success(payments);
        }
    }
}
