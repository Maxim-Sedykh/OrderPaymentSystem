using OrderPaymentSystem.Domain.Dto;
using OrderPaymentSystem.Domain.Dto.Report;
using OrderPaymentSystem.Domain.Dto.User;
using OrderPaymentSystem.Domain.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Interfaces.Services
{
    /// <summary>
    /// Сервис отвечающий за работу с доменной части пользователя (User)
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Получение всех пользователей
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CollectionResult<UserDto>> GetUsersAsync(long id);

        /// <summary>
        /// Получение пользователя по идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseResult<UserDto>> GetUserByIdAsync(long id);

        /// <summary>
        /// Регистрация сотрудника
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<UserDto>> RegisterEmployeeAsync(CreateUserDto dto);

        /// <summary>
        /// Регистрация клиента
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<UserDto>> RegisterCustomerAsync(CreateUserDto dto);

        /// <summary>
        /// Логгирование пользователя
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<UserDto>> LoginUserAsync(CreateUserDto dto);

        /// <summary>
        /// Выход пользователя из аккаунта
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<UserDto>> LogOutUserAsync(CreateUserDto dto);

        /// <summary>
        /// Удаление пользователя по идентификатору
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<UserDto>> DeleteUserAsync(long id);

        /// <summary>
        /// Обновление пользователя
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<UserDto>> UpdateUserAsync(UpdateUserDto dto);
    }
}
