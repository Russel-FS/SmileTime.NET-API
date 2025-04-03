using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SmileTimeNET_API.src.Aplication.services;
using SmileTimeNET_API.src.Aplication.services.chat;
using SmileTimeNET_API.src.Aplication.services.dentist;
using SmileTimeNET_API.src.Domain.Interfaces;

namespace SmileTimeNET_API.src.Infrastructure.DependencyInjection
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddAuthServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthServiceImpl>();
            return services;
        }

        public static IServiceCollection AddCarouselServices(this IServiceCollection services)
        {
            services.AddScoped<CarouselService>();
            return services;
        }

        public static IServiceCollection AddChatServices(this IServiceCollection services)
        {
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IMessageService, MessageService>();
            return services;
        }

        public static IServiceCollection AddAdminServices(this IServiceCollection services)
        {
            services.AddScoped<IAdminManagementService, AdminManagementServiceImpl>();
            return services;
        }

        public static IServiceCollection AddDentistServices(this IServiceCollection services)
        {
            services.AddScoped<IDentistManagementService, DentistManagementServiceImpl>();
            return services;
        }

        public static IServiceCollection AddPacientServices(this IServiceCollection services)
        {
            services.AddScoped<IPacientManagementService, PacientManagementServiceImpl>();
            return services;
        }
    }
}