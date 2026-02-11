using FluentAssertions;
using MapsterMapper;
using Moq;
using OrderPaymentSystem.Application.DTOs.Payment;
using OrderPaymentSystem.Application.Interfaces.Databases;
using OrderPaymentSystem.Application.Services;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Entities;
using OrderPaymentSystem.Domain.Abstract.Interfaces.Repositories;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Enum;
using OrderPaymentSystem.Domain.Errors;
using OrderPaymentSystem.Domain.ValueObjects;
using OrderPaymentSystem.Shared.Exceptions;
using OrderPaymentSystem.Shared.Specifications;
using Xunit;

namespace OrderPaymentSystem.UnitTests.ServiceTests
{
    public class PaymentServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly PaymentService _paymentService;

        // Моки репозиториев
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;

        public PaymentServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _orderRepositoryMock = new Mock<IOrderRepository>();

            _uowMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _uowMock.Setup(uow => uow.Payments).Returns(_paymentRepositoryMock.Object);
            _uowMock.Setup(uow => uow.Orders).Returns(_orderRepositoryMock.Object);

            _paymentService = new PaymentService(_uowMock.Object);
        }

        [Fact]
        public async Task CreateAsync_NewPayment_ShouldCreatePaymentAndReturnSuccess()
        {
            // Arrange
            var createDto = new CreatePaymentDto
            {
                OrderId = 1L,
                AmountPayed = 1000m,
                Method = PaymentMethod.Cash
            };

            var product = Product.CreateExisting(2, "test", "test", 500m, 5);

            var order = Order.CreateExisting(1, Guid.NewGuid(), new Address("S", "C", "1", "Country"), new List<OrderItem>()
            {
                OrderItem.Create(2, 2, 2, product)
            }, 1000m, OrderStatus.Pending);

            // Мокируем репозитории
            _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Order>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);
            _paymentRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<BaseSpecification<Payment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false); // Платеж еще не существует
            _paymentRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()));

            // Act
            var result = await _paymentService.CreateAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _paymentRepositoryMock.Verify(r => r.CreateAsync(It.Is<Payment>(p => p.OrderId == createDto.OrderId && p.AmountPaid == createDto.AmountPayed), It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_PaymentAlreadyExists_ShouldReturnFailure()
        {
            // Arrange
            var createDto = new CreatePaymentDto { OrderId = 1L, AmountPayed = 1000m, Method = PaymentMethod.Cash };

            _paymentRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<BaseSpecification<Payment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true); // Платеж уже существует

            // Act
            var result = await _paymentService.CreateAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Payment.AlreadyExists(createDto.OrderId));
            _paymentRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_OrderNotFound_ShouldReturnFailure()
        {
            // Arrange
            var createDto = new CreatePaymentDto { OrderId = 99L, AmountPayed = 1000m, Method = PaymentMethod.Cash };

            _paymentRepositoryMock.Setup(r => r.AnyAsync(It.IsAny<BaseSpecification<Payment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Order>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order)null); // Заказ не найден

            _uowMock.Setup(uow => uow.Payments).Returns(_paymentRepositoryMock.Object);
            _uowMock.Setup(uow => uow.Orders).Returns(_orderRepositoryMock.Object);

            // Act
            var result = await _paymentService.CreateAsync(createDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Order.NotFound(createDto.OrderId));
            _paymentRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CompletePaymentAsync_ValidData_ShouldProcessPaymentAndReturnSuccess()
        {
            // Arrange
            var paymentId = 1L;
            var payment = Payment.CreateExisting(paymentId, 1, 1000m, 1000m, PaymentMethod.Cash, PaymentStatus.Pending);

            var completeDto = new CompletePaymentDto { AmountPaid = 1000m, CashChange = 0m };

            _paymentRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Payment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(payment);
            _uowMock.Setup(uow => uow.Payments).Returns(_paymentRepositoryMock.Object);

            // Act
            var result = await _paymentService.CompletePaymentAsync(paymentId, completeDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            payment.Status.Should().Be(PaymentStatus.Succeeded);
            payment.AmountPaid.Should().Be(completeDto.AmountPaid);
            payment.CashChange.Should().Be(completeDto.CashChange);
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CompletePaymentAsync_PaymentNotFound_ShouldReturnFailure()
        {
            // Arrange
            var paymentId = 99L;
            _paymentRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Payment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Payment)null);
            _uowMock.Setup(uow => uow.Payments).Returns(_paymentRepositoryMock.Object);

            // Act
            var result = await _paymentService.CompletePaymentAsync(paymentId, new CompletePaymentDto());

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Payment.NotFound(paymentId));
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CompletePaymentAsync_InsufficientAmount_ShouldThrowBusinessException()
        {
            // Arrange
            var paymentId = 1L;
            var payment = Payment.CreateExisting(paymentId, 1, 1000m, 1000m, PaymentMethod.Cash, PaymentStatus.Pending);
            var completeDto = new CompletePaymentDto { AmountPaid = 500m, CashChange = 0m };

            _paymentRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<BaseSpecification<Payment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(payment);
            _uowMock.Setup(uow => uow.Payments).Returns(_paymentRepositoryMock.Object);

            // Act
            Func<Task> act = async () => await _paymentService.CompletePaymentAsync(paymentId, completeDto);

            // Assert
            await act.Should().ThrowAsync<BusinessException>().WithMessage(DomainErrors.Payment.NotEnoughAmount(completeDto.AmountPaid, payment.AmountToPay).Message); // Проверяем сообщение исключения
            _uowMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_ValidId_ShouldReturnPaymentDto()
        {
            // Arrange
            var paymentId = 1L;
            var paymentDto = new PaymentDto { Id = paymentId, OrderId = 1L };

            _paymentRepositoryMock.Setup(r => r.GetProjectedAsync<PaymentDto>(It.IsAny<BaseSpecification<Payment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paymentDto);
            _uowMock.Setup(uow => uow.Payments).Returns(_paymentRepositoryMock.Object);

            // Act
            var result = await _paymentService.GetByIdAsync(paymentId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(paymentId);
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ShouldReturnFailure()
        {
            // Arrange
            var paymentId = 99L;
            _paymentRepositoryMock.Setup(r => r.GetProjectedAsync<PaymentDto>(It.IsAny<BaseSpecification<Payment>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PaymentDto)null);
            _uowMock.Setup(uow => uow.Payments).Returns(_paymentRepositoryMock.Object);

            // Act
            var result = await _paymentService.GetByIdAsync(paymentId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(DomainErrors.Payment.NotFound(paymentId));
        }
    }
}
