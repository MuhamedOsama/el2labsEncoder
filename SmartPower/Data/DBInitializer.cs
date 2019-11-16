using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SmartPower.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPower.Data
{
    public class DBInitializer
    {
        public static void SeedJobOrders(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                DataContext context = serviceScope.ServiceProvider.GetService<DataContext>();

                if (!context.Reading.Any())
                {
                    Reading r1 = new Reading()
                    {
                        MachineCode = "12",
                        Length = 0,
                        status = 1,
                        time = DateTime.Now,
                    };
                    context.Reading.Add(r1);
                }

                context.SaveChanges();
            }

        }
    }
}
