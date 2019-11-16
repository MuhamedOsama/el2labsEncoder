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
                ERPresponse ResponseInJSON = JsonConvert.DeserializeObject<ERPresponse>(response);
            client.Dispose();
            return ResponseInJSON.jobOrders;
            
        }


        public ICollection<JobOrder> GetJobOrderFromEncoder(string id)
        {
            var model = DB.jobOrders.Where(m => m.MachineCode == id).ToList();
            return model;
        }

        public IActionResult Data()
        {
            //AddJobOrder(getJob("12"), "12");
            var model = DB.Reading.Where(d => d.status != 2).Include(d=>d.jobOrders).OrderByDescending(d => d.Id).ToList();
            //var jobs = DB.Reading.FirstOrDefault(r => r.MachineCode == "12").jobOrders.ToList();
            return View(model);
        }

        public IActionResult scales()
        {
            var model = DB.Reading.Where(d => d.status == 2).OrderByDescending(d => d.Id).ToList();
            return View(model);
        }

        public string Update()
        {
            var query = HttpContext.Request.Query;
            var values = query.Where(o => o.Key.StartsWith("p")).ToDictionary(o => o.Key);
            Reading reading = new Reading()
            {

                MachineCode = Convert.ToString(values["pMC"].Value),
                Length = Convert.ToDecimal(values["pLength"].Value),

                status = Convert.ToInt32(values["pST"].Value),

                time = DateTime.Now,
            };

            if (reading.Length == 0 && reading.status == 1)
            {

                var jobOrders = DB.jobOrders.Where(j => j.MachineCode == Convert.ToString(values["pMC"].Value)).ToList();


                if (jobOrders != null)
                {
                    reading.jobOrders = jobOrders;

                }
                else
                {
                    AddJobOrder(getJob(Convert.ToString(values["pMC"].Value)), Convert.ToString(values["pMC"].Value));
                    reading.jobOrders = jobOrders;
                }
            }
            else if (reading.Length != 0 && reading.status == 0)
            {
                var jobOrders = DB.jobOrders.Where(j => j.MachineCode == Convert.ToString(values["pMC"].Value));

                reading.jobOrders = jobOrders.ToList();
                reading.status = 0;
                jobOrders.ForEachAsync(j =>
                {
                    j.EndDate = DateTime.Now;
                    j.TotalLength = reading.Length;
                });
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
        public void AddJobOrder(ICollection<string> Jobs, string mc)
        {
            var readings = DB.Reading.Where(r => r.MachineCode == mc).Include(r=>r.jobOrders);
            var JobList = Jobs.ToList();
            DateTime date = DateTime.Now;
            Console.WriteLine("About to print JOBS before ADDING");
            JobList.ForEach(async j =>
            {
                Console.WriteLine("Adding this order: " + j);
                JobOrder jobOrder = new JobOrder()
                {
                    
                    JobOrderId = j,
                    StartDate = date,
                    EndDate = null,
                    MachineCode = mc,
                    TotalLength = 0,
                };
                await readings.ForEachAsync(r => r.jobOrders.Add(jobOrder));
                await DB.jobOrders.AddAsync(jobOrder);
                await DB.SaveChangesAsync();
            });

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
