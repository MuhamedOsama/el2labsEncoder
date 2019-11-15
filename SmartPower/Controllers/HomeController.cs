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

        public int getJob(string mc)
        {
            //first check to see if this machine is cached
            //then check to see if it has any available unconsumed jobOrders
            //if found job order that is unconsumed it returns it
            
            Machine machine = DB.machines.FirstOrDefault(m => m.MachineCode == mc);
            if (machine!=null && machine.jobOrders.Any((j => j.Consumed==false)))
            {
                JobOrder job = machine.jobOrders.FirstOrDefault(j => !j.Consumed);
                job.Consumed = true;
                DB.SaveChanges();
                return job.JobOrderId;
            }
            //else
            //cache the machine in the machines table
            //request list of job orders
            //asigns it to the machine
            //returns one of them

            else
            {
                //get list of job orders from erp
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
                dynamic ResponseInJSON = JsonConvert.DeserializeObject(response);
                List<string> jobOrders = ResponseInJSON.jobOrders;
                Machine m1 = new Machine()
                {
                    MachineCode = mc
                };
                jobOrders.ForEach((j) =>
                {
                    m1.jobOrders.Add(new JobOrder {
                        MachineCode = mc,
                        JobOrderId = int.Parse(j),
                        TotalLength = 0,
                        StartDate = DateTime.Now,
                        Consumed = false 
                    });
                });
                JobOrder job = m1.jobOrders.FirstOrDefault(j => j.Consumed==false);
                job.Consumed = true;
                DB.SaveChanges();
                client.Dispose();
                return job.JobOrderId;
            }
        }


        public JobOrder GetJobOrderFromEncoder(int id)
        {
            var model = DB.jobOrders.FirstOrDefault(j => j.JobOrderId == id);

            return model;
        }

        public IActionResult Data()
        {

            getJob("asdad");
            var model = DB.Reading.Where(d => d.status != 2).OrderByDescending(d => d.Id).ToList();
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

                JobOrder jobOrder = GetJobOrderFromEncoder(getJob(Convert.ToString(values["pMC"].Value)));


                if (jobOrder != null)
                {
                    reading.JobOrderId = jobOrder.JobOrderId;

                }
                else
                {
                    AddJobOrder(getJob(Convert.ToString(values["pMC"].Value)), Convert.ToString(values["pMC"].Value));
                    reading.JobOrderId = getJob(Convert.ToString(values["pMC"].Value));
                }
            }
            else if (reading.Length != 0 && reading.status == 0)
            {
                JobOrder jobOrder = GetJobOrderFromEncoder(getJob(Convert.ToString(values["pMC"].Value)));

                reading.JobOrderId = getJob(Convert.ToString(values["pMC"].Value));
                reading.status = 0;
                UpdateJobOrder(jobOrder.JobOrderId, reading.Length);

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

        public void AddJobOrder(int jobId, string mc)
        {
            JobOrder jobOrder = new JobOrder()
            {
                JobOrderId = jobId,
                StartDate = DateTime.Now,
                EndDate = null,
                MachineCode = mc,
                TotalLength = 0,

            };
            DB.jobOrders.Add(jobOrder);
            DB.SaveChanges();



        }

        public void UpdateJobOrder(int jobId, decimal length)
        {
            JobOrder jobOrder = DB.jobOrders.FirstOrDefault(j => j.JobOrderId == jobId);
            jobOrder.EndDate = DateTime.Now;
            jobOrder.TotalLength = length;
            DB.jobOrders.Update(jobOrder);
            DB.SaveChanges();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
