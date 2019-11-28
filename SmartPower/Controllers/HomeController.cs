using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using LG.Models;

using Microsoft.Extensions.Configuration;
using SmartPower.Data.Tables;
using Microsoft.EntityFrameworkCore;
using SmartPower.Data;
using System.Net;
using RestSharp;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SmartPower.Controllers
{
    public class HomeController : Controller
    {

        public DataContext DB;
        public HomeController(DataContext dataContext)
        {

            this.DB = dataContext;
        }

        public ICollection<string> getJob(string mc)
        {
            

            
                //get list of job orders from erp for that machine
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get,
               "https://localhost:44368/api/encoderlogs/Response");
                var response = client.GetAsync("https://localhost:44368/api/encoderlogs/Response").Result.Content.ReadAsStringAsync().Result;
                //the structure of the incoming json response:
                /*{
                jobOrders: [
                    12,
                    32,
                    25
                ],
                accessToken: "w489dkfn934809048293i09e22i03",
                status: "Recieved"
                }*/
                List <ERPresponse> ResponseInJSON = JsonConvert.DeserializeObject<List<ERPresponse>>(response);
            client.Dispose();
            ICollection<string> JobOrdersIds = new List<string> ();
            ResponseInJSON.ForEach((j)=>j.jobOrders.ForEach((m=> JobOrdersIds.Add(m.jobOrderId))));
            return JobOrdersIds;
            
        }


        public ICollection<JobOrder> GetJobOrderFromEncoder(string id)
        {
            var model = DB.jobOrders.Where(m => m.MachineCode == id).ToList();
            return model;
        }

        public IActionResult Data()
        {
           
            
            //AddJobOrder(getJob("12"), "12");
            var model = DB.Reading.Where(d => d.status != 2).Include(d=>d.JobOrders).OrderByDescending(d => d.Id).ToList();
            //var jobs = DB.Reading.FirstOrDefault(r => r.MachineCode == "12").jobOrders.ToList();
            return View(model);
        }

        public IActionResult scales()
        {
            var model = DB.Reading.Where(d => d.status == 2).OrderByDescending(d => d.Id).ToList();
            return View(model);
        }

        public async System.Threading.Tasks.Task<string> Update()
        {
            var query = HttpContext.Request.Query;
            var values = query.Where(o => o.Key.StartsWith("p")).ToDictionary(o => o.Key);
            Console.WriteLine("Recieved");
            Reading reading = new Reading()
            {

                MachineCode = Convert.ToString(values["pMC"].Value),
                Length = Convert.ToDecimal(values["pLength"].Value),

                status = Convert.ToInt32(values["pST"].Value),

                time = DateTime.Now,
            };
            //machine starting 
            if (reading.Length == 0 && reading.status == 1)
            {
                Console.WriteLine("Status is 1");

                var jobOrders = DB.jobOrders.Where(j => j.MachineCode == Convert.ToString(values["pMC"].Value)).ToList();

                if (jobOrders.Any())
                {
                    Console.WriteLine("job orders already exist");
                    reading.JobOrders = jobOrders;
                    DB.SaveChanges();
                }
                else
                {
                    Console.WriteLine("getting and adding job orders");
                    AddJobOrder(getJob(Convert.ToString(values["pMC"].Value)),reading);
                    reading.JobOrders = jobOrders;
                }
            }
            else if (reading.Length != 0 && reading.status == 0)
            {
                Console.WriteLine("Status is 0 complete");
                var jobOrders = DB.jobOrders.Where(j => j.MachineCode == Convert.ToString(values["pMC"].Value) && j.TotalLength == 0);

                reading.status = 0;
                await jobOrders.ForEachAsync(j =>
                {
                    j.EndDate = DateTime.Now;
                    j.TotalLength = reading.Length;
                });
                reading.JobOrders = jobOrders.ToList();
                DB.jobOrders.UpdateRange(jobOrders);
                DB.SaveChanges();

            }//else if(encoder.Length != 0 && encoder.status == 2)
             //{

            //}


            DB.Reading.Add(reading);
            //DB.re.Add(encoder);
            DB.SaveChanges();
            return $"ok";
        }
        public IActionResult Index()
        {
            return RedirectToAction("Data");
        }


        public IActionResult JobOrders()
        {
            var model = DB.jobOrders.OrderByDescending(j => j.Id).ToList();
            return View(model);
        }

        //takes the list of job orders and assigns each one to the machine (machine code)
        public void AddJobOrder(ICollection<string> Jobs, Reading reading)
        {
            var JobList = Jobs.ToList();
            DateTime date = DateTime.Now;
            Console.WriteLine("About to print JOBS before ADDING");
            JobList.ForEach(j =>
            {
                JobOrder jobOrder = new JobOrder()
                {

                    JobOrderId = j,
                    StartDate = date,
                    EndDate = null,
                    MachineCode = reading.MachineCode,
                    TotalLength = 0
                };
                DB.jobOrders.Add(jobOrder);
            });
            DB.SaveChanges();

            //DB.jobOrders.AddRange(jobOrders);
            //readings.ForEachAsync(r => r.jobOrders = jobOrders);
            //DB.SaveChangesAsync();


        }

        //public void UpdateJobOrder(ICollection <JobOrder> orders, decimal length)
        //{
        //    JobOrder jobOrder = DB.jobOrders.FirstOrDefault(j => j.JobOrderId == jobId);
        //    jobOrder.EndDate = DateTime.Now;
        //    jobOrder.TotalLength = length;
        //    DB.jobOrders.Update(jobOrder);
        //    DB.SaveChanges();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
