//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.DependencyInjection;
//using SmartPower.Data.Tables;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace SmartPower.Data
//{
//    public class DBInitializer
//    {
//        public static void SeedJobOrders(IApplicationBuilder applicationBuilder)
//        {
//            using (var serviceScope = applicationBuilder.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
//                .CreateScope())
//            {
//                DataContext context = serviceScope.ServiceProvider.GetService<DataContext>();

//                if (!context.jobOrder.Any())
//                {
//                    JobOrder jobOrder1 = new JobOrder()
//                    {
//                        JobOrderId = 1,
//                        StartDate = DateTime.Now,
//                        EndDate = null,
//                        MachineCode = "m1",
//                        TotalLength = 500,

//                    };
//                    context.jobOrder.Add(jobOrder1);
//                    JobOrder jobOrder2 = new JobOrder()
//                    {
//                        JobOrderId =2 ,
//                        StartDate = DateTime.Now,
//                        EndDate = DateTime.Now.AddMinutes(10),
//                        MachineCode = "m1",
//                        TotalLength = 600,

//                    };
//                    context.jobOrder.Add(jobOrder2);
//                    JobOrder jobOrder3 = new JobOrder()
//                    {
//                        JobOrderId = 3,
//                        StartDate = DateTime.Now,
//                        EndDate = DateTime.Now.AddMinutes(5),
//                        MachineCode = "m1",
//                        TotalLength = 400,

//                    };
//                    context.jobOrder.Add(jobOrder3);
//                    JobOrder jobOrder4 = new JobOrder()
//                    {
//                        JobOrderId = 4,
//                        StartDate = DateTime.Now,
//                        EndDate = DateTime.Now.AddMinutes(10),
//                        MachineCode = "m2",
//                        TotalLength = 100,

//                    };
//                    context.jobOrder.Add(jobOrder4);
//                    JobOrder jobOrder5 = new JobOrder()
//                    {
//                        JobOrderId = 5,
//                        StartDate = DateTime.Now,
//                        EndDate = DateTime.Now.AddMinutes(15),
//                        MachineCode = "m2",
//                        TotalLength = 200,

//                    };
//                    context.jobOrder.Add(jobOrder5);
//                }

//                context.SaveChanges();
//            }

//        }
//    }
//}
